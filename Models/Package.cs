using APIs_Graduation.Migrations;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIs_Graduation.Models
{
    public class Package
    {
        [Key]
        public int PackageId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public decimal Price { get; set; }

        public string Duration { get; set; }

        public string ImageUrl { get; set; }

        public string CompanyName { get; set; }

        public string Category { get; set; }

        public string FacebookPage { get; set; } 

        [JsonIgnore]
        public List<PackagePlan> PackagePlans { get; set; } = new();

        [JsonIgnore]
        public List<PackageInclusion> PackageInclusions { get; set; } = new();

        [JsonIgnore]
        public List<PackageExclusion> PackageExclusions { get; set; } = new();
        [JsonIgnore]
        public List<PackageBooking> Bookings { get; set; } = new();
    }
}
