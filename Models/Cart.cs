namespace APIs_Graduation.Models
{
    public class Cart
    {
        public int Id { get; set; }
        public string UserId { get; set; } 
        public int? TouristPlaceId { get; set; } 
        public int? HotelId { get; set; } 
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
        public int CityId { get; set; }

        //public DateTime EndDate { get; set; } 

        // Navigation properties
        public ApplicationUser User { get; set; }
        public TouristPlaces? TouristPlace { get; set; }
        public Hotel? Hotel { get; set; }
        public City? City { get; set; }
    }
}
