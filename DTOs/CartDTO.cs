namespace APIs_Graduation.DTOs
{
    public class CartDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int? TouristPlaceId { get; set; }
        public int? HotelId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TouristPlaceName { get; set; }
        public string HotelName { get; set; }
    }
}
