using System.ComponentModel.DataAnnotations;
using WebsiteQuanLyLamViecNhom.Models;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.CreateClassDTO.StudentDTO;

namespace WebsiteQuanLyLamViecNhom.HelperClasses.TempModels
{
    public class CreateClassDTO
    {
        public class StudentDTO
        {
            public string? StudentCode { get; set; }
            public string? StudentLastName { get; set; }
            public string? StudentFirstName { get; set; }
            public string? Email { get; set; }
            public DateTime? DOB { get; set; }
            public string? StudentImgId { get; set; }
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
        public class ChangePasswordDTO
        {
            [Required]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu hiện tại")]
            public string? CurrentPassword { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} " +
                "characters long.", MinimumLength = 8)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string? NewPassword { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu xác thực")]
            [Compare("NewPassword", ErrorMessage = "Mật khẩu mới không trùng với mật khẩu xác thực")]
            public string? ConfirmPassword { get; set; }

        }
        public StudentDTO? studentDTO { get; set; }
        public ClassDTO? classDTO { get; set; }
        public ICollection<Class>? ClassListDTO { get; set; } = new List<Class>();
        public ICollection<StudentClass>? StudentClassListDTO { get; set; } = new List<StudentClass>();
        public List<List<string>>? crumbs { get; set; }
        public ChangePasswordDTO? changePasswordDTO { get; set; }
    }

}
