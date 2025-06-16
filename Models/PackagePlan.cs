using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace APIs_Graduation.Models
{
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json.Serialization;

    public class PackagePlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int PackageId { get; set; }

        [JsonIgnore]
        public Package? Package { get; set; }
    }


}


