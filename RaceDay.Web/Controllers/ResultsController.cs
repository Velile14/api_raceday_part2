using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    public class ResultsController : Controller
    {
        private readonly ApiService _apiService;

        public ResultsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // Participants: view personal results.
        public async Task<IActionResult> MyResults()
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            try
            {
                var results = await _apiService
                    .GetAsync<List<ResultViewModel>>("api/Results/my");

                return View(results ?? new List<ResultViewModel>());
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<ResultViewModel>());
            }
        }
    }
}
