using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;
using System.Security.Claims;
using Google.Apis.Drive.v3.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.CreateClassDTO;
using WebsiteQuanLyLamViecNhom.Data.Migrations;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.BaseApplicationUser> _userManager;
        private readonly ILogger<TeacherController> _logger;


        static Teacher? viewModel = new Teacher
        {
            TeacherCode = "Teacher",
            ImgId = null
        };


        public TeacherController(ApplicationDbContext context, UserManager<Models.BaseApplicationUser> userManager, ILogger<TeacherController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
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
            createClassDTO.classDTO.Students.RemoveAt(createClassDTO.classDTO.Students.Count - 1);
            //TODO: Still missing some condition and the condition below is rubbishly workable
            if (ModelState.IsValid)
            {
                try
                {
                    if (createClassDTO.classDTO.OpenDate == null) {
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
                    _context.Add(newClass);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Class has been created",
                        new { studentId = newClass.Id, code = newClass.Code });

                    foreach (var student in createClassDTO.classDTO.Students)
                    {
                        //Student? TempStudent = await _context.Student.FirstOrDefaultAsync(e => e.StudentCode == student.StudentCode);
                        Student? TempStudent = await _context.Student
                                    .Include(s => s.ClassList)
                                    .FirstOrDefaultAsync(e => e.StudentCode == student.StudentCode);
                        if (TempStudent == null)
                        {
                            Student newStudent = new Student
                            {
                                StudentCode = student.StudentCode,
                                FirstName = student.StudentFirstName,
                                LastName = student.StudentLastName,
                                DOB = (DateTime)student.DOB,
                                UserName = student.StudentCode,
                                IsLocked = true
                            };
                            //TODO: set a "randomly" generated id for studentId field. https://stackoverflow.com/questions/26967215/generate-seemingly-random-unique-numeric-id-in-sql-server
                            var result = await _userManager.CreateAsync(newStudent, newStudent.DOB.ToString("ddMMyyyy"));
                            if (result.Succeeded)
                            {
                                await _userManager.AddToRoleAsync(newStudent, "Student");
                                _logger.LogInformation("Student has been created a new account. Assigning role to them",
                                    new { studentId = newStudent.Id, username = newStudent.UserName });
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Student already exist... updating classlist.",
                                new { studentId = TempStudent.Id, username = TempStudent.UserName });

                            if (TempStudent.ClassList == null)
                            {
                                TempStudent.ClassList = new List<StudentClass>();
                            }
                            //var existingEntry = TempStudent.ClassList.FirstOrDefault(c => c.Class.Code == newClass.Code);

                            //if(existingEntry == null)
                            //{
                            var newStudentClass = new StudentClass
                            {
                                ClassId = newClass.Id,
                                StudentId = TempStudent.Id,
                                Class = newClass,
                                Student = TempStudent,
                            };
                            TempStudent.ClassList.Add(newStudentClass);

                            _context.Entry(newStudentClass).State = EntityState.Added;

                            _context.Attach(TempStudent);
                            _context.Update(TempStudent);
                            await _context.SaveChangesAsync();
                            //}
                            //else
                            //    _logger.LogInformation("Student's already in this class... aborting");
                        }
                    }

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
