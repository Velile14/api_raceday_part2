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

        // GET: api/Events
        [HttpGet]
        public IActionResult GetEvents()
        {//start of method

            var events = _context.Events.ToList();
            return Ok(events);

        }//end of method

        // POST: api/Events
        [HttpPost]
        public IActionResult CreateEvent([FromBody] models.Event newEvent)
        {
            _context.Events.Add(newEvent);
            _context.SaveChanges();
            return Ok(newEvent);
            
        }//end of method

    }//end of the EventsController class

}//end of namespace
