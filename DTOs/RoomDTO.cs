namespace APIs_Graduation.DTOs
{
    public class RoomDTO
    {
        public int RoomId { get; set; }
        public int? HotelId { get; set; } 
        public string RoomType { get; set; }
        public int Capacity { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
    }


}
