using Microsoft.AspNetCore.Mvc;

namespace RaceDay.Web.Controllers
{
    public class CategoriesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
