using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace APIs_Graduation.Controllers
{
    [Route("api/hotels")]
    [ApiController]
    public class HotelsController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public HotelsController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [HttpPost("addhotels")]
        public async Task<IActionResult> SeedHotels([FromBody] List<HotelDTO> hotelDtos)
        {
            var hotels = hotelDtos.Select(h => new Hotel
            {
                Name = h.Name,
                PictureUrl = h.PictureUrl,
                Rate = h.Rate,
                Address = h.Address,
                StartPrice = h.StartPrice,
                Overview = h.Overview,
                CityId = h.CityId
            }).ToList();

            await context.Hotels.AddRangeAsync(hotels);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Hotels seeded successfully", Count = hotels.Count });
        }

        [HttpGet("hotelsOfCity/{cityId}")]
        public async Task<IActionResult> AllHotelsByCityId(int cityId)
        {
            var hotels = await context.Hotels
                .Where(h => h.CityId == cityId)
                .Select(h => new
                {
                    Id = h.Id,
                    Name = h.Name,
                    PictureUrl = $"http://safarny.runasp.net/Hotel/{Uri.EscapeDataString(h.PictureUrl.Replace("Hotel/", ""))}",
                    Rate = h.Rate
                })
                .ToListAsync();

            return Ok(hotels);
        }

        [HttpGet("hotelDetails/{hotelId}")]
        public async Task<IActionResult> HotelDetails(int hotelId)
        {
            var hotel = await context.Hotels
                .Where(h => h.Id == hotelId)
                .Select(h => new
                {
                    Id = h.Id,
                    Name = h.Name,
                    Overview = h.Overview,
                    Address = h.Address,
                    Rate = h.Rate,
                    StartPrice = h.StartPrice,
                    Features = context.Hotel_Features
                        .Where(f => f.HotelId == h.Id)
                        .Select(f => f.feature)
                        .ToList(),
                    Images = context.Hotel_Images
                        .Where(img => img.HotelId == h.Id)
                        .Select(img => new
                        {
                            PictureUrl = $"http://safarny.runasp.net/Hotel/{Uri.EscapeDataString(img.PictureUrl.Replace("Hotel/", ""))}"
                        })
                        .ToList(),
                })
                .FirstOrDefaultAsync();

            if (hotel == null)
                return NotFound("Hotel not found");

            return Ok(hotel);
        }

        [HttpGet("roomsByHotel/{hotelId}")]
        public async Task<IActionResult> GetRoomsByHotel(int hotelId)
        {
            var hotel = await context.Hotels.FindAsync(hotelId);
            if (hotel == null) return NotFound("Hotel not found");

            var rooms = await context.Rooms
                .Where(r => r.HotelId == hotelId && r.IsAvailable)
                .Select(r => new RoomDTO
                {
                    RoomId = r.RoomId,
                    HotelId = r.HotelId,
                    RoomType = r.RoomType,
                    Capacity = r.Capacity,
                    PricePerNight = r.PricePerNight,
                    IsAvailable = r.IsAvailable
                })
                .ToListAsync();

            return Ok(rooms);
        }

        [HttpPost("addRoom")]
        public async Task<IActionResult> AddRoom([FromBody] RoomDTO roomDto)
        {
            var hotel = await context.Hotels.FindAsync(roomDto.HotelId);
            if (hotel == null)
                return NotFound("Hotel not found");

            var room = new Room
            {
                HotelId = roomDto.HotelId,
                RoomType = roomDto.RoomType,
                Capacity = roomDto.Capacity,
                PricePerNight = roomDto.PricePerNight,
                IsAvailable = roomDto.IsAvailable
            };

            context.Rooms.Add(room);
            await context.SaveChangesAsync();

            return Ok(new { Message = "Room added successfully", RoomId = room.RoomId });
        }

        [HttpPost("addFeatures")]
        public async Task<IActionResult> AddHotelFeatures([FromBody] List<HotelFeatureDTO> hotelFeatures)
        {
            if (hotelFeatures == null || hotelFeatures.Count == 0)
                return BadRequest("Invalid data. Please provide hotel features.");

            var newHotelFeatures = hotelFeatures.Select(feature => new Hotel_Feature
            {
                feature = feature.Feature,
                HotelId = feature.HotelId
            }).ToList();

            context.Hotel_Features.AddRange(newHotelFeatures);
            await context.SaveChangesAsync();

            return Ok(new { message = $"{newHotelFeatures.Count} hotel features added successfully." });
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterHotels(
            [FromQuery] string? cityName,
            [FromQuery] int? rate,
            [FromQuery] decimal? startPrice)
        {
            var query = context.Hotels
                .Include(h => h.City)
                .AsQueryable();

            if (!string.IsNullOrEmpty(cityName))
                query = query.Where(h => h.City.Name.ToLower() == cityName.ToLower());

            if (rate.HasValue)
                query = query.Where(h => h.Rate <= rate.Value);

            if (startPrice.HasValue)
                query = query.Where(h => h.StartPrice <= (double)startPrice.Value);

            var filteredHotels = await query.ToListAsync();

            return Ok(filteredHotels);
        }
    }
}
