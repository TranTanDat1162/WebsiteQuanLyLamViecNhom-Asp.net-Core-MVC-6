using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.HelperClasses.TempModels
{
    public class CreateClassDTO
    {
        public class StudentDTO
        {
            public string StudentCode { get; set; }
            public string StudentLastName { get; set; }
            public string StudentFirstName { get; set; }
            public DateTime? DOB { get; set; }
        }
        public class ClassDTO
        {
            public string SubjectName { get; set; }
            public string SubjectId { get; set; }
            public string Code { get; set; }
            public RoleGroup RoleGroup { get; set; }
            public RoleProject RoleProject { get; set; }
            public DateTime? OpenDate { get; set; }
            public string Year { get; set; }
            public string Semester { get; set; }
            public string? ProjectRequirements { get; set; }
            public List<StudentDTO> Students{ get; set; }

        }
        public ClassDTO classDTO { get; set; }
        public ICollection<Class>? ClassListDTO { get; set; } = new List<Class>();
        public List<List<string>> crumbs { get; set; }

    }

}
