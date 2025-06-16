namespace APIs_Graduation.DTOs
{
    public class CityTripDTO
    {
        public string CityName { get; set; }
        public List<PlaceDTO> Places { get; set; }
        public List<HotelShowDTO> Hotels { get; set; }
    }
}
