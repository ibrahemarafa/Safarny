using APIs_Graduation.Data;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ReviewsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("GetByPackage/{packageId}")]
        public async Task<ActionResult<IEnumerable<Review>>> GetReviewsByPackage(int packageId)
        {
            var reviews = await _context.Reviews
                                        .Where(r => r.PackageId == packageId)
                                        .Include(r => r.Package)
                                        .ToListAsync();

            if (!reviews.Any())
            {
                return NotFound("No reviews found for this package.");
            }

            return Ok(reviews);
        }

        [HttpGet("GetAll")]
        public async Task<ActionResult<IEnumerable<Review>>> GetAllReviews()
        {
            var reviews = await _context.Reviews.Include(r => r.Package).ToListAsync();

            if (!reviews.Any())
            {
                return NotFound("No reviews found.");
            }

            return Ok(reviews);
        }

        [HttpPost("Add")]
        public async Task<ActionResult<Review>> AddReview([FromBody] Review review)
        {
            if (review == null || review.Rating < 1 || review.Rating > 5)
            {
                return BadRequest("Invalid review data.");
            }

            var packageExists = await _context.Packages.AnyAsync(p => p.PackageId == review.PackageId);
            if (!packageExists)
            {
                return BadRequest("Package not found.");
            }

            review.CreatedAt = DateTime.UtcNow;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetReviewsByPackage), new { packageId = review.PackageId }, review);
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound("Review not found.");
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok("Review deleted successfully.");
        }

        [HttpDelete("DeleteAllByPackage/{packageId}")]
        public async Task<IActionResult> DeleteAllReviewsByPackage(int packageId)
        {
            var reviews = await _context.Reviews.Where(r => r.PackageId == packageId).ToListAsync();

            if (!reviews.Any())
            {
                return NotFound("No reviews found for this package.");
            }

            _context.Reviews.RemoveRange(reviews);
            await _context.SaveChangesAsync();

            return Ok("All reviews for this package have been deleted.");
        }
    }
}
