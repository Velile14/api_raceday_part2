using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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
        [Authorize(Roles = "Organiser")]
        [HttpPost]
        public IActionResult CreateEvent([FromBody] models.Event newEvent)
        {
            // Get the ID of the logged-in user
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            // Link the event to the logged-in Organiser
            newEvent.OrganizerID = userId;

            // Keep the existing UserID property valid for now
            newEvent.UserID = userId;

            _context.Events.Add(newEvent);
            _context.SaveChanges();

            return Ok(newEvent);

        }//end of method

        // PUT: api/Events/{id}
        [Authorize(Roles = "Organiser")]
        [HttpPut("{id}")]
        public IActionResult UpdateEvent(int id, [FromBody] models.Event updatedEvent)
        {
            // Find the existing event
            var existingEvent = _context.Events.Find(id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            // Get the logged-in user's ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            // Check that this Organiser owns the event
            if (existingEvent.OrganizerID != userId)
            {
                return Forbid();
            }

            // Update the event details
            existingEvent.EventName = updatedEvent.EventName;
            existingEvent.Description = updatedEvent.Description;
            existingEvent.EventDate = updatedEvent.EventDate;
            existingEvent.Location = updatedEvent.Location;
            existingEvent.Distance = updatedEvent.Distance;
            existingEvent.EventTypeID = updatedEvent.EventTypeID;

            _context.SaveChanges();

            return Ok(existingEvent);

        }//end of method

        // DELETE: api/Events/{id}
        [Authorize(Roles = "Organiser")]
        [HttpDelete("{id}")]
        public IActionResult DeleteEvent(int id)
        {
            // Find the event
            var existingEvent = _context.Events.Find(id);

            if (existingEvent == null)
            {
                return NotFound();
            }

            // Get the logged-in user's ID
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null ||
                !int.TryParse(userIdClaim.Value, out int userId))
            {
                return Unauthorized();
            }

            // Check that this Organiser owns the event
            if (existingEvent.OrganizerID != userId)
            {
                return Forbid();
            }

            // Delete the event
            _context.Events.Remove(existingEvent);
            _context.SaveChanges();

            return Ok(new { message = "Event deleted successfully." });
        }//end of method

    }//end of the EventsController class

}//end of namespace
