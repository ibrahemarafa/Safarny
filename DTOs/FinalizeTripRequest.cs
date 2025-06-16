namespace APIs_Graduation.DTOs
{
    public class FinalizeTripRequest
    {
        public string UserId { get; set; } 
        public int CityId { get; set; } 
        public int? HotelId { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } 
        public double Rate { get; set; } 
    }
}
