using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class StudentClass
    {
        [Key]
        public int Id { get; set; }
        public float? Score { get; set; }

        public int ClassId { get; set; }
        public string StudentId { get; set; }
        public string? GroupID { get; set; }

        public virtual Class Class { get; set; }
        public virtual Student Student { get; set; }
        public virtual Group Group { get; set; }
        public ICollection<Task> Tasks { get; set; }

    }
}
