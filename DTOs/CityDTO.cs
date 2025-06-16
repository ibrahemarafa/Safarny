namespace APIs_Graduation.DTOs
{
    public class CityDTO
    {

        public int Id { get; set; }  
        public string Name { get; set; }  
        public string Description { get; set; } 
        public List<TouristPlacesDTO> TouristPlaces { get; set; } 
        public List<HotelDTO> Hotels { get; set; } 
    }
}
