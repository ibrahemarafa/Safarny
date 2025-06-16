using System.ComponentModel.DataAnnotations;

namespace APIs_Graduation.Models
{
    public class Trip : BaseEntity
    {
        public string Name { get; set; }

        [StringLength(500)]
        public string ?Description { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public int ?GuestNumber { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public int? HotelId { get; set; }
        public Hotel? Hotel { get; set; }

        [Required]
        [Range(1, 5)]
        public int? StarRating { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }
      // public decimal Price { get; set; }
       // public string ?Status { get; set; } = string.Empty;
        public string? Destination { get; set; }

        public ICollection<Activity> Activities { get; set; } = new List<Activity>();

        public ICollection<Trip_Place> TripPlaces { get; set; } = new List<Trip_Place>();
    }

}
