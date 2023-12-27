using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.HelperClasses.TempModels
{
    public class GroupDTO
    {
        public class GroupVM
        {
            public string MOTD { get; set; }
            public int ProjectId { get; set; }
            public string ProjectName { get; set; }
            public string? ProjectAttachmentsJSON { get; set; }
            public string LeaderID { get; set; }
            public string GroupID { get; set; }
            public string LeaderName { get; set; } = string.Empty;
            public Class CurrentClass { get; set; }
            public ICollection<StudentClass> memberList { get; set; }
            public ICollection<Models.Task>? Tasks { get; set; } = new List<Models.Task>();
        }
        public class TaskDTO
        {
            public string TaskName { get; set; }
            public string[] memberList { get; set; }
            public DateTime Deadline { get; set; }
            public string Description { get; set; }
        }
        public class UpdateTaskDTO
        {
            public string TaskID { get; set; }
            public string Description { get; set; }
            public float? Grade { get; set; }
            public IList<IFormFile>? Attachments { get; set; }
        }
        public class GradeGroupDTO
        {
            public string GroupID { get; set; }
            public string? Description { get; set; }
            public float TeacherGrade { get; set; }
            public float LeaderAGVGrade { get; set; }
        }
        public GradeGroupDTO gradeGroupDTO { get; set; }
        public UpdateTaskDTO updateTaskDTO { get; set; }
        public TaskDTO createTaskDTO { get; set; }
        public GroupVM GroupViewModel {  get; set; }

    }
}
