using APIs_Graduation.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

public class HotelBooking
{
    [Key]
    public int BookingId { get; set; }
    public string Username { get; set; }

    [EmailAddress]
    public string Email { get; set; }

    public int? HotelId { get; set; }

    [ForeignKey("HotelId")]
    public Hotel Hotel { get; set; }

    public int RoomId { get; set; } // 🔥 الغرفة التي تم حجزها

    [ForeignKey("RoomId")]
    public Room Room { get; set; }

    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public int NumberOfPersons { get; set; }
    public decimal TotalPrice { get; set; }
    public string Status { get; set; } = "Pending";
}
