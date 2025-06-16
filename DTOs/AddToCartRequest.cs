namespace APIs_Graduation.DTOs
{
    public class AddToCartRequest
    {
        public string UserId { get; set; } 
        public int? TouristPlaceId { get; set; } 
        public int? HotelId { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
    }
}
