using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIs_Graduation.Models
{
    public class PackageBooking
    {
        [Key]
        public int BookingId { get; set; }

        public string Username { get; set; }

        [EmailAddress]
        public string Email { get; set; }

        public int PackageId { get; set; }

        public DateTime TripDate { get; set; }

        public DateTime BookingDate { get; set; } = DateTime.UtcNow;

        public string Status { get; set; } = "Pending";

        public int NumberOfPersons { get; set; }

        public decimal TotalPrice { get; set; } 

        [JsonIgnore]
        public Package Package { get; set; }
    }
}
