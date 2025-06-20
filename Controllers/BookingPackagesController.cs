using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/bookings")]
[ApiController]
public class BookingPackagesController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BookingPackagesController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBooking([FromBody] BookingRequest request)
    {
        var package = await _context.Packages.FindAsync(request.PackageId);
        if (package == null) return NotFound("Package not found");

        decimal totalPrice = package.Price * request.NumberOfPersons;

        var booking = new PackageBooking
        {
            Username = request.Username,
            Email = request.Email,
            PackageId = request.PackageId,
            TripDate = request.TripDate,
            BookingDate = DateTime.UtcNow,
            Status = "Pending",
            NumberOfPersons = request.NumberOfPersons,
            TotalPrice = totalPrice
        };

        _context.PackageBookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Booking Created Successfully",
            booking.BookingId,
            booking.Username,
            booking.Email,
            booking.PackageId,
            booking.TripDate,
            booking.BookingDate,
            booking.Status,
            booking.NumberOfPersons,
            booking.TotalPrice
        });
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _context.PackageBookings
            .Select(b => new
            {
                b.BookingId,
                b.Username,
                b.Email,
                b.PackageId,
                b.TripDate,
                b.BookingDate,
                b.Status,
                b.NumberOfPersons,
                b.TotalPrice
            })
            .ToListAsync();

        return Ok(bookings);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var booking = await _context.PackageBookings
            .Where(b => b.BookingId == id)
            .Select(b => new
            {
                b.BookingId,
                b.Username,
                b.Email,
                b.PackageId,
                b.TripDate,
                b.BookingDate,
                b.Status,
                b.NumberOfPersons,
                b.TotalPrice
            })
            .FirstOrDefaultAsync();

        if (booking == null) return NotFound("Booking not found");

        return Ok(booking);
    }

    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var booking = await _context.PackageBookings.FindAsync(id);
        if (booking == null) return NotFound("Booking not found");

        if (booking.Status == "Cancelled")
            return BadRequest("Booking is already cancelled");

        booking.Status = "Cancelled";
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Booking Cancelled Successfully",
            booking.BookingId,
            booking.Username,
            booking.Email,
            booking.PackageId,
            booking.TripDate,
            booking.BookingDate,
            booking.Status,
            booking.NumberOfPersons,
            booking.TotalPrice // عرض السعر حتى بعد الإلغاء
        });
    }
}
