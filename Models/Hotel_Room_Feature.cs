namespace APIs_Graduation.Models
{
    public class Hotel_Room_Feature : BaseEntity
    {
        public string FeatureRoom { get; set; }
        public int RoomId { get; set; }
        public Room Hotel_Room { get; set; }
    }
}
