using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using api_raceday_part2.Controllers;
using api_raceday_part2.Data;
using api_raceday_part2.models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Xunit;

namespace raceday_unit_test
{
    public class RaceDayTests
    {
        private static ApplicationDBContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDBContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new ApplicationDBContext(options);
        }

        private static void ConfigureHttpContext(AuthController controller)
        {
            var services = new ServiceCollection();

            services.AddLogging();
            services
                .AddAuthentication(
                    CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(
                    CookieAuthenticationDefaults.AuthenticationScheme);

            var provider = services.BuildServiceProvider();

            var httpContext = new DefaultHttpContext
            {
                RequestServices = provider
            };

            httpContext.Request.Scheme = "https";

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = httpContext
            };
        }

        [Fact]
        public async Task Register_WithValidDetails_Returns201AndSavesUser()
        {
            // Arrange
            using var context = CreateContext();

            var participantRole = new Role
            {
                RoleName = "Participant"
            };

            context.Roles.Add(participantRole);
            await context.SaveChangesAsync();

            var controller = new AuthController(context);

            var dto = new RegisterDto
            {
                FirstName = "Test",
                LastName = "Participant",
                Email = "test@example.com",
                Password = "SecurePass123!"
            };

            // Act
            var actionResult = await controller.Register(dto);

            // Assert
            var result = Assert.IsType<ObjectResult>(actionResult);
            Assert.Equal(201, result.StatusCode);

            var savedUser = await context.Users
                .SingleOrDefaultAsync(u => u.Email == "test@example.com");

            Assert.NotNull(savedUser);

            Assert.True(
                savedUser.FirstName == "Test",
                $"FirstName mismatch. Expected: Test, Actual: {savedUser.FirstName}");

            Assert.True(
                savedUser.LastName == "Participant",
                $"LastName mismatch. Expected: Participant, Actual: {savedUser.LastName}");

            var roles = await context.Roles
                .Select(r => new { r.RoleID, r.RoleName })
                .ToListAsync();

            foreach (var role in roles)
            {
                Console.WriteLine($"RoleID: {role.RoleID}, RoleName: {role.RoleName}");
            }

            Console.WriteLine($"Seeded Participant RoleID: {participantRole.RoleID}");
            Console.WriteLine($"Saved User RoleID: {savedUser.RoleID}");

            Assert.True(
                savedUser.RoleID == participantRole.RoleID,
                $"RoleID mismatch. Expected: {participantRole.RoleID}, Actual: {savedUser.RoleID}");

            Assert.NotEqual("SecurePass123!", savedUser.Password);
            Assert.True(
                BCrypt.Net.BCrypt.Verify(
                    "SecurePass123!", savedUser.Password));
        }

        [Fact]
        public async Task Register_WithDuplicateEmail_Returns409Conflict()
        {
            // Arrange
            using var context = CreateContext();

            var participantRole = new Role
            {
                RoleName = "Participant"
            };

            context.Roles.Add(participantRole);
            await context.SaveChangesAsync();

            context.Users.Add(new User
            {
                FirstName = "Existing",
                LastName = "User",
                Email = "existing@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("ExistingPass123!"),
                RoleID = participantRole.RoleID
            });

            await context.SaveChangesAsync();

            var controller = new AuthController(context);

            var dto = new RegisterDto
            {
                FirstName = "Another",
                LastName = "User",
                Email = "existing@example.com",
                Password = "AnotherPass123!"
            };

            // Act
            var actionResult = await controller.Register(dto);

            // Assert
            Assert.IsType<ConflictObjectResult>(actionResult);

            var usersWithEmail = await context.Users
                .CountAsync(u => u.Email == "existing@example.com");

            Assert.Equal(1, usersWithEmail);
        }

