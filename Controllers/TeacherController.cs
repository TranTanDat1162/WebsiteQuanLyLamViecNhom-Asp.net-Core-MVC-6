using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;
using System.Security.Claims;
using Google.Apis.Drive.v3.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.CreateClassDTO;

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

        public async Task<IActionResult> CreateClass(CreateClassDTO createClassDTO)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    if (createClassDTO.classDTO == null) {
                        createClassDTO.classDTO.OpenDate = DateTime.Now;
                    }
                    Class newClass = new Class
                    {
                        SubjectName = createClassDTO.classDTO.SubjectName,
                        SubjectId = createClassDTO.classDTO.SubjectId,
                        Code = createClassDTO.classDTO.Code,
                        ClassGroup = createClassDTO.classDTO.Code.Substring(createClassDTO.classDTO.Code.Length - 3),
                        RoleGroup = createClassDTO.classDTO.RoleGroup,
                        RoleProject = createClassDTO.classDTO.RoleProject,
                        ProjectRequirements = createClassDTO.classDTO.ProjectRequirements,
                        OpenDate = (DateTime)createClassDTO.classDTO.OpenDate,
                        Year =  int.Parse(createClassDTO.classDTO.Year.Substring(0, 4)),
                        Semester = createClassDTO.classDTO.Semester,
                        TeacherId = viewModel.Id
                    };
                    foreach (var student in createClassDTO.classDTO.Students)
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
                            _context.Add(newStudent);
                            await _context.SaveChangesAsync();
                        }
                        else
                        {
                            TempStudent.ClassList.Add(new StudentClass
                            {
                                ClassId = newClass.Id,
                                StudentId = TempStudent.Id,
                                Class = newClass,
                                Student = TempStudent,                        
                            });
                            _context.Update(TempStudent);
                        }
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
