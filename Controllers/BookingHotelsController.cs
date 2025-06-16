using APIs_Graduation.Data;
using APIs_Graduation.DTOs;
using APIs_Graduation.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[Route("api/hotel-bookings")]
[ApiController]
public class BookingHotelsController : ControllerBase
{
    private readonly ApplicationDbContext _context;

    public BookingHotelsController(ApplicationDbContext context)
    {
        _context = context;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateBooking([FromBody] HotelBookingRequest request)
    {
        var hotel = await _context.Hotels.FindAsync(request.HotelId);
        if (hotel == null) return NotFound("Hotel not found");

        var room = await _context.Rooms.FindAsync(request.RoomId);
        if (room == null || room.HotelId != request.HotelId)
            return NotFound("Room not found or does not belong to this hotel");

        if (!room.IsAvailable)
            return BadRequest("Room is not available for booking");

        int numberOfNights = (request.CheckOutDate - request.CheckInDate).Days;
        if (numberOfNights <= 0)
            return BadRequest("Check-out date must be after check-in date.");

        decimal totalPrice = numberOfNights * room.PricePerNight * request.NumberOfPersons;

        var booking = new HotelBooking
        {
            Username = request.Username,
            Email = request.Email,
            HotelId = request.HotelId,
            RoomId = request.RoomId, // ✅ حجز غرفة معينة
            CheckInDate = request.CheckInDate,
            CheckOutDate = request.CheckOutDate,
            Status = "Pending",
            NumberOfPersons = request.NumberOfPersons,
            TotalPrice = totalPrice
        };

        // تحديث حالة الغرفة إلى "غير متاحة"
        room.IsAvailable = false;

        _context.HotelBookings.Add(booking);
        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Booking Created Successfully",
            booking.BookingId,
            booking.Username,
            booking.Email,
            booking.HotelId,
            booking.RoomId, // ✅ عرض رقم الغرفة المحجوزة
            booking.CheckInDate,
            booking.CheckOutDate,
            booking.Status,
            booking.NumberOfPersons,
            booking.TotalPrice
        });
    }


    [HttpGet("all")]
    public async Task<IActionResult> GetAllBookings()
    {
        var bookings = await _context.HotelBookings
            .Include(b => b.Hotel) 
            .Include(b => b.Room) 
            .Select(b => new
            {
                b.BookingId,
                b.Username,
                b.Email,
                b.HotelId,
                HotelName = b.Hotel != null ? b.Hotel.Name : "Unknown", 
                HotelImage = b.Hotel != null
                    ? $"http://safarny.runasp.net/Hotel/{Uri.EscapeDataString(b.Hotel.PictureUrl.Replace("Hotel/", ""))}"
                    : null, 
                b.CheckInDate,
                b.CheckOutDate,
                b.NumberOfPersons,
                RoomType = b.Room != null ? b.Room.RoomType : "Not Specified",
                b.TotalPrice,
                b.Status
            })
            .ToListAsync();

        return Ok(bookings);
    }



    [HttpGet("{id}")]
    public async Task<IActionResult> GetBookingById(int id)
    {
        var booking = await _context.HotelBookings
            .Include(b => b.Hotel) // جلب بيانات الفندق
            .Include(b => b.Room)  // جلب بيانات الغرفة المحجوزة
            .Where(b => b.BookingId == id)
            .Select(b => new
            {
                b.BookingId,
                b.Username,
                b.Email,
                b.HotelId,
                HotelName = b.Hotel.Name,
                HotelImage = $"http://safarny.runasp.net/Hotel/{Uri.EscapeDataString(b.Hotel.PictureUrl.Replace("Hotel/", ""))}",
                RoomType = b.Room != null ? b.Room.RoomType : "Not Specified", // في حالة عدم تحديد غرفة
                RoomPrice = b.Room != null ? b.Room.PricePerNight : 0,
                b.CheckInDate,
                b.CheckOutDate,
                b.NumberOfPersons,
                b.TotalPrice,
                b.Status
            })
            .FirstOrDefaultAsync();

        if (booking == null) return NotFound("Booking not found");

        return Ok(booking);
    }
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteBooking(int id)
    {
        // البحث عن الحجز باستخدام الـ id
        var booking = await _context.HotelBookings.FindAsync(id);
        if (booking == null)
        {
            return NotFound("Booking not found");
        }

        // حذف المدفوعات المرتبطة بالحجز
        var payments = await _context.Payments.Where(p => p.HotelBookingId == id).ToListAsync();
        _context.Payments.RemoveRange(payments);

        // حذف الحجز
        _context.HotelBookings.Remove(booking);

        // إعادة الغرفة إلى الحالة المتاحة بعد الحذف
        var room = await _context.Rooms.FindAsync(booking.RoomId);
        if (room != null)
        {
            room.IsAvailable = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Booking and associated payments Deleted Successfully",
            booking.BookingId,
            booking.Username,
            booking.Email,
            booking.HotelId,
            booking.RoomId,
            booking.CheckInDate,
            booking.CheckOutDate,
            booking.Status,
            booking.NumberOfPersons,
            booking.TotalPrice
        });
    }



    [HttpPost("cancel/{id}")]
    public async Task<IActionResult> CancelBooking(int id)
    {
        var booking = await _context.HotelBookings.FindAsync(id);
        if (booking == null) return NotFound("Booking not found");

        if (booking.Status == "Cancelled")
            return BadRequest("Booking is already cancelled");

        booking.Status = "Cancelled";

        // 🔥 إعادة الغرفة إلى الحالة المتاحة
        var room = await _context.Rooms.FindAsync(booking.RoomId);
        if (room != null)
        {
            room.IsAvailable = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            Message = "Booking Cancelled Successfully",
            booking.BookingId,
            booking.Username,
            booking.Email,
            booking.HotelId,
            booking.RoomId,
            booking.CheckInDate,
            booking.CheckOutDate,
            booking.Status,
            booking.NumberOfPersons,
            booking.TotalPrice
        });
    }

}
