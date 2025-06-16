namespace APIs_Graduation.Models
{
    public class Hotel_Feature : BaseEntity
    {
        public string feature {  get; set; }
        public int? HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }
}
