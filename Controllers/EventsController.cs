using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api_raceday_part2.Controllers
{//start of namespace

    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {//start of the EventsController class

        private readonly Data.ApplicationDBContext _context;

        public EventsController(Data.ApplicationDBContext context)
        {//start of constructor

            _context = context;

        }//end of constructor

        //actions go here, e.g. Get, Post, Put, Delete

    }//end of the EventsController class

}//end of namespace
