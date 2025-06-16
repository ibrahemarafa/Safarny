using APIs_Graduation.Data;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RecommendationController(ApplicationDbContext _context) : ControllerBase
    {
        private readonly ApplicationDbContext context = _context;

        [HttpPost("TrackUserInteraction")]
        public async Task<IActionResult> TrackUserInteraction(string username, int touristPlaceId)
        {
            var interaction = new UserInteraction
            {
                Username = username,
                TouristPlaceId = touristPlaceId,
                Timestamp = DateTime.UtcNow
            };

            _context.UserInteraction.Add(interaction);
            await _context.SaveChangesAsync();

            return Ok("Interaction tracked successfully.");
        }


        [HttpGet("GetReorderedTouristPlaces")]
        public async Task<IActionResult> GetReorderedTouristPlaces(string username, int cityId)
        {
            // Step 1: Get user's preferred tourism type
            var userPreference = await _context.UserPreferences
                .Where(up => up.Username == username)
                .Select(up => up.CategoryPreference)
                .FirstOrDefaultAsync();

            if (userPreference == null)
            {
                return NotFound("User preference not found.");
            }

            // Step 2: Get last two interactions
            var lastTwoInteractions = await _context.UserInteraction
                .Where(ui => ui.Username == username)
                .OrderByDescending(ui => ui.Timestamp)
                .Take(2)
                .Select(ui => ui.TouristPlaceId)
                .ToListAsync();

            // Step 3: Get all tourist places in the city
            var places = await _context.TouristPlaces
                .Where(tp => tp.CityId == cityId)
                .ToListAsync();

            // Step 4: Separate places based on criteria
            var preferredPlaces = places.Where(tp => tp.Category.Equals( userPreference)).ToList();
            var interactedPlaces = places.Where(tp => lastTwoInteractions.Contains(tp.Id)).ToList();
            var remainingPlaces = places.Where(tp => !tp.Category.Equals( userPreference ) && !lastTwoInteractions.Contains(tp.Id)).ToList();

            // Step 5: Combine in the required order
            var reorderedPlaces = preferredPlaces
                .Union(interactedPlaces)
                .Union(remainingPlaces)
                .ToList();

            return Ok(reorderedPlaces);
        }

    }
}
