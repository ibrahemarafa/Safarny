using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIs_Graduation.Data;
using APIs_Graduation.Models;
using APIs_Graduation.DTOs;
using APIs_Graduation.Services;

namespace APIs_Graduation.Controllers
{
    [Route("api/trip-payments")]
    [ApiController]
    public class TripPaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PaymobService _paymobService;
        private const string PaymobIframeId = "905633"; // غيره لو عندك Iframe تاني

        public TripPaymentController(ApplicationDbContext context, PaymobService paymobService)
        {
            _context = context;
            _paymobService = paymobService;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> MakePayment([FromBody] PaymentRequest request)
        {
            var booking = await _context.BookingCustomTrips.FindAsync(request.BookingId);
            if (booking == null)
                return NotFound(new { message = "Custom trip booking not found" });

            decimal totalPrice = (decimal)booking.TotalPrice;

            var token = await _paymobService.GetTokenAsync();
            var orderId = await _paymobService.CreateOrderAsync(token, totalPrice);
            var paymentKey = await _paymobService.GetPaymentKeyAsync(token, orderId, totalPrice);

            var payment = new Payment
            {
                CustomTripBookingId = booking.Id, // لاحظ هنا CustomTripBookingId
                Amount = totalPrice,
                Status = "Pending",
                BookingType = "CustomTrip"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            string paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{PaymobIframeId}?payment_token={paymentKey}";

            return Ok(new { PaymentUrl = paymentUrl, TotalPrice = totalPrice });
        }

        [HttpPost("payment-webhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] PaymobWebhookData data)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.CustomTripBookingId == data.MerchantOrderId);
            if (payment == null)
                return NotFound(new { message = "Payment not found" });

            if (data.Success)
            {
                payment.Status = "Paid";
                var booking = await _context.BookingCustomTrips.FindAsync(payment.CustomTripBookingId);
                if (booking != null)
                {
                    // تقدر تحدث حالة الحجز لو ضايف Status مثلا
                    // booking.Status = "Confirmed";
                }
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
