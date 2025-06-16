namespace APIs_Graduation.Models
{
    public class Hotel_Image : BaseEntity
    {
        public string PictureUrl { get; set; }
        public int? HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
