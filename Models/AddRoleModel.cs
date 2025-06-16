using System.ComponentModel.DataAnnotations;

namespace APIs_Graduation.Models
{
    public class AddRoleModel
    {

        [Required]
        public string UserId { get; set; }
        [Required]
        public string Role { get; set; }
    }
}
