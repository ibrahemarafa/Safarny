namespace APIs_Graduation.DTOs
{
    public class TripSummaryDTO
    {
        public int CityId { get; set; }
        public string CityName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysInCity { get; set; }
        public HotelShowDTO? Hotel { get; set; }
        public List<PlaceDTO> Places { get; set; }
        public string ArrivalAirport { get; internal set; }
        public string DepartureAirport { get; internal set; }
    }
}
