using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Services;
using RaceDay.Web.ViewModels;

namespace RaceDay.Web.Controllers
{//start of namespace

    public class EventsController : Controller
    {//start of the EventsController class

        private readonly ApiService _apiService;

        public EventsController(ApiService apiService)
        {
            _apiService = apiService;
        }

        // GET: /Events
        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _apiService
                    .GetAsync<List<EventViewModel>>("api/Events");

                return View(events ?? new List<EventViewModel>());
            }
            catch (HttpRequestException)
            {
                ViewBag.ErrorMessage =
                    "Unable to retrieve events. Please try again later.";

                return View(new List<EventViewModel>());
            }
        }

    }//end of the EventsController class

}//end of namespace
