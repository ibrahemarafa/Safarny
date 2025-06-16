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
    public class TripCartController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TripCartController(ApplicationDbContext context)
        {
            _context = context;
        }

     
        [HttpPost("add-places")]
        public async Task<IActionResult> AddMultiplePlacesToCart(AddMultiplePlacesDTO dto)
        {
            var places = await _context.TouristPlaces
                .Where(p => dto.PlaceIds.Contains(p.Id))
                .ToListAsync();

            if (!places.Any())
                return NotFound(new { message = "No places found" });

            var cartItems = places.Select(place => new Cart
            {
                UserId = dto.UserId,
                TouristPlaceId = place.Id,
                CityId = place.CityId,
                StartDate = DateTime.UtcNow
            }).ToList();

            _context.Carts.AddRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Places added to your trip cart!" });
        }


        [HttpGet("view-my-trip")]
        public async Task<IActionResult> ViewMyTrip(string userId)
        {
            var cartItems = await _context.Carts
                .Include(c => c.TouristPlace)
                .Include(c => c.City)
                .Where(c => c.UserId == userId && c.TouristPlaceId != null)
                .ToListAsync();

            if (!cartItems.Any())
                return NotFound(new { message = "No trip data found." });

            var groupedByCity = cartItems.GroupBy(c => c.CityId);

            List<CityTripDTO> trip = new List<CityTripDTO>();

            foreach (var cityGroup in groupedByCity)
            {
                var city = cityGroup.First().City;

                var places = cityGroup.Select(c => new PlaceDTO
                {
                    PlaceId = c.TouristPlace.Id,
                    PlaceName = c.TouristPlace.Name
                }).ToList();

                var hotels = await _context.Hotels
                        .Where(h => h.CityId == city.Id)
                        .OrderByDescending(h => h.Rate) 
                        .Take(3)
                        .Select(h => new HotelShowDTO
                        {
                           HotelId = h.Id,
                           HotelName = h.Name,
                           Rating = h.Rate,
                           PricePerNight = h.StartPrice
                        })
                  .ToListAsync();

                trip.Add(new CityTripDTO
                {
                    CityName = city.Name,
                    Places = places,
                    Hotels = hotels
                });
            }

            return Ok(trip);
        }

        [HttpGet("all-hotels")]
        public async Task<IActionResult> GetAllHotelsForCity(int cityId)
        {
            var hotels = await _context.Hotels
                .Where(h => h.CityId == cityId)
                .OrderByDescending(h => h.Rate)
                .Select(h => new HotelShowDTO
                {
                    HotelId = h.Id,
                    HotelName = h.Name,
                    Rating = h.Rate,
                    PricePerNight = h.StartPrice
                })
                .ToListAsync();

            if (!hotels.Any())
                return NotFound(new { message = "No hotels found for this city." });

            return Ok(hotels);
        }


        [HttpPost("choose-hotels")]
        public async Task<IActionResult> ChooseHotels(List<ChooseHotelDTO> hotelSelections)
        {
            foreach (var selection in hotelSelections)
            {
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == selection.UserId && c.CityId == selection.CityId)
                    .ToListAsync();

                if (!cartItems.Any())
                    return NotFound(new { message = $"No trip data found for city {selection.CityId}." });

                foreach (var item in cartItems)
                {
                    item.HotelId = selection.HotelId;
                }

                await _context.SaveChangesAsync();
            }

            return Ok(new { message = "Hotels selected successfully for all cities!" });
        }


        [HttpGet("trip-days")]
        public async Task<IActionResult> CalculateTripDays(string userId)
        {
            var cartItems = await _context.Carts
                             .Include(c => c.TouristPlace)
                             .Include(c => c.City) 
                             .Where(c => c.UserId == userId && c.TouristPlaceId != null)
                             .ToListAsync();
            if (!cartItems.Any())
                return NotFound(new { message = "No trip data found." });

            var daysPerCity = cartItems
                .GroupBy(c => c.CityId)
                .Select(group => new
                {
                    CityId = group.Key,
                    CityName = group.First().City.Name,
                    NumberOfPlaces = group.Count(),
                    Days = (int)Math.Ceiling(group.Count() / 3.0)
                })
                .OrderBy(x => x.CityName) 
                .ToList();

            return Ok(daysPerCity);
        }

        [HttpPost("analyze-trip")]
        public async Task<IActionResult> AnalyzeTrip(AnalyzeTripRequest request)
        {
            var cartItems = await _context.Carts
                .Include(c => c.City)
                .Include(c => c.TouristPlace)
                .Where(c => c.UserId == request.UserId && c.TouristPlaceId != null)
                .ToListAsync();

            if (!cartItems.Any())
                return NotFound(new { message = "No trip data found." });

            var daysPerCity = cartItems
                .GroupBy(c => c.CityId)
                .Select(group => new
                {
                    CityId = group.Key,
                    CityName = group.First().City.Name,
                    NumberOfPlaces = group.Count(),
                    Days = (int)Math.Ceiling(group.Count() / 3.0)
                })
                .OrderBy(city => city.CityName) 
                .ToList();

            var tripPlan = new List<TripCityPlanDto>();

            DateTime currentStartDate = request.StartDate;

            foreach (var city in daysPerCity)
            {
                var endDate = currentStartDate.AddDays(city.Days - 1);

                tripPlan.Add(new TripCityPlanDto
                {
                    CityName = city.CityName,
                    StartDate = currentStartDate,
                    EndDate = endDate,
                    DaysInCity = city.Days
                });

                currentStartDate = endDate.AddDays(1);
            }

            return Ok(tripPlan);
        }

        [HttpGet("full-trip-summary")]
        public async Task<IActionResult> GetFullTripSummary(string userId, DateTime startDate)
        {
            var cartItems = await _context.Carts
                .Include(c => c.City)
                .Include(c => c.TouristPlace)
                .Include(c => c.Hotel) // still include it
                .Where(c => c.UserId == userId && c.TouristPlaceId != null)
                .ToListAsync(); // removed: && c.HotelId != null

            if (!cartItems.Any())
                return NotFound(new { message = "No trip data found." });

            var groupedByCity = cartItems.GroupBy(c => c.CityId)
                .Select(group => new
                {
                    City = group.First().City,
                    Hotel = group.First().Hotel, // could be null now
                    Places = group.Select(c => c.TouristPlace).ToList(),
                    Days = (int)Math.Ceiling(group.Count() / 3.0)
                })
                .ToList();

            var orderedCityNames = new List<string> { "Alexandria", "Cairo", "Luxor", "Aswan" };

            var sortedCities = groupedByCity
                .OrderBy(g =>
                    orderedCityNames.IndexOf(g.City.Name) >= 0
                    ? orderedCityNames.IndexOf(g.City.Name)
                    : int.MaxValue)
                .ToList();

            var tripSummary = new List<TripSummaryDTO>();

            DateTime currentStartDate = startDate;

            string firstCityAirport = "";
            string lastCityAirport = "";

            foreach (var cityGroup in sortedCities)
            {
                var endDate = currentStartDate.AddDays(cityGroup.Days - 1);

                if (firstCityAirport == "")
                {
                    firstCityAirport = cityGroup.City.Airport ?? "Unknown Airport";
                }

                lastCityAirport = cityGroup.City.Airport ?? "Unknown Airport";

                tripSummary.Add(new TripSummaryDTO
                {
                    CityId = cityGroup.City.Id,
                    CityName = cityGroup.City.Name,
                    StartDate = currentStartDate,
                    EndDate = endDate,
                    DaysInCity = cityGroup.Days,
                    Hotel = cityGroup.Hotel == null ? null : new HotelShowDTO
                    {
                        HotelId = cityGroup.Hotel.Id,
                        HotelName = cityGroup.Hotel.Name,
                        Rating = cityGroup.Hotel.Rate,
                        PricePerNight = cityGroup.Hotel.StartPrice
                    },
                    Places = cityGroup.Places.Select(p => new PlaceDTO
                    {
                        PlaceId = p.Id,
                        PlaceName = p.Name
                    }).ToList(),
                    ArrivalAirport = firstCityAirport,
                    DepartureAirport = lastCityAirport
                });

                currentStartDate = endDate.AddDays(1);
            }

            return Ok(tripSummary);
        }

        [HttpGet("smart-trip-summary")]
        public async Task<IActionResult> GetSmartTripSummary(string userId, DateTime startDate, bool confirm, string? selectedCity = null)
        {
            var cartItems = await _context.Carts
                .Include(c => c.City)
                .Include(c => c.TouristPlace)
                .Include(c => c.Hotel)
                .Where(c => c.UserId == userId && c.TouristPlaceId != null)
                .ToListAsync();

            if (!cartItems.Any())
                return NotFound(new { message = "No trip data found." });

            if (!confirm)
                return Ok(new { message = "OK, Thanks" });

            // جاي يختار مدينة البداية
            var groupedByCity = cartItems
                .GroupBy(c => c.CityId)
                .Select(group => new
                {
                    City = group.First().City,
                    Hotel = group.First().Hotel,
                    Places = group.Select(c => c.TouristPlace).ToList(),
                    Days = (int)Math.Ceiling(group.Count() / 3.0)
                })
                .ToList();

            // لسه ما اختارش مدينة، نرجعله قائمة المدن بس
            if (string.IsNullOrEmpty(selectedCity))
            {
                var cityNames = groupedByCity.Select(g => g.City.Name).Distinct().ToList();
                return Ok(new { AvailableCities = cityNames });
            }

            // ترتيب حسب المدينة المختارة
            var priority = selectedCity.ToLower() switch
            {
                "alexandria" => new List<string> { "Alexandria", "Cairo", "Luxor", "Aswan" },
                "cairo" => new List<string> { "Cairo", "Alexandria", "Luxor", "Aswan" },
                "luxor" => new List<string> { "Luxor", "Aswan", "Cairo", "Alexandria" },
                "aswan" => new List<string> { "Aswan", "Luxor", "Cairo", "Alexandria" },
                _ => new List<string>() // أي مدينة مش من الأربعة
            };

            var sortedCities = groupedByCity
                .OrderBy(g =>
                    priority.Contains(g.City.Name)
                    ? priority.IndexOf(g.City.Name)
                    : int.MaxValue)
                .ToList();

            // بناء ملخص الرحلة
            var tripSummary = new List<TripSummaryDTO>();
            DateTime currentStartDate = startDate;

            string firstCityAirport = "";
            string lastCityAirport = "";

            foreach (var cityGroup in sortedCities)
            {
                var endDate = currentStartDate.AddDays(cityGroup.Days - 1);

                if (firstCityAirport == "")
                {
                    firstCityAirport = cityGroup.City.Airport ?? "Unknown Airport";
                }

                lastCityAirport = cityGroup.City.Airport ?? "Unknown Airport";

                tripSummary.Add(new TripSummaryDTO
                {
                    CityId = cityGroup.City.Id,
                    CityName = cityGroup.City.Name,
                    StartDate = currentStartDate,
                    EndDate = endDate,
                    DaysInCity = cityGroup.Days,
                    Hotel = cityGroup.Hotel == null ? null : new HotelShowDTO
                    {
                        HotelId = cityGroup.Hotel.Id,
                        HotelName = cityGroup.Hotel.Name,
                        Rating = cityGroup.Hotel.Rate,
                        PricePerNight = cityGroup.Hotel.StartPrice
                    },
                    Places = cityGroup.Places.Select(p => new PlaceDTO
                    {
                        PlaceId = p.Id,
                        PlaceName = p.Name
                    }).ToList(),
                    ArrivalAirport = firstCityAirport,
                    DepartureAirport = lastCityAirport
                });

                currentStartDate = endDate.AddDays(1);
            }

            return Ok(tripSummary);
        }


        [HttpGet("calculate-trip-price")]
        public async Task<IActionResult> CalculateTripPrice(string userId, int numberOfPeople, bool wantHotelBooking)
        {
            var cartItems = await _context.Carts
                .Include(c => c.City)
                .Include(c => c.Hotel)
                .Include(c => c.TouristPlace)
                .Where(c => c.UserId == userId && c.TouristPlaceId != null)
                .ToListAsync();

            if (!cartItems.Any())
                return NotFound(new { message = "No trip data found." });

            var groupedByCity = cartItems.GroupBy(c => c.CityId)
                .Select(group => new
                {
                    City = group.First().City,
                    Hotel = group.First().Hotel,
                    NumberOfPlaces = group.Count()
                })
                .ToList();

            double costPerPerson = 0;

            foreach (var cityGroup in groupedByCity)
            {
                int days = (int)Math.Ceiling(cityGroup.NumberOfPlaces / 3.0);
                double cityBasePrice = (double)cityGroup.City.Price;

                double hotelTotalPrice = 0;
                if (wantHotelBooking && cityGroup.Hotel != null)
                {
                    hotelTotalPrice = cityGroup.Hotel.StartPrice * days;
                }

                double cityTotalPrice = cityBasePrice + hotelTotalPrice;

                costPerPerson += cityTotalPrice;
            }

            double totalCost = costPerPerson * numberOfPeople;

            return Ok(new
            {
                TotalTripPrice = Math.Round(totalCost, 2),
                PricePerPerson = Math.Round(costPerPerson, 2),
                HotelIncluded = wantHotelBooking
            });
        }

        [HttpPost("book-trip")]
        public async Task<IActionResult> BookTrip(BookingCustomTripRequestDTO bookingRequest)
        {
            var cartItems = await _context.Carts
                .Include(c => c.City)
                .Include(c => c.Hotel)
                .Include(c => c.TouristPlace)
                .Where(c => c.UserId == bookingRequest.UserId && c.TouristPlaceId != null)
                .ToListAsync();

            if (!cartItems.Any())
                return NotFound(new { message = "No trip data found to book." });

            double totalTripPrice = 0;

            var groupedByCity = cartItems.GroupBy(c => c.CityId)
                .Select(group => new
                {
                    City = group.First().City,
                    Hotel = group.First().Hotel, // ممكن تكون null
                    NumberOfPlaces = group.Count()
                })
                .ToList();

            foreach (var cityGroup in groupedByCity)
            {
                int days = (int)Math.Ceiling(cityGroup.NumberOfPlaces / 3.0);

                double cityBasePrice = (double)cityGroup.City.Price;
                double hotelPricePerNight = cityGroup.Hotel != null ? cityGroup.Hotel.StartPrice : 0; // لو مفيش فندق، السعر صفر

                double hotelTotalPrice = hotelPricePerNight * days;
                double cityTotalPrice = cityBasePrice + hotelTotalPrice;

                totalTripPrice += cityTotalPrice;
            }

            var booking = new BookingCustomTrip
            {
                UserId = bookingRequest.UserId,
                BookingDate = DateTime.UtcNow,
                FullName = bookingRequest.FullName,
                Email = bookingRequest.Email,
                PhoneNumber = bookingRequest.PhoneNumber,
                TotalPrice = Math.Round(totalTripPrice, 2)
            };

            _context.BookingCustomTrips.Add(booking);
            await _context.SaveChangesAsync();

            foreach (var item in cartItems)
            {
                var bookingDetail = new BookingCustomTripDetail
                {
                    BookingId = booking.Id,
                    CityId = item.CityId,
                    HotelId = item.HotelId, 
                    CheckInDate = DateTime.UtcNow,
                    CheckOutDate = DateTime.UtcNow.AddDays(1)
                };

                _context.BookingCustomTripDetails.Add(bookingDetail);
            }

            await _context.SaveChangesAsync();

           
            _context.Carts.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Your trip has been booked!",
                totalPrice = booking.TotalPrice,
                bookingId = booking.Id
            });
        }


        [HttpGet("get-bookings-by-user/{userId}")]
        public async Task<IActionResult> GetBookingsByUser(string userId)
        {
            var bookings = await _context.BookingCustomTrips
                .Include(b => b.BookingCustomTripDetail)
                    .ThenInclude(bd => bd.City)   // جلب بيانات المدينة
                .Include(b => b.BookingCustomTripDetail)
                    .ThenInclude(bd => bd.Hotel)  // جلب بيانات الفندق
                .Where(b => b.UserId == userId) // فلترة بالحجز حسب اليوزر
                .ToListAsync();

            if (!bookings.Any())
                return NotFound(new { message = "No bookings found for this user." });

            var result = bookings.Select(booking => new
            {
                booking.Id,
                booking.UserId,
                booking.FullName,
                booking.Email,
                booking.PhoneNumber,
                booking.BookingDate,
                booking.TotalPrice,
                Details = booking.BookingCustomTripDetail.Select(detail => new
                {
                    detail.CityId,
                    CityName = detail.City != null ? detail.City.Name : null,
                    detail.HotelId,
                    HotelName = detail.Hotel != null ? detail.Hotel.Name : null,
                    detail.CheckInDate,
                    detail.CheckOutDate
                }).ToList()
            }).ToList();

            return Ok(result);
        }


        [HttpGet("get-all-bookings")]
        public async Task<IActionResult> GetBookings()
        {
            var bookings = await _context.BookingCustomTrips
                .Include(b => b.BookingCustomTripDetail)
                    .ThenInclude(bd => bd.City)   // جلب بيانات المدينة
                .Include(b => b.BookingCustomTripDetail)
                    .ThenInclude(bd => bd.Hotel)  // جلب بيانات الفندق
                .ToListAsync();

            if (!bookings.Any())
                return NotFound(new { message = "No bookings found." });

            var result = bookings.Select(booking => new
            {
                booking.Id,
                booking.UserId,
                booking.FullName,
                booking.Email,
                booking.PhoneNumber,
                booking.BookingDate,
                booking.TotalPrice,
                Details = booking.BookingCustomTripDetail.Select(detail => new
                {
                    detail.CityId,
                    CityName = detail.City != null ? detail.City.Name : null,
                    detail.HotelId,
                    HotelName = detail.Hotel != null ? detail.Hotel.Name : null,
                    detail.CheckInDate,
                    detail.CheckOutDate
                }).ToList()
            }).ToList();

            return Ok(result);
        }



    }
}
