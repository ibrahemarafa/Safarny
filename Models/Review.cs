using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIs_Graduation.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PackageId { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("PackageId")]
        [JsonIgnore]
        public Package? Package { get; set; }
    }
}
