namespace APIs_Graduation.Models
{
    public class Restaurant : BaseEntity
    {
        public string Name { get; set; }
        public string PictureUrl { get; set; }
        public int CityId { get; set; }
        public City city { get; set; }
        public string Type { get; set; }
        public string PriceRange { get; set; }
        public string Rate { get; set; }
        public string DiningOptions { get; set; }
        public string OpeningHours { get; set; }
    }
}
