using System.ComponentModel.DataAnnotations.Schema;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.HelperClasses.TempModels
{
    public class ProjectDTO
    {
        public class CreateProjectDTO
        {
            public string? Name { get; set; }
            public string? Requirement { get; set; }
            public DateTime Deadline { get; set; }

        }
        public class UpdateProjectDTO
        {
            public int Id { get; set; }
            public string? Name { get; set; }
            public string? Requirement { get; set; }
            public DateTime Deadline { get; set; }
            public IList<IFormFile>? Attachments { get; set; }

        }
        public class  CreateGroupDTO
        {
            public string MOTD { get; set; }
            public int ProjectId { get; set; }
            public string LeaderID { get; set; }
            public string[] memberList { get; set; }
        }
        public int? ClassID { get; set; }
        public string? TeacherName { get; set; }
        public string? TeacherId { get; set; }
        public CreateGroupDTO? createGroupDTO { get; set; }
        public CreateProjectDTO? createProjectDTO { get; set; }
        public UpdateProjectDTO? updateProjectDTO { get; set; }
        public ICollection<Project>? CurrentProjects { get; set; } = new List<Project>();
        public ICollection<Group>? CurrentGroups { get; set; } = new List<Group>();
        public ICollection<StudentClass> StudentList{ get; set; }
        public List<List<string>> crumbs { get; set; }

    }
}
