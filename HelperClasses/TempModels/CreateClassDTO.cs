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
            public string Email { get; set; }
            public DateTime? DOB { get; set; }
            public string StudentImgId { get; set; }
            public IFormFile? StudentImgPfp { get; set; }
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
        public StudentDTO? studentDTO { get; set; }
        public ClassDTO classDTO { get; set; }
        public ICollection<Class>? ClassListDTO { get; set; } = new List<Class>();
        public ICollection<StudentClass>? StudentClassListDTO { get; set; } = new List<StudentClass>();
        public List<List<string>>? crumbs { get; set; }

    }

}
