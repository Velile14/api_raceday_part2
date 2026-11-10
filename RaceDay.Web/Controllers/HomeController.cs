using Microsoft.AspNetCore.Mvc;
using RaceDay.Web.Models;
using System.Diagnostics;

namespace RaceDay.Web.Controllers
{//start of namespace
    public class HomeController : Controller
    {//start of HomeController class

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {//start of HomeController constructor
            _logger = logger;
        }//end of HomeController constructor

        public IActionResult Index()
        {//start of Index method
            return View();
        }//end of Index method

        // This method is used to handle errors and display the error view.
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {//start of Error method
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }//end of Error method

    }//end of HomeController class

}//end of namespace
