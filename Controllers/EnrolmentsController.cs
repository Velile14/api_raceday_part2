using api_raceday_part2.Data;
using api_raceday_part2.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace api_raceday_part2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrolmentsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public EnrolmentsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Enrolments/my
        // Participants can view their own enrolments.
        [HttpGet("my")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetMyEnrolments()
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.UserID == userId)
                .Include(e => e.Event)
                .Include(e => e.Category)
                .ToListAsync();

            return Ok(enrolments);
        }

        // GET: api/Enrolments/5
        // Participants can view their own enrolment.
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<Enrolment>> GetEnrolment(int id)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            var enrolment = await _context.Enrolments
                .AsNoTracking()
                .Include(e => e.Event)
                .Include(e => e.Category)
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentID == id && e.UserID == userId);

            if (enrolment == null)
            {
                return NotFound();
            }

            return Ok(enrolment);
        }

        // POST: api/Enrolments
        // Participants can enrol themselves in an event.
        [HttpPost]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<Enrolment>> CreateEnrolment(
            [FromBody] Enrolment enrolment)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == enrolment.EventID);

            if (!eventExists)
            {
                return BadRequest("The specified event does not exist.");
            }

            var categoryBelongsToEvent = await _context.Categories
                .AnyAsync(c =>
                    c.CategoryID == enrolment.CategoryID &&
                    c.EventID == enrolment.EventID);

            if (!categoryBelongsToEvent)
            {
                return BadRequest(
                    "The category does not exist or does not belong to this event.");
            }

            var alreadyEnrolled = await _context.Enrolments
                .AnyAsync(e =>
                    e.UserID == userId &&
                    e.EventID == enrolment.EventID);

            if (alreadyEnrolled)
            {
                return Conflict("You are already enrolled in this event.");
            }

            // Never trust a UserID supplied by the client.
            enrolment.UserID = userId;

            _context.Enrolments.Add(enrolment);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetEnrolment),
                new { id = enrolment.EnrolmentID },
                enrolment);
        }

        // DELETE: api/Enrolments/5
        // Participants can cancel their own enrolments.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Participant")]
        public async Task<IActionResult> DeleteEnrolment(int id)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            var enrolment = await _context.Enrolments
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentID == id && e.UserID == userId);

            if (enrolment == null)
            {
                return NotFound();
            }

            var hasResult = await _context.Results
                .AnyAsync(r => r.EnrolmentID == id);

            if (hasResult)
            {
                return Conflict(
                    "This enrolment has a result and cannot be cancelled.");
            }

            _context.Enrolments.Remove(enrolment);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // GET: api/Enrolments/event/5
        // Organisers can view enrolments for their own events.
        [HttpGet("event/{eventId:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<IEnumerable<Enrolment>>> GetEventEnrolments(
            int eventId)
        {
            var userIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!int.TryParse(userIdValue, out int userId))
            {
                return Unauthorized();
            }

            var eventExists = await _context.Events
                .AnyAsync(e =>
                    e.EventID == eventId &&
                    e.OrganizerID == userId);

            if (!eventExists)
            {
                return NotFound(
                    "Event not found or you do not organise this event.");
            }

            var enrolments = await _context.Enrolments
                .AsNoTracking()
                .Where(e => e.EventID == eventId)
                .Include(e => e.User)
                .Include(e => e.Category)
                .ToListAsync();

            return Ok(enrolments);
        }
    }
}
