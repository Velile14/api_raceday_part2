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


        // Organiser: view participants enrolled in an event.
        public async Task<IActionResult> EventEnrolments(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            try
            {
                var enrolments = await _apiService
                    .GetAsync<List<EnrolmentViewModel>>(
                        $"api/Enrolments/event/{eventId}");

                ViewBag.EventID = eventId;

                return View(enrolments ?? new List<EnrolmentViewModel>());
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                ViewBag.EventID = eventId;

                return View(new List<EnrolmentViewModel>());
            }
        }

        // Organiser: capture a participant's result.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Capture(CreateResultViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            if (model.FinishPosition <= 0 || model.FinishTime < TimeSpan.Zero)
            {
                TempData["ErrorMessage"] = "Enter a valid finish position and time.";
                return RedirectToAction(
                    nameof(EventEnrolments), new { eventId = model.EventID });
            }

            try
            {
                var request = new
                {
                    model.EnrolmentID,
                    model.FinishTime,
                    model.FinishPosition
                };

                await _apiService.PostAsync<object, ResultViewModel>(
                    "api/Results", request);

                TempData["SuccessMessage"] = "Result successfully recorded.";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(
                nameof(EventEnrolments), new { eventId = model.EventID });
        }

    }
}
