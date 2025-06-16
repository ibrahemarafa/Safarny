using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace APIs_Graduation.Models
{
    public class ApplicationUser:IdentityUser
    {

        [Required, MaxLength(30)]
        public string FirstName { get; set; }

        [Required, MaxLength(30)]
        public string LastName { get; set; }
        [Required, MaxLength(50)]
        public string Nationality { get; set; }
        
    }
}
