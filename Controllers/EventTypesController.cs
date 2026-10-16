
using api_raceday_part2.Data;
using api_raceday_part2.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_raceday_part2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventTypesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EventTypesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/EventTypes
        // Public: view all event types.
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EventType>>> GetEventTypes()
        {
            var eventTypes = await _context.EventTypes
                .AsNoTracking()
                .OrderBy(e => e.EventTypeID)
                .ToListAsync();

            return Ok(eventTypes);
        }

        // GET: api/EventTypes/1
        // Public: view one event type.
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<EventType>> GetEventType(int id)
        {
            var eventType = await _context.EventTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EventTypeID == id);

            if (eventType == null)
            {
                return NotFound();
            }

            return Ok(eventType);
        }

        // POST: api/EventTypes
        // Organisers can create event types.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<EventType>> CreateEventType(
            [FromBody] EventType eventType)
        {
            var typeName = eventType.TypeName?.Trim();

            if (string.IsNullOrWhiteSpace(typeName))
            {
                return BadRequest("Event type name is required.");
            }

            var alreadyExists = await _context.EventTypes
                .AnyAsync(e => e.TypeName == typeName);

            if (alreadyExists)
            {
                return Conflict("This event type already exists.");
            }

            eventType.TypeName = typeName;

            _context.EventTypes.Add(eventType);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEventType),
                new { id = eventType.EventTypeID },
                eventType);
        }

        // PUT: api/EventTypes/1
        // Organisers can update event types.
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateEventType(
            int id, [FromBody] EventType updatedEventType)
        {
            if (id != updatedEventType.EventTypeID)
            {
                return BadRequest(
                    "The event type ID does not match.");
            }

            var typeName = updatedEventType.TypeName?.Trim();

            if (string.IsNullOrWhiteSpace(typeName))
            {
                return BadRequest("Event type name is required.");
            }

            var eventType = await _context.EventTypes
                .FirstOrDefaultAsync(e => e.EventTypeID == id);

            if (eventType == null)
            {
                return NotFound();
            }

            var duplicateName = await _context.EventTypes
                .AnyAsync(e =>
                    e.EventTypeID != id &&
                    e.TypeName == typeName);

            if (duplicateName)
            {
                return Conflict("This event type already exists.");
            }

            eventType.TypeName = typeName;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/EventTypes/1
        // Organisers can delete unused event types.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteEventType(int id)
        {
            var eventType = await _context.EventTypes
                .FirstOrDefaultAsync(e => e.EventTypeID == id);

            if (eventType == null)
            {
                return NotFound();
            }

            var isInUse = await _context.Events
                .AnyAsync(e => e.EventTypeID == id);

            if (isInUse)
            {
                return Conflict(
                    "This event type is assigned to an event and cannot be deleted.");
            }

            _context.EventTypes.Remove(eventType);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
