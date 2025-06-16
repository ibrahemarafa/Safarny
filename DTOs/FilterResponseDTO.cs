namespace APIs_Graduation.DTOs
{
    public class FilterResponseDTO
    {

        public string Message { get; set; }

        public List<CityDTO> DefaultCities { get; set; }

        public List<HotelDTO> PopularHotels { get; set; }

        public List<TouristPlacesDTO> PopularPlaces { get; set; }
    }
}
