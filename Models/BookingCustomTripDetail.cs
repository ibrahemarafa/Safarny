using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIs_Graduation.Models
{
    public class BookingCustomTripDetail
    {
        public int Id { get; set; }

        [Required]
        public int BookingId { get; set; }

        [Required]
        public int CityId { get; set; }

        public int? HotelId { get; set; }  // تم تغييره ليكون nullable

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [ForeignKey("BookingId")]
        public BookingCustomTrip BookingCustomTrip { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }

        [ForeignKey("HotelId")]  // ربط الـ Hotel بـ HotelId
        public Hotel? Hotel { get; set; }  // تم تغييره ليكون nullable لأنه مرتبط بـ HotelId nullable
    }
}
