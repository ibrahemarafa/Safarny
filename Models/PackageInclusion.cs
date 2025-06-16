using System.ComponentModel.DataAnnotations;

namespace APIs_Graduation.Models
{
    public class PackageInclusion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        public int PackageId { get; set; }
        public Package Package { get; set; }
    }

}
