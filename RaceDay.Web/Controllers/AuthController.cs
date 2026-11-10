
using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;
using System.Text.Json;

namespace RaceDay.Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;

        public AuthController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Auth/Login
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var loginData = new
                {
                    model.Email,
                    model.Password
                };

                var response =
                    await _apiService.LoginAsync<object, AuthApiResponse>(
                        "api/Auth/login", loginData);

                if (response?.User == null)
                {
                    ModelState.AddModelError(
                        string.Empty, "Login failed. Please try again.");

                    return View(model);
                }

                // Store authenticated user details in MVC session.
                HttpContext.Session.SetInt32(
                    "UserID", response.User.UserID);

                HttpContext.Session.SetString(
                    "FirstName", response.User.FirstName);

                HttpContext.Session.SetString(
                    "LastName", response.User.LastName);

                HttpContext.Session.SetString(
                    "Email", response.User.Email);

                HttpContext.Session.SetString(
                    "Role", response.User.Role);

                TempData["SuccessMessage"] =
                    "Welcome back, " + response.User.FirstName + "!";

                // Temporary landing page until dashboards are implemented.
                return RedirectToAction("Index", "Home");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(
                    string.Empty, GetApiErrorMessage(ex));

                return View(model);
            }
        }

        // GET: /Auth/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Auth/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(
            RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                // Send only the properties expected by the API.
                var registerData = new
                {
                    model.FirstName,
                    model.LastName,
                    model.Email,
                    model.Password
                };

                var response =
                    await _apiService.PostAsync<object, AuthApiResponse>(
                        "api/Auth/register", registerData);

                TempData["SuccessMessage"] =
                    response?.Message ?? "Registration successful.";

                return RedirectToAction("Login");
            }
            catch (HttpRequestException ex)
            {
                ModelState.AddModelError(
                    string.Empty, GetApiErrorMessage(ex));

                return View(model);
            }
        }

        // POST: /Auth/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            try
            {
                await _apiService.LogoutAsync("api/Auth/logout");
            }
            catch (HttpRequestException)
            {
                // Clear the local session even if API logout fails.
            }
            finally
            {
                HttpContext.Session.Clear();
            }

            TempData["SuccessMessage"] = "You have been logged out.";

            return RedirectToAction("Login");
        }

        // Extract the API's error message.
        private static string GetApiErrorMessage(
            HttpRequestException ex)
        {
            const string marker = "Details: ";

            int index = ex.Message.IndexOf(
                marker, StringComparison.OrdinalIgnoreCase);

            if (index >= 0)
            {
                string details = ex.Message[(index + marker.Length)..];

                try
                {
                    using var document = JsonDocument.Parse(details);

                    if (document.RootElement.TryGetProperty(
                        "message", out var message))
                    {
                        return message.GetString()
                            ?? "The request could not be completed.";
                    }
                }
                catch (JsonException)
                {
                    // Use the original exception message below.
                }
            }

            return "Unable to communicate with the API. Please try again.";
        }
    }
}
