using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class User : IdentityUser
    {
        public Teacher teacher { get; set; }
    }
    public class Teacher
    {
        [Key]
        public int TeacherId { get; set; }
        public string? TeacherCode { get; set; }
        [Required]
        public string? FirstName { get; set; }
        [Required]
        public string? LastName { get; set; }
        [Required]
        public string? Email { get; set; } 
        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        [NotMapped]
        public IFormFile? ImgPfp { get; set; }
        public string? ImgId { get; set; }
        public Boolean IsLocked { get; set; }

        [ForeignKey("User")]
        [MaxLength(450)]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}
