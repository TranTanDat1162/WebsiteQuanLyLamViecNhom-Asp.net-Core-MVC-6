using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class Project
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? Requirements { get; set; }
        public DateTime Deadline { get; set; }
        public int ClassId { get; set; }
        public Class Class { get; set; }
        public string? fileIDJSON { get; set; }
    }
}
