using System.Text.Json.Serialization;

namespace APIs_Graduation.Models
{
    public enum Category
    {

        Historical ,
        Adventure ,
        Custom ,
        Luxury ,
        General,
        Cruise
    }

    public class UserPreference
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int StayDuration { get; set; }
        public decimal Budget { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Category CategoryPreference { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
