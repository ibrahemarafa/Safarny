using APIs_Graduation.Models;

namespace APIs_Graduation.DTOs
{
    public class UserTripDTO
    {
        public int TripId { get; set; }
        public string CityName { get; set; }
        public List<TouristPlacesDTO> TouristPlaces { get; set; } = new List<TouristPlacesDTO>();
        public HotelDTO Hotel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}