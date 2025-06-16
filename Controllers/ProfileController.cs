using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIs_Graduation.Models;

namespace APIs_Graduation.Controllers
{



    [Route("api/profile")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("user-profile/{userId}")]
        public async Task<IActionResult> GetUserProfile(string userId)
        {
            // 1. Get User Basic Information
            var user = await _context.Users
                .Where(u => u.Id == userId)
                .Select(u => new
                {
                    u.Id,
                    u.UserName,
                    u.Email,
                    u.PhoneNumber,
                    u.FirstName,
                    u.LastName,
                   
                })
                .FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound("User not found");
            }

            // 2. Get Hotel Bookings
            var hotelBookings = await _context.HotelBookings
                .Include(b => b.Hotel)
                .Include(b => b.Room)
                .Where(b => b.Username == userId.ToString())
                .Select(b => new
                {
                    Type = "Hotel",
                    b.BookingId,
                    HotelName = b.Hotel.Name,
                    HotelImage = $"http://safarny.runasp.net/Hotel/{Uri.EscapeDataString(b.Hotel.PictureUrl.Replace("Hotel/", ""))}",
                    RoomType = b.Room.RoomType,
                    b.CheckInDate,
                    b.CheckOutDate,
                    b.Status,
                    b.TotalPrice,
                    
                })
                .OrderByDescending(b => b.Status)
                .ToListAsync();

            // 3. Get Package Bookings
            var packageBookings = await _context.PackageBookings
                .Include(p => p.Package)
                .Where(p => p.Username == userId.ToString())
                .Select(p => new
                {
                    Type = "Package",
                    p.BookingId,
                    PackageName = p.Package.Name,
                    PackageImage = $"http://safarny.runasp.net/Package/{Uri.EscapeDataString(p.Package.ImageUrl.Replace("Package/", ""))}",
                    p.TripDate,
                    p.BookingDate,
                    p.Status,
                    p.TotalPrice,
                    Duration = p.Package.Duration,
                    Description = p.Package.Description
                })
                .OrderByDescending(p => p.BookingDate)
                .ToListAsync();

            // 4. Get Custom Trip Bookings (if needed)
            var customTrips = await _context.BookingCustomTrips
                .Include(b => b.BookingCustomTripDetail)
                    .ThenInclude(d => d.City)
                .Include(b => b.BookingCustomTripDetail)
                    .ThenInclude(d => d.Hotel)
                .Where(b => b.UserId == userId)
                .Select(b => new
                {
                    Type = "CustomTrip",
                    b.Id,
                    b.FullName,
                    b.Email,
                    b.PhoneNumber,
                    b.BookingDate,
                    b.TotalPrice,
                    Status = "Confirmed", // or whatever status field you have
                    Details = b.BookingCustomTripDetail.Select(d => new
                    {
                        CityName = d.City.Name,
                        HotelName = d.Hotel.Name,
                        d.CheckInDate,
                        d.CheckOutDate
                    })
                })
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            // 5. Combine results
            // 5. Combine results
            var result = new
            {
                User = user,
                Bookings = new
                {
                    Hotels = hotelBookings,
                    Packages = packageBookings,
                    CustomTrips = customTrips
                },
                // 5. Calculate Stats
                stats = new
                {
                   

                    UpcomingTrips =
                        hotelBookings.Where(h => h.CheckInDate > DateTime.Now).Count() +
                        packageBookings.Where(p => p.TripDate > DateTime.Now).Count() +
                        customTrips.Where(c => c.Details.Any(d => d.CheckInDate > DateTime.Now)).Count(),

                    PastTrips =
                        hotelBookings.Where(h => h.CheckOutDate < DateTime.Now).Count() +
                        packageBookings.Where(p => p.TripDate < DateTime.Now).Count() +
                        customTrips.Where(c => c.Details.Any(d => d.CheckOutDate < DateTime.Now)).Count()
                }
            };

            return Ok(result);
        }
    }


}
     