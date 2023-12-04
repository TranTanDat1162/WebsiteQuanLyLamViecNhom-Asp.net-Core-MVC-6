using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class BaseApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        public Boolean IsLocked { get; set; }

    }
}
