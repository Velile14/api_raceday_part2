using api_raceday_part2.Data;
using api_raceday_part2.models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace api_raceday_part2.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ApplicationDBContext _context;

        public CategoriesController(ApplicationDBContext context)
        {
            _context = context;
        }

        // GET: api/Categories
        // Public: view all categories
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories
                .AsNoTracking()
                .ToListAsync();
        }

        // GET: api/Categories/5
        // Public: view one category
        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // POST: api/Categories
        // Organisers can create categories
        [HttpPost]
        [Authorize(Roles = "Organiser")]
        public async Task<ActionResult<Category>> CreateCategory(
            Category category)
        {
            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == category.EventID);

            if (!eventExists)
            {
                return BadRequest("The specified event does not exist.");
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetCategory),
                new { id = category.CategoryID },
                category);
        }

        // PUT: api/Categories/5
        // Organisers can update categories
        [HttpPut("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> UpdateCategory(
            int id, Category category)
        {
            if (id != category.CategoryID)
            {
                return BadRequest("The category ID does not match.");
            }

            var existingCategory =
                await _context.Categories.FindAsync(id);

            if (existingCategory == null)
            {
                return NotFound();
            }

            var eventExists = await _context.Events
                .AnyAsync(e => e.EventID == category.EventID);

            if (!eventExists)
            {
                return BadRequest("The specified event does not exist.");
            }

            existingCategory.EventID = category.EventID;
            existingCategory.CategoryName = category.CategoryName;
            existingCategory.MinimumAge = category.MinimumAge;
            existingCategory.MaximumAge = category.MaximumAge;
            existingCategory.Distance = category.Distance;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/Categories/5
        // Organisers can delete categories
        [HttpDelete("{id:int}")]
        [Authorize(Roles = "Organiser")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            var hasEnrolments = await _context.Enrolments
                .AnyAsync(e => e.CategoryID == id);

            if (hasEnrolments)
            {
                return Conflict(
                    "This category has enrolments and cannot be deleted.");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
