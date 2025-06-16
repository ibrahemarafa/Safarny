using APIs_Graduation.Models;

namespace APIs_Graduation.DTOs
{
    public class TouristPlacesDTO
    {
        public double Rate { get; set; }

        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }
        public int CityId { get; set; }
        public string Category { get; set; }

        public double Price {  get; set; }
      public ICollection<Trip_Place> TripPlaces { get; set; } = new List<Trip_Place>();
    }
}

