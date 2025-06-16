using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace APIs_Graduation.Models
{
    public class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        public int? PackageBookingId { get; set; }  // لحجوزات الباقات السياحية
        public int? HotelBookingId { get; set; }    // لحجوزات الفنادق
        public int? CustomTripBookingId { get; set; } // لحجوزات الرحلات المخصصة

        [Required]
        [StringLength(50)]
        public string BookingType { get; set; } // "Package" أو "Hotel" أو "CustomTrip"

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Pending";  // (Pending, Paid, Failed)

        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        // العلاقات مع الحجوزات المختلفة
        [ForeignKey("PackageBookingId")]
        [JsonIgnore]
        public virtual PackageBooking PackageBooking { get; set; }

        [ForeignKey("HotelBookingId")]
        [JsonIgnore]
        public virtual HotelBooking HotelBooking { get; set; }

        [ForeignKey("CustomTripBookingId")]
        [JsonIgnore]
        public virtual BookingCustomTrip BookingCustomTrip { get; set; }
    }
}
