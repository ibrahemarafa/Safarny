
namespace APIs_Graduation.Models
{
    public class TouristPlaces : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string PictureUrl { get; set; }

        public int CityId { get; set; }
        public City City { get; set; }
        public  string Category { get; set; }
       public double Price {  get; set; }
        public double Rate { get; set; }
    
        public string Address {  get; set; }
        public ICollection<Trip_Place> TripPlaces { get; set; } = new List<Trip_Place>();
    }
}
