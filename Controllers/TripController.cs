using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Migrations;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Numerics;

[Route("api/[controller]")]
[ApiController]
public class TripController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public TripController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpGet("city/{cityId}")]
    public async Task<ActionResult<City>> GetCity(int cityId)
    {
        var city = await _context.Cities.FindAsync(cityId);
        if (city == null)
        {
            return NotFound("City not found");
        }
        return Ok(city);
    }

    [HttpGet("places/{cityId}")]
    public async Task<ActionResult<IEnumerable<TouristPlacesDTO>>> GetTouristPlaces(int cityId)
    {
        var places = await _context.TouristPlaces
            .Where(p => p.CityId == cityId)
            .Select(p => new TouristPlacesDTO
            {
                Name = p.Name,
                Description = p.Description,
                PictureUrl = $"http://safarny.runasp.net/image/{Uri.EscapeDataString(p.PictureUrl.Replace("image/", ""))}",
                CityId = p.CityId,
                Price = p.Price,
                Category = p.Category,
            })
            .ToListAsync();

        return Ok(places);
    }

    [HttpGet("placesHotels/{cityId}")]
    public async Task<ActionResult<CityDTO>> GetPlacesAndHotels(int cityId)
    {
        var places = await _context.TouristPlaces
            .Where(p => p.CityId == cityId)
            .Select(p => new TouristPlacesDTO
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                PictureUrl = !string.IsNullOrEmpty(p.PictureUrl)
                    ? $"http://safarny.runasp.net/image/{Uri.EscapeDataString(p.PictureUrl.Replace("image/", ""))}"
                    : null,
                CityId = p.CityId,
                Price = p.Price,
                Category = p.Category,
            })
            .ToListAsync();

        var hotels = await _context.Hotels
            .Where(h => h.CityId == cityId)
            .Select(h => new HotelDTO
            {
                Id = h.Id,
                Name = h.Name,
                StartPrice = h.StartPrice,
                Rate = h.Rate,
                PictureUrl = h.Hotel_Images.FirstOrDefault() != null && !string.IsNullOrEmpty(h.Hotel_Images.FirstOrDefault().PictureUrl)
                    ? $"http://safarny.runasp.net/image/{Uri.EscapeDataString(h.Hotel_Images.FirstOrDefault().PictureUrl.Replace("image/", ""))}"
                    : null,
                CityId = h.CityId
            })
            .ToListAsync();

        return Ok(new CityDTO
        {
            TouristPlaces = places,
            Hotels = hotels
        });
    }

    [HttpGet("cities")]
    public async Task<ActionResult<IEnumerable<CityDTO>>> GetAllCities()
    {
        try
        {
            var cities = await _context.Cities
                .Select(c => new CityDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                })
                .ToListAsync();

            if (cities == null || !cities.Any())
            {
                return NotFound("No cities found.");
            }

            return Ok(cities);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while fetching cities.");
        }
    }

    /* 
    [HttpPost("selectTrip")]
    public async Task<ActionResult> SelectTrip([FromBody] TripDTO selection)
    {
        // your existing code
    }
    */

    [HttpPost("planWithBudget")]
    // [Authorize]
    public async Task<ActionResult<IEnumerable<HotelDTO>>> PlanTripWithBudget([FromBody] TripDTO budgetRange)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var nights = (budgetRange.EndDate - budgetRange.StartDate).Days;

        var hotels = await _context.Hotels
            .OrderBy(h => h.StartPrice)
            .Select(h => new HotelDTO
            {
                Id = h.Id,
                Name = h.Name,
                StartPrice = h.StartPrice,
                Rate = h.Rate,
                PictureUrl = $"http://safarny.runasp.net/image/{Uri.EscapeDataString(h.Hotel_Images.FirstOrDefault().PictureUrl.Replace("image/", ""))}",
                CityId = h.CityId
            })
            .ToListAsync();

        var touristPlaces = await _context.TouristPlaces
            .OrderBy(p => p.Price)
            .Select(p => new TouristPlacesDTO
            {
                Name = p.Name,
                Description = p.Description,
                Price = p.Price,
                PictureUrl = $"http://safarny.runasp.net/image/{Uri.EscapeDataString(p.PictureUrl.Replace("image/", ""))}",
                CityId = p.CityId,
                Category = p.Category
            })
            .ToListAsync();

        return Ok(new
        {
            Hotels = hotels,
            TouristPlaces = touristPlaces
        });
    }

    [HttpGet("viewTrips/{userId}")]
    public async Task<ActionResult<List<UserTripDTO>>> ViewTrips(string userId)
    {
        try
        {
            var trips = await _context.Trips
                .Where(t => t.UserId == userId)
                .Include(t => t.City)
                .Include(t => t.TripPlaces)
                    .ThenInclude(tp => tp.Place)
                .Include(t => t.Hotel)
                .ToListAsync();

            if (!trips.Any())
            {
                return NotFound("No trips found for this user.");
            }

            var userTrips = trips.Select(t => new UserTripDTO
            {
                TripId = t.Id,
                CityName = t.City.Name,
                TouristPlaces = t.TripPlaces.Select(tp => new TouristPlacesDTO
                {
                    Id = tp.Place.Id,
                    Name = tp.Place.Name,
                    Description = tp.Place.Description,
                    PictureUrl = $"http://safarny.runasp.net/image/{Uri.EscapeDataString(tp.Place.PictureUrl.Replace("image/", ""))}",
                    CityId = tp.Place.CityId,
                    Price = tp.Place.Price,
                    Category = tp.Place.Category,
                }).ToList(),
                Hotel = new HotelDTO
                {
                    Id = t.Hotel.Id,
                    Name = t.Hotel.Name,
                    StartPrice = t.Hotel.StartPrice,
                    Rate = t.Hotel.Rate,
                    PictureUrl = $"http://safarny.runasp.net/image/{Uri.EscapeDataString(t.Hotel.Hotel_Images.FirstOrDefault().PictureUrl.Replace("image/", ""))}",
                    CityId = t.Hotel.CityId
                },
                StartDate = t.StartDate,
                EndDate = t.EndDate
            }).ToList();

            return Ok(userTrips);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while fetching trips.");
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<UserTripDTO>>> SearchTrips(
        [FromQuery] int? cityId,
        [FromQuery] DateTime? startDate,
        [FromQuery] DateTime? endDate,
        [FromQuery] int? hotelId)
    {
        try
        {
            if (!cityId.HasValue && !startDate.HasValue && !endDate.HasValue && !hotelId.HasValue)
            {
                return BadRequest("At least one search parameter is required.");
            }

            var query = _context.Trips.AsQueryable();

            if (cityId.HasValue)
                query = query.Where(t => t.CityId == cityId.Value);
            if (startDate.HasValue)
                query = query.Where(t => t.StartDate >= startDate.Value);
            if (endDate.HasValue)
                query = query.Where(t => t.EndDate <= endDate.Value);
            if (hotelId.HasValue)
                query = query.Where(t => t.HotelId == hotelId.Value);

            var trips = await query
                .Include(t => t.City)
                .Include(t => t.TripPlaces)
                    .ThenInclude(tp => tp.Place)
                .Include(t => t.Hotel)
                .Select(t => new UserTripDTO
                {
                    TripId = t.Id,
                    CityName = t.City.Name,
                    TouristPlaces = t.TripPlaces.Select(tp => new TouristPlacesDTO
                    {
                        Name = tp.Place.Name,
                        Description = tp.Place.Description,
                        PictureUrl = $"http://safarny.runasp.net/City/{Uri.EscapeDataString(tp.Place.PictureUrl.Replace("image/", ""))}",
                        CityId = tp.Place.CityId,
                        Category = tp.Place.Category
                    }).ToList(),
                    Hotel = new HotelDTO
                    {
                        Id = t.Hotel.Id,
                        Name = t.Hotel.Name,
                    },
                    StartDate = t.StartDate,
                    EndDate = t.EndDate
                })
                .AsNoTracking()
                .ToListAsync();

            return Ok(trips);
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while searching for trips.");
        }
    }

    [HttpDelete("reset/{userId}")]
    public async Task<ActionResult> ResetUserTrips(string userId)
    {
        var trips = await _context.Trips
            .Where(t => t.UserId == userId)
            .ToListAsync();

        if (!trips.Any())
        {
            return NotFound("No trips found for this user.");
        }

        _context.Trips.RemoveRange(trips);
        await _context.SaveChangesAsync();

        return Ok("All trips have been reset.");
    }

    [HttpDelete("deleteTrip/{tripId}")]
    public async Task<IActionResult> DeleteTrip(int tripId)
    {
        var trip = await _context.Trips.FindAsync(tripId);

        if (trip == null)
        {
            return NotFound(new { message = "Trip not found!" });
        }

        _context.Trips.Remove(trip);
        await _context.SaveChangesAsync();

        return Ok(new { message = "Trip deleted successfully!" });
    }

    [HttpGet("clearFilters")]
    public async Task<ActionResult<FilterResponseDTO>> ClearFilters()
    {
        try
        {
            var defaultCities = await _context.Cities
                .Take(10)
                .Select(c => new CityDTO
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();

            var popularHotels = await _context.Hotels
                .OrderByDescending(h => h.Rate)
                .Take(5)
                .Select(h => new HotelDTO
                {
                    Id = h.Id,
                    Name = h.Name,
                    StartPrice = h.StartPrice,
                    Rate = h.Rate,
                    PictureUrl = $"http://safarny.runasp.net/image/{Uri.EscapeDataString(h.Hotel_Images.FirstOrDefault().PictureUrl.Replace("image/", ""))}",
                    CityId = h.CityId
                })
                .ToListAsync();

            var popularPlaces = await _context.TouristPlaces
                .OrderByDescending(p => p.Id)
                .Take(5)
                .Select(p => new TouristPlacesDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    PictureUrl = $"http://safarny.runasp.net/image/{Uri.EscapeDataString(p.PictureUrl.Replace("image/", ""))}",
                    CityId = p.CityId,
                    Price = p.Price,
                    Category = p.Category
                })
                .ToListAsync();

            return Ok(new FilterResponseDTO
            {
                Message = "Filters cleared",
                DefaultCities = defaultCities,
                PopularHotels = popularHotels,
                PopularPlaces = popularPlaces
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, "An error occurred while clearing filters.");
        }
    }
}
