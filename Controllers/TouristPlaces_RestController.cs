using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Stripe;

namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TouristPlaces_RestController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TouristPlaces_RestController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [HttpPost("add-tourist-place")]
        public async Task<IActionResult> AddTouristPlaces([FromBody] List<TouristPlacesDTO> touristPlacesDto)
        {
            if (touristPlacesDto == null || !touristPlacesDto.Any())
            {
                return BadRequest("Invalid input data.");
            }

            var touristPlaces = touristPlacesDto.Select(tp => new TouristPlaces
            {
                Name = tp.Name,
                Description = tp.Description,
                PictureUrl = tp.PictureUrl,
                CityId = tp.CityId,
                Category = tp.Category
            }).ToList();

            context.TouristPlaces.AddRange(touristPlaces);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Tourist places added successfully" });
        }

        [HttpGet("all-tourist-places/{cityId}")]
        public async Task<IActionResult> GetTouristPlacesByCity(int cityId)
        {
            var places = await context.TouristPlaces
                .Where(p => p.CityId == cityId)
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    // Ensure the picture path does not contain "image/" twice
                    PictureUrl = $"http://safarny.runasp.net/image/{Uri.EscapeDataString(p.PictureUrl.Replace("image/", ""))}",
                    Category = p.Category,
                    Price = (decimal)p.Price,
                    Address = p.Address,
                    Rate = (double)p.Rate,
                    CityId = p.CityId
                })
                .ToListAsync();

            if (!places.Any())
            {
                return NotFound(new { Message = "No tourist places found for this city." });
            }

            return Ok(places);
        }


        [HttpGet("tourist-places-details/{id}")]
        public async Task<IActionResult> GetTouristPlaceDetailsById(int id)
        {
            var place = await context.TouristPlaces
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    Name = p.Name,
                    PictureUrl = $"http://safarny.runasp.net/image/{Uri.EscapeDataString(p.PictureUrl.Replace("image/", ""))}",
                    Description = p.Description,
                    Address = p.Address,
                    Rate = (double)p.Rate,
                    Price = (decimal)p.Price,
                   // PriceDetalis = p.
                })
                .FirstOrDefaultAsync();

            if (place == null)
            {
                return NotFound(new { Message = "Tourist place not found" });
            }

            return Ok(place);
        }


        [HttpGet("all-Restaurants/{cityId}")]
        public async Task<IActionResult> GetRestaurantsByCity(int cityId)
        {
            var Rest = await context.restaurants
                .Where(p => p.CityId == cityId) // Filter by CityId
                .Select(p => new
                {
                    Id = p.Id,
                    Name = p.Name,
                    PictureUrl = $"http://safarny.runasp.net/rest/{Uri.EscapeDataString(p.PictureUrl.Replace("rest/", ""))}",
                    CityId = p.CityId,
                    Type = p.Type,
                    rate = p.Rate

                })
                .ToListAsync();

            if (!Rest.Any())
            {
                return NotFound(new { Message = "No Restaurants found for this city." });
            }

            return Ok(Rest);
        }


         [HttpPost("add-Restaurants")]
          public async Task<IActionResult> AddRestaurants([FromBody] List<RestaurantsDTO> restaurantDTOs)
          {
              if (restaurantDTOs == null || !restaurantDTOs.Any())
              {
                  return BadRequest("Invalid data.");
              }

              var restaurants = restaurantDTOs.Select(dto => new Restaurant
              {
                  Name = dto.Name,
                  PictureUrl = dto.PictureUrl,
                  CityId = dto.CityId,
                  Type = dto.Type,
                  PriceRange = dto.Price_Range,
                  Rate = dto.Rate,
                  DiningOptions = dto.Dining_Options,
                  OpeningHours = dto.Opening_Hours
              }).ToList();

               context.restaurants.AddRange(restaurants);
              await context.SaveChangesAsync();

              return Ok(new { Message = $"{restaurants.Count} restaurant(s) added successfully!" });
          }

        [HttpGet("filter-Restaurants")]
        public async Task<IActionResult> GetRestaurants([FromQuery] string? diningOptions, [FromQuery] string? rate, [FromQuery] string? type, [FromQuery] string? priceRange, [FromQuery] string? openNow)
        {
            var query = context.restaurants.AsQueryable();

            if (!string.IsNullOrWhiteSpace(diningOptions))
            {
                query = query.Where(r => r.DiningOptions.Contains(diningOptions));
            }

            if (!string.IsNullOrWhiteSpace(rate))
            {
                query = query.Where(r => r.Rate == rate);  // Exact rating match
            }

            if (!string.IsNullOrWhiteSpace(type))
            {
                query = query.Where(r => r.Type.Contains(type));
            }

            if (!string.IsNullOrWhiteSpace(priceRange))
            {
                query = query.Where(r => r.PriceRange == priceRange);
            }

            var restaurants = await query.Select(r => new
            {
                r.Id,
                r.Name,
                PictureUrl = $"http://safarny.runasp.net/rest/{Uri.EscapeDataString(r.PictureUrl.Replace("rest/", ""))}",
                r.DiningOptions,
                r.Rate,
                r.Type,
                r.PriceRange,
                r.OpeningHours
            }).ToListAsync();

            return Ok(restaurants);
        }


    }
}
