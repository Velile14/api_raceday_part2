using api_raceday_part2.models;
using api_raceday_part2.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api_raceday_part2.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {//start of AuthController class

        private readonly ApplicationDBContext _context;

        public AuthController(ApplicationDBContext context)
        {
            _context = context;
        }

        // POST: api/Auth/register
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register( 
            [FromBody] RegisterDto dto)

        {//start of register method

            string email = dto.Email.Trim();

            bool emailExists = await _context.Users
                .AnyAsync(u => u.Email == email);

            if (emailExists)
            {//start of if block

                return Conflict(new
                {

                    message = "An account with this email already exists."

                });

            }//end of if block

            //find the default participant role
            var participantRole = await _context.Roles
                .FirstOrDefaultAsync(r => r.RoleName == "Participant");

            if (participantRole == null)
            {//start of if block

                return StatusCode(500, new
                {

                    message = "The Participant role is not configured."

                });

            }//end of if block

            //create the user and hash password
            var user = new User
            {//start of user object initialization

                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = email,
                Password = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                RoleID = participantRole.RoleID

            };//end of user object initialization

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            //return safe user details only
            return StatusCode(201, new
            {

                message = "registration successful.",
                user = new
                {
                    user.UserID,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    Role = participantRole.RoleName
                }

            });

        }//end of register method

        // POST: api/Auth/login
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<IActionResult> Login(
            [FromBody] LoginDto dto)

        {//start of login method 

            string email = dto.Email.Trim();

            //find the user and load their role
            var user = await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Email == email);

            //verify the submitted password against the hashed password
            if (user == null ||
                !BCrypt.Net.BCrypt.Verify(dto.Password, user.Password))
            {//start of if block

                return Unauthorized(new
                {
                    message = "Invalid email or password."
                });

            }//end of if block

            //create identity claims
            var claims = new List<Claim>
            {//start of claims list initialization

                new Claim(
                    ClaimTypes.NameIdentifier,
                    user.UserID.ToString()),

                new Claim(
                    ClaimTypes.Name,
                    user.Email),

                new Claim(
                    ClaimTypes.Role,
                    user.Role.RoleName)

            };//end of claims list initialization

            var identity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme); 

            var principal = new ClaimsPrincipal(identity);

            //issue the authentication cookie
            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                principal,
                new AuthenticationProperties
                {

                    IsPersistent = false

                });

            return Ok(new
            {//start of return block

                message = "login successful.",
                user = new
                {
                    user.UserID,
                    user.FirstName,
                    user.LastName,
                    user.Email,
                    Role = user.Role.RoleName
                }

            });//end of return block

        }//end of login method

        // POST: api/Auth/logout
        [Authorize]
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {//start of logout method

            await HttpContext.SignOutAsync(
                CookieAuthenticationDefaults.AuthenticationScheme);

            return Ok(new
            {
                message = "logout successful."
            });

        }//end of logout method

    }//end of AuthController class

}//end of namespace
