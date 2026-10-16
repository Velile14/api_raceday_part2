
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
    public class ResultsController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public ResultsController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Results/my
        // Participants can view their own results.
        [HttpGet("my")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<IEnumerable<Result>>> GetMyResults()
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int userId))
            {
                return Unauthorized();
            }

            var results = await _context.Results
                .AsNoTracking()
                .Where(r => r.Enrolment.UserID == userId)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Event)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Category)
                .ToListAsync();

            return Ok(results);
        }

        // GET: api/Results/5
        // Participants can view a result belonging to them.
        [HttpGet("{id:int}")]
        [Authorize(Roles = "Participant")]
        public async Task<ActionResult<Result>> GetResult(int id)
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int userId))
            {
                return Unauthorized();
            }

            var result = await _context.Results
                .AsNoTracking()
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Event)
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Category)
                .FirstOrDefaultAsync(r =>
                    r.ResultID == id &&
                    r.Enrolment.UserID == userId);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        // POST: api/Results
        // Organisers can record results for their own events.
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Result>> CreateResult(
            [FromBody] Result result)
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int organiserId))
            {
                return Unauthorized();
            }

            if (result.FinishPosition <= 0)
            {
                return BadRequest(
                    "Finish position must be greater than zero.");
            }

            if (result.FinishTime < TimeSpan.Zero)
            {
                return BadRequest(
                    "Finish time cannot be negative.");
            }

            var enrolment = await _context.Enrolments
                .Include(e => e.Event)
                .FirstOrDefaultAsync(e =>
                    e.EnrolmentID == result.EnrolmentID);

            if (enrolment == null)
            {
                return BadRequest(
                    "The specified enrolment does not exist.");
            }

            if (enrolment.Event.OrganizerID != organiserId)
            {
                return Forbid();
            }

            var resultExists = await _context.Results
                .AnyAsync(r =>
                    r.EnrolmentID == result.EnrolmentID);

            if (resultExists)
            {
                return Conflict(
                    "A result already exists for this enrolment.");
            }

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetResult),
                new { id = result.ResultID },
                result);
        }

        // PUT: api/Results/5
        // Organisers can update results for their own events.
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateResult(
            int id, [FromBody] Result updatedResult)
        {
            if (id != updatedResult.ResultID)
            {
                return BadRequest(
                    "The result ID does not match.");
            }

            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int organiserId))
            {
                return Unauthorized();
            }

            if (updatedResult.FinishPosition <= 0)
            {
                return BadRequest(
                    "Finish position must be greater than zero.");
            }

            if (updatedResult.FinishTime < TimeSpan.Zero)
            {
                return BadRequest(
                    "Finish time cannot be negative.");
            }

            var existingResult = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (existingResult == null)
            {
                return NotFound();
            }

            if (existingResult.Enrolment.Event.OrganizerID != organiserId)
            {
                return Forbid();
            }

            // Keep the existing enrolment association unchanged.
            existingResult.FinishTime = updatedResult.FinishTime;
            existingResult.FinishPosition = updatedResult.FinishPosition;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Results/5
        // Organisers can delete results for their own events.
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteResult(int id)
        {
            if (!int.TryParse(
                User.FindFirstValue(ClaimTypes.NameIdentifier),
                out int organiserId))
            {
                return Unauthorized();
            }

            var result = await _context.Results
                .Include(r => r.Enrolment)
                    .ThenInclude(e => e.Event)
                .FirstOrDefaultAsync(r => r.ResultID == id);

            if (result == null)
            {
                return NotFound();
            }

            if (result.Enrolment.Event.OrganizerID != organiserId)
            {
                return Forbid();
            }

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
