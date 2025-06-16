namespace APIs_Graduation.DTOs
{
    public class ActivitiesDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public int TouristPlaceId { get; set; }
        public int? TripId { get; internal set; }
    }
}
