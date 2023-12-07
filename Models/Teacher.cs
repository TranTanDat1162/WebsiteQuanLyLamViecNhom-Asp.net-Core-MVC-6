using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Hosting;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class Teacher : BaseApplicationUser
    {
        public string? TeacherCode { get; set; }
        [NotMapped]
        public IFormFile? ImgPfp { get; set; }
        public string? ImgId { get; set; }
    }
}
