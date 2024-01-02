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
using Microsoft.AspNetCore.Authorization;
using WebsiteQuanLyLamViecNhom.HelperClasses;
using Newtonsoft.Json;
using System.Net.Mail;
using Microsoft.AspNetCore.SignalR.Client;
namespace WebsiteQuanLyLamViecNhom.Controllers
{
    [Authorize(Roles = "Teacher")]
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        // Retrieve the Identity of user
        private readonly UserManager<Models.BaseApplicationUser> _userManager;
        private readonly ILogger<TeacherController> _logger;

        static Teacher? viewModel = new Teacher
        {
            TeacherCode = "Teacher",
            ImgId = null,
            FirstName = null,
            LastName =null
        };


        public TeacherController(ApplicationDbContext context, UserManager<Models.BaseApplicationUser> userManager, ILogger<TeacherController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [Route("Teacher/Class")]
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng đăng nhập
            // Teacher có public ICollection<Class>? ClassList { get; set; } là kiểu phức tạp
            // Nên phải .Include(t => t.ClassList) thì mới lấy được danh sách lớp
            viewModel = await _context.Teacher
                            .Include(t => t.ClassList)
                            .FirstOrDefaultAsync(t => t.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
            // Kiểm tra xem người dùng có tồn tại không
            if (viewModel != null)
            {
                ViewData["Teacher"] = viewModel;
                CreateClassDTO ClassList = new CreateClassDTO();
                ClassList.ClassListDTO = viewModel.ClassList;
                return View(ClassList);
            }
            // Xử lý trường hợp không có người dùng đăng nhập
            return NotFound();
        }

        [HttpGet("Teacher/{classCode}")]
        public async Task<IActionResult> TeacherClass(string classCode)
        {
            ViewData["Teacher"] = viewModel;

            var projectList = await _context.Project
                .Where(t => t.Class.Code == classCode)
                .Include(c => c.Class)
                .ToListAsync();

            var studentList = await _context.StudentClass
                .Where(s => s.Class.Code == classCode)
                .Include(l => l.Student)
                .ToListAsync();

            var groupList = await _context.Group
                .Where(t => t.Project.Class.Code == classCode)
                .ToListAsync();

            ProjectDTO ProjectDTO = new()
            {
                TeacherName = viewModel.LastName + " " + viewModel.FirstName,
                TeacherId = projectList.First().Class.TeacherId,
                CurrentGroups = groupList,
                CurrentProjects = projectList,
                ClassID = studentList.ToArray().First().ClassId,
                StudentList = studentList                
            };
                
            return View(ProjectDTO);
        }

        //------------------Actions starts------------------->>

        /// <summary>
        /// Tạo project dựa vào createProjectDTO
        /// </summary>
        /// <param name="id"></param> khoá chính của Lớp
        /// <param name="createProjectDTO"></param> gồm: Name, Requirements, Deadline, ClassId, Class
        /// <returns></returns>
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
            return RedirectToRoute(new { controller = "Teacher", action = currentclass.Code});
        }

        /// <summary>
        /// Cập nhất thông tin và tải tệp đính kèm của project lên
        /// </summary>
        /// <param name="id"></param> Khoá chính của Project
        /// <param name="updateProjectDTO"></param> gồm: Name, Requirements, Deadline, Attachments
        /// <returns></returns>
        public async Task<IActionResult> UpdateProject(int id, ProjectDTO.UpdateProjectDTO updateProjectDTO)
        {
            var currentProject = await _context.Project
                .Where(t => t.Id == id)
                .Include(c => c.Class)
                .FirstOrDefaultAsync();

            if (currentProject != null && ModelState.IsValid)
            {

                currentProject.Name = updateProjectDTO.Name;
                currentProject.Deadline = updateProjectDTO.Deadline;
                currentProject.Requirements = updateProjectDTO.Requirement;

                if (updateProjectDTO.Attachments != null)
                {
                    GDriveServices gDriveServices = new GDriveServices();
                    UploadHelper uploadHelper = new UploadHelper();

                    List<List<string>> uploadFiles = new List<List<string>>();

                    foreach(var attachment in updateProjectDTO.Attachments)
                    {
                        byte[] data = uploadHelper.ConvertToByteArray(attachment);

                        var fileID =
                        gDriveServices.UploadFile(currentProject.Class.Code+attachment.FileName, data, "1HN1IZIiLErNA_JX3ze2DC8lUvWmGWg-T");

                        var downloadlink = gDriveServices
                            .GetDownloadLink((string)(fileID?.GetType().GetProperty("FileId")?.GetValue(fileID)));

                        if(downloadlink != null)
                            uploadFiles.Add(new List<string> { downloadlink, attachment.FileName });
                    }
                    currentProject.fileIDJSON = JsonConvert.SerializeObject(uploadFiles);

                }
                _context.Update(currentProject);
                await _context.SaveChangesAsync();
                _logger.LogInformation("New project has been created {Project}",
                    new { projectId = currentProject.Id, code = currentProject.Name, Class = currentProject.ClassId });
            }
            //TODO: create a dynamic error view
            //return View("~/Views/Shared/Error.cshtml");
                return RedirectToAction("TeacherClass", new { classCode = currentProject.Class.Code });
        }

