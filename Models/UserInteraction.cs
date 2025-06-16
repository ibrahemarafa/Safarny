namespace APIs_Graduation.Models
{
    public class UserInteraction
    {
        public int Id { get; set; }
        public string UserId { get; set; } // Identity User ID
        public string Username { get; set; }
        public string ItemType { get; set; } // e.g., "TouristPlace", "Hotel", "Package"
        public int ItemId { get; set; } // The ID of the place, hotel, etc.
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public int TouristPlaceId { get; set; }
        public TouristPlaces TouristPlace { get; set; }
        public int ActivityId { get; set; }  
        public Activity Activity { get; set; }
        public int CityId { get; internal set; }
        public City City { get; internal set; }
        // public DateTime InteractionTime { get; set; }  // Timestamp
    }
}
