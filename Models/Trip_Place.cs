namespace APIs_Graduation.Models
{
    public class Trip_Place:BaseEntity
    {

        public int TripId { get; set; }
        public Trip Trip { get; set; }

        public int PlaceId { get; set; }
        public TouristPlaces Place { get; set; }
    }
}