        /// <summary>
        /// Lấy tt của group được gửi lên r gửi lên database
        /// </summary>
        /// <param name="id"></param> id (index) của lớp nắm group đó
        /// <param name="createGroupDTO"></param> tt của group đc post lên
        /// <returns></returns>
        public async Task<IActionResult> CreateGroup(int id, ProjectDTO.CreateGroupDTO createGroupDTO)
        {
            var currentclass = await _context.Class
                .Where(t => t.Id == id)
                .FirstOrDefaultAsync();

            var selectedProject = await _context.Project
                .Where(p => p.Id == createGroupDTO.ProjectId)
                .FirstOrDefaultAsync();
            if (ModelState.IsValid)
            {
                //Tạo Group mới dựa vào createGroupDTO
                Group newGroup = new Group
                {
                    MOTD = createGroupDTO.MOTD,
                    Project = selectedProject,
                    LeaderID = createGroupDTO.LeaderID
                };

                //Lấy các StudentCLass từ dãy string trong createGroupDTO.memberList
                List<StudentClass> memberList = new List<StudentClass>();
                foreach (var studentid in createGroupDTO.memberList)
                {
                    var member = await _context.StudentClass
                                        .Where(sc => sc.StudentId == studentid)
                                        .Include(s => s.Student)
                                        .Include(c => c.Class)
                                        .FirstOrDefaultAsync();

                    //Check nếu id này là leader thì add vô group
                    if(studentid == createGroupDTO.LeaderID)
                        newGroup.LeaderID = studentid;

                    //Đề phòng thôi
                    if(member != null)
                    {
                        member.Group = newGroup;
                        memberList.Add(member);
                    }
                }

                //Bỏ List StudentClass vừa lấy đc vào cái newGroup mới tạo rồi add vào context
                if(memberList.Count > 0)
                {
                    newGroup.Students = memberList;
                    _context.Add(newGroup);
                    await _context.SaveChangesAsync();
                    return  RedirectToAction("TeacherClass", new { classCode = currentclass.Code });
                }
                //TODO: Trả về báo lỗi thiếu info
                return RedirectToAction("TeacherClass", new { classCode = currentclass.Code });

            }
            //TODO: Trả về báo lỗi sai cú pháp
            return RedirectToAction("TeacherClass", new { classCode = currentclass.Code });
        }

        /// <summary>
        /// API xử lý việc chỉ trả lại những sinh viên được 
        /// chọn làm thành viên của group để cho ô dropdown
        /// chọn leader của nhóm
        /// </summary>
        /// <param name="selectedStudentIds"></param> Id của những student được chọn
        /// <returns></returns>
        public async Task<IActionResult> GetDependentOptions(List<string> selectedStudentIds)
        {
            var dependentOptions = new List<object>();
            foreach (var studentid in selectedStudentIds)
            {
                var fetchedStudent = await _context.Student
                                        .Where(s => s.Id == studentid)
                                        .FirstOrDefaultAsync();
                var option = new
                {
                    value = fetchedStudent.StudentCode,
                    text = (fetchedStudent.FirstName +" "+ fetchedStudent.LastName)
                };
                dependentOptions.Add(option);

            }
            return Json(new { data = dependentOptions });
        }

        /// <summary>
        /// Tạo Lớp dựa vào createCLassDTO
        /// Tạo tài khoản sinh viên của lớp đó
        /// </summary>
        /// <param name="createClassDTO"></param> thông tin dựa vào Excel
        /// <returns></returns>
        public async Task<IActionResult> CreateClass(CreateClassDTO createClassDTO)
        {
            // Vấn đề trong javascript khi import lớp (giả sử 42 đứa nhưng đứa 43 là rỗng)
            // Nên thêm dòng này
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

                                //var existingEntry = TempStudent.ClassList.FirstOrDefault(c => c.Class.Code == newClass.Code);

                                //if(existingEntry == null)
                                //{
                                var newStudentClass = new StudentClass
                                {
                                    //StudentId = TempStudent.Id,
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
                        TempData["ErrorMessage"] = "Đã xảy ra lỗi trong quá trình tạo lớp.";
                        _logger.LogError(ex, "Error creating class");
                        return RedirectToAction("Error", "Home");
                    }
                }
                else
                {
                    // Lớp đã tồn tại, thêm thông báo vào TempData
                    TempData["ErrorMessage"] = "Lớp đã tồn tại.";
                    return RedirectToAction("Index", "Teacher");
                }
            }
            return RedirectToAction("Index", "Teacher", ModelState);
        }

        public async Task<IActionResult> UpdateClass()
        {
            return View();
        }

        public async Task<IActionResult> VerifyProject()
        {
            return View();
        }

        //------------------Actions ends------------------->>

    }
}
