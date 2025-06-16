using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIs_Graduation.Data;
using APIs_Graduation.Models;
using APIs_Graduation.DTOs;
using APIs_Graduation.Services;

namespace APIs_Graduation.Controllers
{
    [Route("api/hotel-payments")]
    [ApiController]
    public class HotelPaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PaymobService _paymobService;
        private const string PaymobIframeId = "905633";

        public HotelPaymentController(ApplicationDbContext context, PaymobService paymobService)
        {
            _context = context;
            _paymobService = paymobService;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> MakePayment([FromBody] PaymentRequest request)
        {
            var hotelBooking = await _context.HotelBookings.FindAsync(request.BookingId);
            if (hotelBooking == null)
                return NotFound("Hotel booking not found");

            decimal totalPrice = hotelBooking.TotalPrice;

            var token = await _paymobService.GetTokenAsync();
            var orderId = await _paymobService.CreateOrderAsync(token, totalPrice);
            var paymentKey = await _paymobService.GetPaymentKeyAsync(token, orderId, totalPrice);

            var payment = new Payment
            {
                HotelBookingId = hotelBooking.BookingId, // ربط الـ Payment بحجز الفندق
                Amount = totalPrice,
                Status = "Pending",
                BookingType = "Hotel"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            string paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{PaymobIframeId}?payment_token={paymentKey}";

            return Ok(new { PaymentUrl = paymentUrl, TotalPrice = totalPrice });
        }

        [HttpPost("payment-webhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] PaymobWebhookData data)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.HotelBookingId == data.MerchantOrderId); // استخدام HotelBookingId بدلاً من BookingId
            if (payment == null)
                return NotFound("Payment not found");

            if (data.Success)
            {
                payment.Status = "Paid";
                var hotelBooking = await _context.HotelBookings.FindAsync(payment.HotelBookingId);
                if (hotelBooking != null)
                    hotelBooking.Status = "Confirmed";
            }
            else
            {
                payment.Status = "Failed";
            }

            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
