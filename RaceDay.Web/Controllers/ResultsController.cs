using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Web.Controllers
{
    public class ResultsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
