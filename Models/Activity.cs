namespace APIs_Graduation.Models
{
    public class Activity : BaseEntity
    {
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public int TouristPlacesId { get; set; }
        public TouristPlaces TouristPlaces { get; set; }
        public int? TripId { get; set; }
        public Trip Trip { get; set; }
        public int Popularity { get; set; }
    }
}
