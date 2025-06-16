namespace APIs_Graduation.DTOs
{
    public class HotelBookingRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int HotelId { get; set; }
        public int RoomId { get; set; } // 🔥 الغرفة التي سيتم حجزها
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int NumberOfPersons { get; set; }
    }


}
