using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;
using System.Security.Claims;
using Google.Apis.Drive.v3.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        
        static Teacher? viewModel = new Teacher
        {
            TeacherCode = "Teacher",
            ImgId = null
        };


        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng đăng nhập
            viewModel = await _context.Teacher.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            // Kiểm tra xem người dùng có tồn tại không
            if (viewModel != null)
            {
                ViewData["Teacher"] = viewModel;
                return View();
            }

            // Xử lý trường hợp không có người dùng đăng nhập
            return NotFound();
        }

        public async Task<IActionResult> TeacherClass()
        {
            ViewData["Teacher"] = viewModel;
            return View();
        }

        public async Task<IActionResult> CreateClass(CreateClassDTO classDTO)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    if (classDTO.classDTO == null) {
                        classDTO.classDTO.OpenDate = DateTime.Now;
                    }
                    Class newClass = new Class
                    {
                        SubjectName = classDTO.classDTO.SubjectName,
                        SubjectId = classDTO.classDTO.SubjectId,
                        Code = classDTO.classDTO.Code,
                        ClassGroup = classDTO.classDTO.Code.Substring(classDTO.classDTO.Code.Length - 3),
                        RoleGroup = classDTO.classDTO.RoleGroup,
                        RoleProject = classDTO.classDTO.RoleProject,
                        ProjectRequirements = classDTO.classDTO.ProjectRequirements,
                        OpenDate = (DateTime)classDTO.classDTO.OpenDate,
                        Year =  int.Parse(classDTO.classDTO.Year.Substring(0, 4)),
                        Semester = classDTO.classDTO.Semester,
                        TeacherId = viewModel.Id
                    };
                    foreach (var student in classDTO.classDTO.Students)
                    {
                        Student? TempStudent = await _context.Student.FirstOrDefaultAsync(e => e.StudentCode == student.StudentCode);
                        if (TempStudent == null)
                        {
                            Student newStudent = new Student
                            {
                                StudentCode = student.StudentCode,
                                FirstName = student.StudentFirstName,
                                LastName = student.StudentLastName,
                                DOB = (DateTime)student.DOB
                            };
                        }
                        //else
                        //{
                        //    TempStudent.ClassList.Add
                        //}
                    }
                    _context.Add(newClass);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Teacher");

                }
                catch (Exception ex)
                {
                    RedirectToAction("Index", "Teacher", ex.Message);
                }
                return RedirectToAction("Index", "Teacher");
            }
            return RedirectToAction("Index", "Teacher", ModelState);
        }
    }
}
