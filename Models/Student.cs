using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class Student : BaseApplicationUser
    {
        public int StudentId { get; set; }
        [Required]
        public string? StudentCode { get; set; }
    }
}
