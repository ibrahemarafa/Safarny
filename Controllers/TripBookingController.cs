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
        }

        [HttpPost("addToCart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest("Invalid request.");

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

        [HttpDelete("deleteCartItem/{id}")]
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

                if (cartId.HasValue && !touristPlaceId.HasValue && !hotelId.HasValue)
                {
                    _context.Carts.Remove(cartItem);
                }
                else
                {
                    if (touristPlaceId.HasValue && cartItem.TouristPlaceId == touristPlaceId)
                    {
                        cartItem.TouristPlaceId = null;
                    }

                    if (hotelId.HasValue && cartItem.HotelId == hotelId)
                    {
                        cartItem.HotelId = null;
                    }

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
