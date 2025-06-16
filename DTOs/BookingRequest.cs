namespace APIs_Graduation.DTOs
{
    public class BookingRequest
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public int PackageId { get; set; }
        public DateTime TripDate { get; set; }
        public int NumberOfPersons { get; set; } 
    }
}
