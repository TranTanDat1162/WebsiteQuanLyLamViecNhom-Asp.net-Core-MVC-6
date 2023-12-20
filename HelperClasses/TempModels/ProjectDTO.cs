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
        public class  CreateGroupDTO
        {
            public string Id { get; set; }
            public string MOTD { get; set; }
            public int ProjectId { get; set; }
            public string[] memberList { get; set; }
        }
        public int ClassID { get; set; }
        public CreateGroupDTO? createGroupDTO { get; set; }
        public CreateProjectDTO? createProjectDTO { get; set; }
        public ICollection<Project>? CurrentProjects { get; set; }
        public ICollection<StudentClass>? StudentList{ get; set; }
    }
}
