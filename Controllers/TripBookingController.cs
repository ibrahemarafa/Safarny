using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Migrations;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace APIs_Graduation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TripBookingController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<TripBookingController> _logger;
        


        public TripBookingController(ApplicationDbContext context, ILogger<TripBookingController> logger)
        {
            _context = context;
            _logger = logger;
            _logger = logger;
            
        }
       
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request.");

                // Ensure null values don't convert to double
                int? hotelId = request.HotelId.HasValue ? Convert.ToInt32(request.HotelId) : (int?)null;
                int? touristPlaceId = request.TouristPlaceId.HasValue ? Convert.ToInt32(request.TouristPlaceId) : (int?)null;

                var cartItem = new Cart
                {
                    UserId = request.UserId,
                    TouristPlaceId = touristPlaceId,
                    HotelId = hotelId,
                    StartDate = request.StartDate,
                    //EndDate = request.EndDate
                };

                _context.Carts.Add(cartItem);
                await _context.SaveChangesAsync();

                return Ok(cartItem);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal Server Error: {ex.Message}");
            }
        }

       

       /* [HttpGet("cart/{userId}")]
        public async Task<ActionResult<List<UserTripDTO>>> GetCartItems(string userId)
        {
            try
            {
                if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out _))
                {
                    return BadRequest("Invalid user ID format");
                }

                // Load related data in separate queries to avoid complex joins
                var cartItems = await _context.Carts
    .Where(c => c.UserId == userId)
    .Include(c => c.TouristPlace)
        .ThenInclude(tp => tp.City)
    .Include(c => c.Hotel)
        .ThenInclude(h => h.City)
    .Include(c => c.Hotel)
        .ThenInclude(h => h.Hotel_Images)
    .AsNoTracking()
    .ToListAsync();

                if (!cartItems.Any())
                {
                    return NotFound("No items found in cart");
                }

                // Explicitly load related data
                foreach (var cart in cartItems)
                {
                    if (cart.TouristPlaceId.HasValue)
                    {
                        await _context.Entry(cart)
                            .Reference(c => c.TouristPlace)
                            .Query()
                            .Include(tp => tp.City)
                            .LoadAsync();
                    }

                    if (cart.HotelId.HasValue)
                    {
                        await _context.Entry(cart)
                            .Reference(c => c.Hotel)
                            .Query()
                            .Include(h => h.City)
                            .Include(h => h.Hotel_Images.Take(1))
                            .LoadAsync();
                    }
                }

                var userTrips = cartItems.Select(c => new UserTripDTO
                {
                    TripId = (int)c.Id,
                    CityName = c.TouristPlace?.City?.Name ?? c.Hotel?.City?.Name ?? "Unknown",
                    TouristPlaces = c.TouristPlace != null ? [new TouristPlacesDTO
                    {
                        Id = (int)c.TouristPlace.Id,
                        Name = c.TouristPlace.Name ?? "",
                        PictureUrl = c.TouristPlace.PictureUrl != null ?
                            $"https://safarny.runasp.net/image/{Uri.EscapeDataString(c.TouristPlace.PictureUrl)}" : null,
                        CityId = c.TouristPlace.CityId,
                        Price = c.TouristPlace.Price,
                        Category = c.TouristPlace.Category,
                        Rate = c.TouristPlace?.Rate ?? 0

                    }] : [],
                    Hotel = c.Hotel != null ? new HotelDTO
                    {
                        Id = c.Hotel.Id,
                        Name = c.Hotel.Name ?? "",
                        StartPrice = c.Hotel.StartPrice,
                        PictureUrl = c.Hotel.Hotel_Images.FirstOrDefault()?.PictureUrl != null ?
                            $"https://safarny.runasp.net/image/{Uri.EscapeDataString(c.Hotel.Hotel_Images.First().PictureUrl)}" : null,
                        CityId = c.Hotel.CityId
                    } : null,
                    StartDate = c.StartDate,
                   // EndDate = c.EndDate
                }).ToList();

                return Ok(userTrips);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetCartItems for user {UserId}", userId);
                return StatusCode(500, new
                {
                    Message = "Operation failed",
                    Detailed = ex.Message // Only for development!
                });
            }
        }

        // Only keep one FormatImageUrl method
        [HttpPost("finalize-trip")]
        public async Task<ActionResult> FinalizeTrip([FromBody] FinalizeTripRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                
                var cartItems = await _context.Carts
                    .Where(c => c.UserId == request.UserId)
                    .ToListAsync();

                if (!cartItems.Any())
                {
                    return NotFound("No items found in the cart.");
                }


                var trip = new Trip
                {
                    UserId = request.UserId,
                    CityId = request.CityId,
                    HotelId = (int)request.HotelId,
                    StartDate = request.StartDate,
                    EndDate = request.EndDate,
                    StarRating = (int)request.Rate,
                    Description = "Generated description", 
                    Destination = "El Ain Sokhna",         
                    Name = "Trip to El Ain Sokhna"         
                };



                _context.Trips.Add(trip);
                await _context.SaveChangesAsync();

                // Add tourist places to the trip
                foreach (var cartItem in cartItems)
                {
                    if (cartItem.TouristPlaceId.HasValue)
                    {
                        _context.Trip_Places.Add(new Trip_Place
                        {
                            TripId = trip.Id,
                            PlaceId =(int) cartItem.TouristPlaceId.Value
                        });
                    }
                }

                await _context.SaveChangesAsync();

               
                _context.Carts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Trip finalized successfully.", tripId = trip.Id });
            }
            catch (Exception ex)
            {
                 _logger.LogError(ex, "An error occurred while finalizing the trip.");
                return StatusCode(500, $"An error occurred while finalizing the trip: {ex.Message}");

            }
        }


       
        */

        [HttpDelete("DeleteCartItem/{id}")]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            var cartItem = await _context.Carts.FindAsync(id);
            if (cartItem == null)
            {
                return NotFound();
            }

            _context.Carts.Remove(cartItem);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Cart item deleted successfully." });
        }
        [HttpDelete("cart")]
        public async Task<IActionResult> DeleteCartItem(
    [FromQuery] string userId,
    [FromQuery] int? cartId = null,
    [FromQuery] int? touristPlaceId = null,
    [FromQuery] int? hotelId = null)
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    return BadRequest("User ID is required.");
                }

                IQueryable<Cart> query = _context.Carts.Where(c => c.UserId == userId);

                if (cartId.HasValue)
                {
                    query = query.Where(c => c.Id == cartId.Value);
                }

                var cartItem = await query.FirstOrDefaultAsync();

                if (cartItem == null)
                {
                    return NotFound("Cart item not found");
                }

                // Case 1: Remove entire cart item
                if (cartId.HasValue && !touristPlaceId.HasValue && !hotelId.HasValue)
                {
                    _context.Carts.Remove(cartItem);
                }
                else
                {
                    // Case 2: Nullify fields instead of deleting the row
                    if (touristPlaceId.HasValue && cartItem.TouristPlaceId == touristPlaceId)
                    {
                        cartItem.TouristPlaceId = null;
                    }

                    if (hotelId.HasValue && cartItem.HotelId == hotelId)
                    {
                        cartItem.HotelId = null;
                    }

                    // Optional: if both become null, you could choose to remove the row
                    if (cartItem.TouristPlaceId == null && cartItem.HotelId == null)
                    {
                        _context.Carts.Remove(cartItem);
                    }
                }

                await _context.SaveChangesAsync();

                return Ok(new { message = "Cart updated successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting cart item. User: {UserId}, Cart: {CartId}, Place: {TouristPlaceId}, Hotel: {HotelId}",
                    userId, cartId, touristPlaceId, hotelId);
                return StatusCode(500, "Error updating cart");
            }
        }
       
    }


}
