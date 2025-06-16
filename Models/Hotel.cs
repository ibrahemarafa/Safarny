using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIs_Graduation.Models
{
    public class Hotel : BaseEntity
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string PictureUrl { get; set; }

        [Required]
        public int CityId { get; set; }

        [ForeignKey("CityId")]
        public City City { get; set; }

        [Range(0, 5)]
        public double Rate { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public double StartPrice { get; set; }

        public string Overview { get; set; }

        // قائمة الصور الخاصة بالفندق
        public List<Hotel_Image> Hotel_Images { get; set; } = new List<Hotel_Image>();

        // قائمة الغرف المتاحة داخل الفندق
        public List<Room> Rooms { get; set; } = new List<Room>();

        // قائمة الميزات الخاصة بالفندق
        public List<Hotel_Feature> Hotel_Features { get; set; } = new List<Hotel_Feature>();
    }
}