        [Fact]
        public async Task Login_WithCorrectCredentials_Returns200()
        {
            // Arrange
            using var context = CreateContext();

            var role = new Role { RoleName = "Participant" };
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            context.Users.Add(new User
            {
                FirstName = "Test",
                LastName = "Participant",
                Email = "login@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("CorrectPass123!"),
                RoleID = role.RoleID
            });

            await context.SaveChangesAsync();

            var controller = new AuthController(context);
            ConfigureHttpContext(controller);

            var dto = new LoginDto
            {
                Email = "login@example.com",
                Password = "CorrectPass123!"
            };

            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task Login_WithIncorrectPassword_ReturnsUnauthorized()
        {
            // Arrange
            using var context = CreateContext();

            var role = new Role { RoleName = "Participant" };
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            context.Users.Add(new User
            {
                FirstName = "Test",
                LastName = "Participant",
                Email = "login@example.com",
                Password = BCrypt.Net.BCrypt.HashPassword("CorrectPass123!"),
                RoleID = role.RoleID
            });

            await context.SaveChangesAsync();

            var controller = new AuthController(context);

            var dto = new LoginDto
            {
                Email = "login@example.com",
                Password = "WrongPass123!"
            };

            // Act
            var result = await controller.Login(dto);

            // Assert
            Assert.IsType<UnauthorizedObjectResult>(result);
        }


        private static void SetAuthenticatedUser(
            ControllerBase controller, int userId, string roleName)
        {
            var claims = new[]
            {
        new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
        new Claim(ClaimTypes.Role, roleName)
    };

            controller.ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(
                        new ClaimsIdentity(claims, "Test"))
                }
            };
        }

        [Fact]
        public async Task Organiser_CreatesEvent_EventIsSaved()
        {
            // Arrange
            using var context = CreateContext();

            var role = new Role { RoleName = "Organiser" };
            context.Roles.Add(role);
            await context.SaveChangesAsync();

            var organiser = new User
            {
                FirstName = "Test",
                LastName = "Organiser",
                Email = "organiser@example.com",
                Password = "test-hash",
                RoleID = role.RoleID,
                Role = role
            };
            context.Users.Add(organiser);

            var eventType = new EventType { TypeName = "Running" };
            context.EventTypes.Add(eventType);
            await context.SaveChangesAsync();

            var controller = new EventsController(context);
            SetAuthenticatedUser(controller, organiser.UserID, "Organiser");

            var newEvent = new Event
            {
                EventName = "Test Race",
                Description = "Unit test event",
                EventDate = new DateOnly(2027, 5, 10),
                Location = "Johannesburg",
                Distance = 10,
                EventTypeID = eventType.EventTypeID
            };

            // Act
            var result = controller.CreateEvent(newEvent);

            // Assert
            Assert.IsType<OkObjectResult>(result);

            var savedEvent = await context.Events
                .SingleOrDefaultAsync(e => e.EventName == "Test Race");

            Assert.NotNull(savedEvent);
            Assert.Equal(organiser.UserID, savedEvent.OrganizerID);
            Assert.Equal(organiser.UserID, savedEvent.UserID);
        }

        [Fact]
        public void CreateEvent_RequiresOrganiserRole()
        {
            // Assert that the action's authorization metadata restricts access
            // to Organisers. This does not simulate an HTTP request.
            var method = typeof(EventsController)
                .GetMethod(nameof(EventsController.CreateEvent));

            Assert.NotNull(method);

            var authorize = method!
                .GetCustomAttributes(typeof(AuthorizeAttribute), true)
                .Cast<AuthorizeAttribute>()
                .SingleOrDefault();

            Assert.NotNull(authorize);
            Assert.Equal("Organiser", authorize!.Roles);
        }

        [Fact]
        public async Task Participant_CreatesEnrolment_EnrolmentIsSaved()
        {
            // Arrange
            using var context = CreateContext();

            var participantRole = new Role { RoleName = "Participant" };
            var organiserRole = new Role { RoleName = "Organiser" };

            context.Roles.AddRange(participantRole, organiserRole);
            await context.SaveChangesAsync();

            var participant = new User
            {
                FirstName = "Test",
                LastName = "Participant",
                Email = "participant@example.com",
                Password = "test-hash",
                RoleID = participantRole.RoleID,
                Role = participantRole
            };

            var organiser = new User
            {
                FirstName = "Test",
                LastName = "Organiser",
                Email = "eventorganiser@example.com",
                Password = "test-hash",
                RoleID = organiserRole.RoleID,
                Role = organiserRole
            };

            context.Users.AddRange(participant, organiser);

            var eventType = new EventType { TypeName = "Running" };
            context.EventTypes.Add(eventType);
            await context.SaveChangesAsync();

            var raceEvent = new Event
            {
                EventName = "Enrolment Test Race",
                Description = "Race for unit testing",
                EventDate = new DateOnly(2027, 6, 1),
                Location = "Johannesburg",
                Distance = 10,
                EventTypeID = eventType.EventTypeID,
                UserID = organiser.UserID,
                OrganizerID = organiser.UserID
            };

            context.Events.Add(raceEvent);
            await context.SaveChangesAsync();

            var category = new Category
            {
                EventID = raceEvent.EventID,
                CategoryName = "Senior",
                MinimumAge = 20,
                MaximumAge = 59,
                Distance = 10
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var controller = new EnrolmentsController(context);
            SetAuthenticatedUser(controller, participant.UserID, "Participant");

            var enrolment = new Enrolment
            {
                EventID = raceEvent.EventID,
                CategoryID = category.CategoryID,
                UserID = -1 // The controller must use the authenticated user's ID.
            };

            // Act
            var result = await controller.CreateEnrolment(enrolment);

            // Assert
            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.NotNull(created.Value);

            var savedEnrolment = await context.Enrolments
                .SingleOrDefaultAsync(e =>
                    e.EventID == raceEvent.EventID &&
                    e.UserID == participant.UserID);

            Assert.NotNull(savedEnrolment);
            Assert.Equal(category.CategoryID, savedEnrolment.CategoryID);
            Assert.Equal(participant.UserID, savedEnrolment.UserID);
        }

        [Fact]
        public async Task Participant_CanRetrieveSavedEnrolment()
        {
            // Arrange
            using var context = CreateContext();

            var participantRole = new Role { RoleName = "Participant" };
            var organiserRole = new Role { RoleName = "Organiser" };

            context.Roles.AddRange(participantRole, organiserRole);
            await context.SaveChangesAsync();

            var participant = new User
            {
                FirstName = "Test",
                LastName = "Participant",
                Email = "retrieve@example.com",
                Password = "test-hash",
                RoleID = participantRole.RoleID,
                Role = participantRole
            };

            var organiser = new User
            {
                FirstName = "Test",
                LastName = "Organiser",
                Email = "retrieveorganiser@example.com",
                Password = "test-hash",
                RoleID = organiserRole.RoleID,
                Role = organiserRole
            };

            context.Users.AddRange(participant, organiser);

            var eventType = new EventType { TypeName = "Running" };
            context.EventTypes.Add(eventType);
            await context.SaveChangesAsync();

            var raceEvent = new Event
            {
                EventName = "Retrieval Test Race",
                Description = "Race for retrieval testing",
                EventDate = new DateOnly(2027, 7, 1),
                Location = "Johannesburg",
                Distance = 10,
                EventTypeID = eventType.EventTypeID,
                UserID = organiser.UserID,
                OrganizerID = organiser.UserID
            };

            context.Events.Add(raceEvent);
            await context.SaveChangesAsync();

            var category = new Category
            {
                EventID = raceEvent.EventID,
                CategoryName = "Senior",
                MinimumAge = 20,
                MaximumAge = 59,
                Distance = 10
            };

            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var saved = new Enrolment
            {
                EventID = raceEvent.EventID,
                CategoryID = category.CategoryID,
                UserID = participant.UserID
            };

            context.Enrolments.Add(saved);
            await context.SaveChangesAsync();

            var controller = new EnrolmentsController(context);
            SetAuthenticatedUser(controller, participant.UserID, "Participant");

            // Act
            var result = await controller.GetEnrolment(saved.EnrolmentID);

            // Assert
            var response = Assert.IsType<OkObjectResult>(result.Result);
            var retrieved = Assert.IsType<Enrolment>(response.Value);

            Assert.Equal(saved.EnrolmentID, retrieved.EnrolmentID);
            Assert.Equal(participant.UserID, retrieved.UserID);
            Assert.Equal(raceEvent.EventID, retrieved.EventID);
        }
    }
}
