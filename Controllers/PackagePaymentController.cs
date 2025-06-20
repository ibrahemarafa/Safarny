using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using APIs_Graduation.Data;
using APIs_Graduation.Models;
using APIs_Graduation.DTOs;
using APIs_Graduation.Services;

namespace APIs_Graduation.Controllers
{
    [Route("api/packagepayments")]
    [ApiController]
    public class PackagePaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly PaymobService _paymobService;
        private const string PaymobIframeId = "905633";

        public PackagePaymentController(ApplicationDbContext context, PaymobService paymobService)
        {
            _context = context;
            _paymobService = paymobService;
        }

        [HttpPost("pay")]
        public async Task<IActionResult> MakePayment([FromBody] PaymentRequest request)
        {
            var booking = await _context.PackageBookings.FindAsync(request.BookingId);
            if (booking == null)
                return NotFound("Package booking not found");

            decimal totalPrice = booking.TotalPrice;

            var token = await _paymobService.GetTokenAsync();
            var orderId = await _paymobService.CreateOrderAsync(token, totalPrice);
            var paymentKey = await _paymobService.GetPaymentKeyAsync(token, orderId, totalPrice);

            var payment = new Payment
            {
                PackageBookingId = booking.BookingId,
                Amount = totalPrice,
                Status = "Pending",
                BookingType = "Package"
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            string paymentUrl = $"https://accept.paymob.com/api/acceptance/iframes/{PaymobIframeId}?payment_token={paymentKey}";

            return Ok(new { PaymentUrl = paymentUrl, TotalPrice = totalPrice });
        }

        [HttpPost("paymentwebhook")]
        public async Task<IActionResult> PaymentWebhook([FromBody] PaymobWebhookData data)
        {
            var payment = await _context.Payments.FirstOrDefaultAsync(p => p.PackageBookingId == data.MerchantOrderId);
            if (payment == null)
                return NotFound("Payment not found");

            if (data.Success)
            {
                payment.Status = "Paid";
                var booking = await _context.PackageBookings.FindAsync(payment.PackageBookingId);
                if (booking != null)
                    booking.Status = "Confirmed";
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
