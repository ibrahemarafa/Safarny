namespace APIs_Graduation.DTOs
{
    public class UserInteractionDTO
    {
        public string Username { get; set; }  // The user who interacted
        public int TouristPlaceId { get; set; }  // ID of the tourist place
        public int ActivityId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }
}
