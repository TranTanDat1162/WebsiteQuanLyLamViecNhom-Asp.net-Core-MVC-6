using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class Student
    {
        public int StudentId { get; set; }
        [Required]
        public string? FullName { get; set; }
        [Required]
        public string? Email { get; set; }
        [Required]
        [DataType(DataType.Date)]
        public DateTime DOB { get; set; }
        public Boolean IsLocked { get; set; }

    }
}
