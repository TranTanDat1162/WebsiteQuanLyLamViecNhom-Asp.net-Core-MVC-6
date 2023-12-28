using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public enum TaskStatus
    {
        OnGoing,
        Pending,
        Complete
    }
    public class Task
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string TaskId { get; set; }
        public string TaskName { get; set; }
        public TaskStatus Status { get; set; }
        public string? Attachment { get; set; }
        public DateTime DeadLineDate { get; set; }
        public float? TaskGradedByLeader { get; set; }
        public float? TaskGradedByLecturer { get; set; }
        public string GroupId { get; set; }
        public string? AttachmentLinksJson { get; set; }
        public virtual Group Group { get; set; }
        public virtual ICollection<StudentClass> StudentClass { get; set; }
        public string Description { get; set; }
    }
}
