using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class Student : BaseApplicationUser
    {
        public int StudentId { get; set; }
        [Required]
        public string? StudentCode { get; set; }
        [NotMapped]
        public IFormFile? StudentImgPfp { get; set; }
        public string? StudentImgId { get; set; }
        public ICollection<StudentClass> ClassList { get; set; } = new List<StudentClass>();
    }
}
