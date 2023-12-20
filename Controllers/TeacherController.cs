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
using NuGet.Versioning;
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
                Teacher? currentTeacher = await _context.Teacher
                    .Include(t => t.ClassList)
                    .FirstOrDefaultAsync(t => t.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                CreateClassDTO ClassList = new CreateClassDTO();
                ClassList.ClassListDTO = currentTeacher.ClassList;
                return View(ClassList);
            }
            // Xử lý trường hợp không có người dùng đăng nhập
            return NotFound();
        }
        [Route("Teacher/TeacherClass/{id}")]
        public async Task<IActionResult> TeacherClass(string id)
        {
            ViewData["Teacher"] = viewModel;
            var result = await _context.Project
                .Where(t => t.Class.Code == id)
                .ToListAsync();

            ProjectDTO ProjectDTO = new()
            {
                CurrentProjects = result,
                ClassID = result.ToArray().First().ClassId
            };
            return View(ProjectDTO);
        }
        [Route("Teacher/TeacherClass/CreateProject/{id?}")]
        public async Task<IActionResult> CreateProject(int id, ProjectDTO.CreateProjectDTO createProjectDTO)
        {
            var currentclass = await _context.Class
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {
                Project newProject = new Project
                {
                    Name = createProjectDTO.Name,
                    Requirements = createProjectDTO.Requirement,
                    Deadline = createProjectDTO.Deadline,
                    ClassId = id,
                    Class = currentclass
                };
                _context.Add(newProject);
                await _context.SaveChangesAsync();
                _logger.LogInformation("New project has been created {Project}",
                    new { projectId = newProject.Id, code = newProject.Name, Class = newProject.ClassId });
            }
            //TODO: create a dynamic error view
            //return View("~/Views/Shared/Error.cshtml");
            return RedirectToRoute(new { controller = "Teacher", action = "TeacherClass", id = currentclass.Code });
        }
        [Route("Teacher/TeacherClass/CreateGroup/{id?}")]
        public async Task<IActionResult> CreateGroup(int id, ProjectDTO.CreateProjectDTO createProjectDTO)
        {
            var currentclass = await _context.Class
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();
            if(ModelState.IsValid)
            {

            }
            return RedirectToRoute(new { controller = "Teacher", action = "TeacherClass", id = currentclass.Code });
        }
        public async Task<IActionResult> CreateClass(CreateClassDTO createClassDTO)
        {
            createClassDTO.classDTO.Students.RemoveAt(createClassDTO.classDTO.Students.Count - 1);
            //TODO: Still missing some condition and the condition below is rubbishly workable
            if (ModelState.IsValid)
            {
                //Try to fetch alreadly existed Class based on submitted class code
                Class? TempClass = await _context.Class.FirstOrDefaultAsync(e => e.Code == createClassDTO.classDTO.Code);
                if (TempClass == null)
                {
                    try
                    {
                        if (createClassDTO.classDTO.OpenDate == null)
                        {
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
                            Year = int.Parse(createClassDTO.classDTO.Year.Substring(0, 4)),
                            Semester = createClassDTO.classDTO.Semester,
                            TeacherId = viewModel.Id
                        };

                        _context.Add(newClass);
                        await _context.SaveChangesAsync();
                        _logger.LogInformation("Class has been created {Class}",
                            new { studentId = newClass.Id, code = newClass.Code });

                        //Loop each student in the List
                        foreach (var student in createClassDTO.classDTO.Students)
                        {
                            //Student? TempStudent = await _context.Student.FirstOrDefaultAsync(e => e.StudentCode == student.StudentCode);
                            //Try to fetch already existing student
                            Student? TempStudent = await _context.Student
                                        .Include(s => s.ClassList)
                                        .ThenInclude(c => c.Class)
                                        .FirstOrDefaultAsync(e => e.StudentCode == student.StudentCode);
                            //Create an account for student if not already exist
                            if (TempStudent == null)
                            {
                                //Create new student object based on the StudentDTO
                                Student newStudent = new Student
                                {
                                    StudentCode = student.StudentCode,
                                    FirstName = student.StudentFirstName,
                                    LastName = student.StudentLastName,
                                    DOB = (DateTime)student.DOB,
                                    UserName = student.StudentCode,
                                    IsLocked = true
                                };

                                //Create a StudentClass for containning that student info in that class
                                var newStudentClass = new StudentClass
                                {
                                    ClassId = newClass.Id,
                                    StudentId = newStudent.Id,
                                    Class = newClass,
                                    Student = newStudent,
                                };

                                if (newStudent.ClassList == null)
                                {
                                    newStudent.ClassList = new List<StudentClass>();
                                }

                                newStudent.ClassList.Add(newStudentClass);
                                //TODO: set a "randomly" generated id for studentId field. https://stackoverflow.com/questions/26967215/generate-seemingly-random-unique-numeric-id-in-sql-server
                                var result = await _userManager.CreateAsync(newStudent, newStudent.DOB.ToString("ddMMyyyy"));
                                if (result.Succeeded)
                                {
                                    await _userManager.AddToRoleAsync(newStudent, "Student");
                                    _logger.LogInformation("Student has been created a new account. Assigning role to them {Student}",
                                        new { studentId = newStudent.Id, username = newStudent.UserName });
                                }
                            }
                            else
                            {
                                //Create a StudentClass object for already existed student
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
                                    StudentId = TempStudent.Id,
                                    Class = newClass,
                                    Student = TempStudent,
                                };

                                TempStudent.ClassList.Add(newStudentClass);
                                _context.Add(newStudentClass);
                                await _userManager.UpdateAsync(TempStudent);
                                //}
                                //else
                                //    _logger.LogInformation("Student's already in this class... aborting");
                            }
                        }
                        await _context.SaveChangesAsync();

                        return RedirectToAction("Index", "Teacher");

                    }
                    catch (Exception ex)
                    {
                        RedirectToAction("Index", "Teacher", ex.Message);
                    }
                }
                return RedirectToAction("Index", "Teacher");
            }
            return RedirectToAction("Index", "Teacher", ModelState);
        }
    }
}
