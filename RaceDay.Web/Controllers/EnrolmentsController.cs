using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{
    public class EnrolmentsController : Controller
    {
        private readonly ApiService _apiService;

        public EnrolmentsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Enrolments
        // Display the logged-in participant's enrolments.
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            try
            {
                var enrolments = await _apiService
                    .GetAsync<List<EnrolmentViewModel>>(
                        "api/Enrolments/my");

                return View(enrolments ?? new List<EnrolmentViewModel>());
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<EnrolmentViewModel>());
            }
        }

        // GET: /Enrolments/Create?eventId=1
        // Load categories belonging to the selected event.
        public async Task<IActionResult> Create(int eventId)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            try
            {
                var categories = await _apiService
                    .GetAsync<List<CategoryViewModel>>("api/Categories");

                var model = new CreateEnrolmentViewModel
                {
                    EventID = eventId,
                    Categories = (categories ?? new List<CategoryViewModel>())
                        .Where(c => c.EventID == eventId)
                        .ToList()
                };

                if (!model.Categories.Any())
                {
                    ViewBag.ErrorMessage =
                        "There are no categories available for this event.";
                }

                return View(model);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;

                return View(new CreateEnrolmentViewModel
                {
                    EventID = eventId
                });
            }
        }

        // POST: /Enrolments/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            CreateEnrolmentViewModel model)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            try
            {
                var categories = await _apiService
                    .GetAsync<List<CategoryViewModel>>("api/Categories");

                model.Categories = (categories ?? new List<CategoryViewModel>())
                    .Where(c => c.EventID == model.EventID)
                    .ToList();

                if (!model.Categories.Any(c =>
                    c.CategoryID == model.CategoryID))
                {
                    ViewBag.ErrorMessage =
                        "Please select a valid category for this event.";

                    return View(model);
                }

                var request = new
                {
                    EventID = model.EventID,
                    CategoryID = model.CategoryID
                };

                await _apiService.PostAsync<object, EnrolmentViewModel>(
                    "api/Enrolments", request);

                TempData["SuccessMessage"] =
                    "You have successfully entered the event!";

                return RedirectToAction(nameof(Index));
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(model);
            }
        }

        // POST: /Enrolments/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (HttpContext.Session.GetString("Role") != "Participant")
                return RedirectToAction("Login", "Auth");

            try
            {
                await _apiService.DeleteAsync($"api/Enrolments/{id}");

                TempData["SuccessMessage"] =
                    "Your enrolment has been cancelled.";
            }
            catch (HttpRequestException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }

        // Organiser: view enrolments across all owned events.
        public async Task<IActionResult> Manage()
        {
            if (HttpContext.Session.GetString("Role") != "Organiser")
                return RedirectToAction("Login", "Auth");

            if (!int.TryParse(
                HttpContext.Session.GetString("UserID"),
                out int userId))
            {
                return RedirectToAction("Login", "Auth");
            }

            try
            {
                var events = await _apiService
                    .GetAsync<List<EventViewModel>>("api/Events");

                var organiserEvents = (events ?? new List<EventViewModel>())
                    .Where(e => e.OrganizerID == userId)
                    .ToList();

                var allEnrolments = new List<EnrolmentViewModel>();

                foreach (var raceEvent in organiserEvents)
                {
                    var enrolments = await _apiService
                        .GetAsync<List<EnrolmentViewModel>>(
                            $"api/Enrolments/event/{raceEvent.EventID}");

                    if (enrolments != null)
                        allEnrolments.AddRange(enrolments);
                }

                return View(allEnrolments);
            }
            catch (HttpRequestException ex)
            {
                ViewBag.ErrorMessage = ex.Message;
                return View(new List<EnrolmentViewModel>());
            }
        }

    }
}
