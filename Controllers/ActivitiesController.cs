using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActivitiesController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public ActivitiesController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [HttpGet("Activities/{touristPlaceId}")]
        public async Task<IActionResult> GetActivitiesByTouristPlaceId(int touristPlaceId)
        {
            var activities = await context.Activities
                .Where(a => a.TouristPlacesId == touristPlaceId)
                .Select(a => new ActivitiesDTO
                {
                    Id = a.Id,
                    Name = a.Name,
                    PictureUrl = $"http://safarny.runasp.net/Activity/{Uri.EscapeDataString(a.PictureUrl.Replace("Activity/", ""))}"
                })
                .ToListAsync();

            if (activities == null || activities.Count == 0)
            {
                return NotFound($"No activities found for TouristPlaceId {touristPlaceId}");
            }

            return Ok(activities);
        }


        [HttpPost("Add-Activities")]
        public async Task<IActionResult> SeedActivities([FromBody] List<ActivitiesDTO> activitiesDto)
        {
            if (activitiesDto == null || !activitiesDto.Any())
            {
                return BadRequest("No activity data provided.");
            }

            var activities = new List<Activity>();

            foreach (var dto in activitiesDto)
            {
                // Check if the provided TouristPlaceId exists in the database
                bool touristPlaceExists = await context.TouristPlaces.AnyAsync(tp => tp.Id == dto.TouristPlaceId);
                if (!touristPlaceExists)
                {
                    return BadRequest($"TouristPlaceId {dto.TouristPlaceId} does not exist.");
                }

                // Create the activity object with TripId set to null
                activities.Add(new Activity
                {
                    Name = dto.Name,
                    PictureUrl = dto.PictureUrl,
                    TouristPlacesId = dto.TouristPlaceId,
                    TripId = null // TripId is optional and set to null by default
                });
            }

            // Save all activities to the database
            await context.Activities.AddRangeAsync(activities);
            await context.SaveChangesAsync();

            return Ok(new { Message = $"{activities.Count} activity(ies) added successfully!" });
        }


        [HttpGet("GetTouristPlaceDetails/{touristPlaceId}")]
        public async Task<IActionResult> GetTouristPlaceDetails(int touristPlaceId)
        {
            var touristPlace = await context.TouristPlaces
                .Where(tp => tp.Id == touristPlaceId)
                .Select(tp => new
                {
                    tp.Id,
                    tp.Name,
                    tp.CityId
                })
                .FirstOrDefaultAsync();

            if (touristPlace == null)
            {
                return NotFound($"Tourist Place with ID {touristPlaceId} not found.");
            }

            var topRestaurants = await context.restaurants
                .Where(r => r.CityId == touristPlace.CityId) 
                .Take(3) 
                .Select(r => new
                {
                    r.Id,
                    r.Name,
                    pictureUrl = $"http://safarny.runasp.net/rest/{Uri.EscapeDataString(r.PictureUrl.Replace("rest/", ""))}"
                })
                .ToListAsync();

            return Ok(new
            {
                TouristPlace = touristPlace,
                TopRestaurants = topRestaurants
            });
        }

    }
}
