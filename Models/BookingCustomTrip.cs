using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace APIs_Graduation.Models
{
    public class BookingCustomTrip
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }

        public DateTime BookingDate { get; set; }

        public double TotalPrice { get; set; }

        // إضافة الحقول الجديدة
        [Required]
        [StringLength(100)] // تحديد طول الاسم
        public string FullName { get; set; }

        [Required]
        [EmailAddress] // التحقق من صحة الإيميل
        public string Email { get; set; }

        [Required]
        [Phone] // التحقق من صحة رقم الهاتف
        public string PhoneNumber { get; set; }

        public ICollection<BookingCustomTripDetail> BookingCustomTripDetail { get; set; }
    }
}
