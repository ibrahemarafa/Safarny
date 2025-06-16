using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIs_Graduation.Models
{
    public class Room : BaseEntity
    {
        [Key]
        public int RoomId { get; set; }

        [Required]
        public int? HotelId { get; set; }

        [ForeignKey("HotelId")]
        public Hotel Hotel { get; set; }

        [Required]
        public string RoomType { get; set; } 

        [Required]
        public int Capacity { get; set; } // عدد الأشخاص اللي تتحملهم الغرفة

        [Required]
        public decimal PricePerNight { get; set; } // سعر الغرفة لكل ليلة

        public bool IsAvailable { get; set; } = true;

        public string Name { get; set; }    
        public string PictureUrl { get; set; }


    }
}
