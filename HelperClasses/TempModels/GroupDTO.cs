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
            public string LeaderID { get; set; }
            public string LeaderName { get; set; } = string.Empty;
            public ICollection<StudentClass> memberList { get; set; }
            public ICollection<Models.Task>? Tasks { get; set; } = new List<Models.Task>();
        }
        public class TaskDTO
        {
            public int TaskName { get; set; }
            public string[] memberList { get; set; }
            public DateTime? Deadline { get; set; }
            public string Description { get; set; }
        }
        public GroupVM GroupViewModel {  get; set; }

    }
}
