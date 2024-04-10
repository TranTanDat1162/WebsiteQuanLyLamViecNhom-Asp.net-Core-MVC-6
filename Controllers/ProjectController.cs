using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Security.Claims;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Data.Migrations;
using WebsiteQuanLyLamViecNhom.HelperClasses;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using WebsiteQuanLyLamViecNhom.Models;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.GroupDTO;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.ProjectDTO;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.BaseApplicationUser> _userManager;

        static Teacher? viewModel = new Teacher
        {
            TeacherCode = "Teacher",
            ImgId = null
        };

        public ProjectController(ApplicationDbContext context, UserManager<Models.BaseApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        static Student? viewModelStudent = new Student
        {   
            StudentCode = "Student",
            Id = null 
        };

        static Teacher? viewModelTeacher = new Teacher
        {
            TeacherCode = "Teacher"
        };  

        //TO-DO:
        //https://stackoverflow.com/questions/37554536/ho-do-i-show-a-button-that-links-to-a-page-only-if-the-user-is-authorized-to-vie
        // Return View for Teacher
        [Route("Teacher/Project/{Class?}/{GroupId?}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherIndex(string Class, string GroupId)
        {
            try
            {
                viewModelTeacher = await _context.Teacher.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewData["Teacher"] = viewModelTeacher;
                //var result = await _context.Class.Where(t => t.Code == id).FirstOrDefaultAsync();
                Group? group = await _context.Group.Where(t => t.Id == GroupId)
                                    .Include(p => p.Project)
                                        .ThenInclude(c => c.Class)
                                    .Include(s => s.Students)
                                        .ThenInclude(sc => sc.Student)
                                    .Include(s => s.Students)
                                        .ThenInclude(t => t.Tasks)
                                    .FirstOrDefaultAsync();


                GroupDTO groupDTO = new();
                StudentClass? leader = group.Students.Where(l => l.Student.StudentCode == group.LeaderID)
                                            .FirstOrDefault();

                List<AssigneeReportViewModel> assigneeReports = new List<AssigneeReportViewModel>();
                group.Students.ToList().ForEach(s =>
                {
                    assigneeReports.Add(new AssigneeReportViewModel
                    {
                        Name = s.Student.LastName + " " + s.Student.FirstName,
                        Quantity = s.Tasks.Count(),
                    });
                });

                var currentGroup = new GroupDTO.GroupVM
                {
                    LeaderID = group.LeaderID,
                    MOTD = group.MOTD,
                    ProjectId = group.ProjectId,
                    ProjectName = group.Project.Name,
                    memberList = group.Students,
                    LeaderName = leader.Student.LastName + " " + leader.Student.FirstName,
                    CurrentClass = group.Project.Class,
                    CurrentUser = viewModelTeacher.Id,
                    ProjectAttachmentsJSON = group.Project.fileIDJSON,
                    GroupID = group.Id,
                    NumOfTaskPerMember = assigneeReports,
                };

                var taskList = await _context.Task
                        .Where(p => p.GroupId == GroupId)
                        .Include(sc => sc.StudentClass)
                        .ThenInclude(s => s.Student)
                        .ToListAsync();

                if (taskList.Count > 0)
                    currentGroup.Tasks = taskList;

                groupDTO.GroupViewModel = currentGroup;
                groupDTO.crumbs = new List<List<string>>()
                    {
                        new List<string>() { "/Teacher/Class", "Home" },
                        new List<string>() { "/Teacher/"+ Class, Class  },
                        new List<string>() { "/Teacher/Project/" + GroupId + "/" + GroupId, GroupId }
                    };
                return View(groupDTO);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        // Return View for Student
        [Route("Student/Project/{ClassCode?}/{GroupId?}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentIndex(string ClassCode, string GroupId)
        {
            viewModelStudent = await _context.Student.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["Student"] = viewModelStudent;

            Group? group = await _context.Group
                        .Where(t => t.Id == GroupId)
                        .Include(p => p.Project)
                            .ThenInclude(c => c.Class)
                        .Include(s => s.Students)
                            .ThenInclude(sc => sc.Student)
                        .FirstOrDefaultAsync();

            Class currentClass = await _context.Class
                        .Where(t => t.Code == ClassCode)
                        .FirstOrDefaultAsync();

            if (group == null /*&& currentClass.RoleGroup == RoleGroup.AssignByStudent*/)
            {
                // Tạo thêm trang tìm kiếm group
                return RedirectToAction("Error", "Student");

            }
/*            else
            {
                // Nếu group là null, chuyển hướng đến trang lỗi hoặc trang thông báo
                return RedirectToAction("Error", "Student");
            }*/

            GroupDTO groupDTO = new();
            StudentClass? leader = group?.Students.Where(l => l.Student.StudentCode == group.LeaderID)
                                        .FirstOrDefault();

            var currentGroup = new GroupVM
            {
                LeaderID = group.LeaderID,
                MOTD = group.MOTD,
                ProjectId = group.ProjectId,
                ProjectName = group.Project.Name,
                memberList = group.Students,
                LeaderName = leader?.Student.LastName + " " + leader?.Student.FirstName,
                CurrentClass = group.Project.Class,
                ProjectAttachmentsJSON = group.Project.fileIDJSON,
                GroupID = group.Id,
                Deadline = group.Project?.Deadline,
                CurrentUser = viewModelStudent.Id
            };

            var taskList = await _context.Task
                                         .Where(p => p.GroupId == group.Id)
                                         .Include(sc => sc.StudentClass)
                                         .ThenInclude(s => s.Student)
                                         .ToListAsync();
            if (taskList.Count > 0)
                currentGroup.Tasks = taskList;

            groupDTO.GroupViewModel = currentGroup;
            groupDTO.crumbs = new List<List<string>>()
                {
                    new List<string>() { "/Student", "Home" },
                    new List<string>() { "/Student/Project/" + ClassCode, ClassCode }
                };
            return View(groupDTO);

        }

        [Route("Student/Error")]
        public IActionResult Error()
        {
            return View();
        }

        //------------------Actions starts------------------->>

        public async Task<IActionResult> CreateTask(GroupDTO.TaskDTO createTaskDTO)
        {
            if (ModelState.IsValid)
            {

                List<StudentClass> memberList = new List<StudentClass>();

                foreach (var studentid in createTaskDTO.memberList)
                {
                    var member = await _context.StudentClass
                                        .Where(sc => sc.StudentId == studentid 
                                                && sc.GroupID == createTaskDTO.GroupID)
                                        .Include(s => s.Student)
                                        .Include(g => g.Group)
                                        .Include(t => t.Tasks)
                                        .Include(c => c.Class)
                                        .FirstOrDefaultAsync();

                    //Đề phòng thôi
                    if (member != null)
                    {
                        memberList.Add(member);
                    }
                }

                Models.Task newtask = new Models.Task()
                {
                    TaskName = createTaskDTO.TaskName,
                    StudentClass = memberList,
                    DeadLineDate = createTaskDTO.Deadline,
                    Description = createTaskDTO.Description,
                    Status = Models.TaskStatus.OnGoing,
                    Group = memberList.FirstOrDefault().Group
                };

                _context.Add(newtask);
                foreach(var member in memberList)
                {
                    member.Tasks.Add(newtask);
                }
                _context.UpdateRange(memberList);

                await _context.SaveChangesAsync();
                return RedirectToAction("StudentIndex", 
                    new { classCode = memberList.FirstOrDefault().Class.Code, GroupId= newtask.GroupId });
            }
            // TODO: Return errors
            return RedirectToAction("StudentIndex",
                new { Error = ModelState.ToString() });
        }

        public async Task<IActionResult> UpdateTask(GroupDTO.UpdateTaskDTO updateTaskDTO)
        {
            if (ModelState.IsValid)
            {

                var task = await _context.Task
                                        .Where(sc => sc.TaskId == updateTaskDTO.TaskID)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(s => s.Student)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(c => c.Class)
                                        .FirstOrDefaultAsync();

                List<StudentClass> memberList = task.StudentClass.ToList();

                //Only setting the status for now
                task.Description = updateTaskDTO.Description;
                task.Status = Models.TaskStatus.Pending;

                _context.UpdateRange(memberList);

                if (updateTaskDTO.Attachments != null)
                {
                    GDriveServices gDriveServices = new GDriveServices();
                    UploadHelper uploadHelper = new UploadHelper();

                    List<List<string>> uploadFiles = new List<List<string>>();

                    foreach (var attachment in updateTaskDTO.Attachments)
                    {
                        byte[] data = uploadHelper.ConvertToByteArray(attachment);

                        var fileID =
                        gDriveServices.UploadFile(task.TaskId + attachment.FileName, data, "1eY_PYFOhlkXoi76uwXgxfjWJrRo6YC_K");

                        var downloadlink = gDriveServices
                            .GetDownloadLink((string)(fileID?.GetType().GetProperty("FileId")?.GetValue(fileID)));

                        if (downloadlink != null)
                            uploadFiles.Add(new List<string> { downloadlink, attachment.FileName });
                    }
                    task.AttachmentLinksJson = JsonConvert.SerializeObject(uploadFiles);

                }

                await _context.SaveChangesAsync();
                return RedirectToAction("StudentIndex",
                    new { classCode = memberList.FirstOrDefault().Class.Code, GroupId = task.GroupId });
            }
            // TODO: Return errors
            return RedirectToAction("StudentIndex",
                new { Error = ModelState.ToString() });
        }

        public async Task<IActionResult> RedoTask(GroupDTO.UpdateTaskDTO updateTaskDTO)
        {
            if (ModelState.IsValid)
            {
                var task = await _context.Task
                                        .Where(sc => sc.TaskId == updateTaskDTO.TaskID)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(s => s.Student)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(c => c.Class)
                                        .FirstOrDefaultAsync();

                List<StudentClass> memberList = task.StudentClass.ToList();

                //Only setting the status for now
                task.Description = updateTaskDTO.Description;

                _context.UpdateRange(memberList);

                if (updateTaskDTO.Attachments != null)
                {
                    GDriveServices gDriveServices = new GDriveServices();
                    UploadHelper uploadHelper = new UploadHelper();

                    List<List<string>> uploadFiles = new List<List<string>>();

                    foreach (var attachment in updateTaskDTO.Attachments)
                    {
                        byte[] data = uploadHelper.ConvertToByteArray(attachment);

                        var fileID =
                        gDriveServices.UploadFile(task.TaskId + attachment.FileName, data, "1eY_PYFOhlkXoi76uwXgxfjWJrRo6YC_K");

                        var downloadlink = gDriveServices
                            .GetDownloadLink((string)(fileID?.GetType().GetProperty("FileId")?.GetValue(fileID)));

                        if (downloadlink != null)
                            uploadFiles.Add(new List<string> { downloadlink, attachment.FileName });
                    }
                    task.AttachmentLinksJson = JsonConvert.SerializeObject(uploadFiles);

                }

                await _context.SaveChangesAsync();
                return RedirectToAction("StudentIndex",
                    new { classCode = memberList.FirstOrDefault().Class.Code, GroupId = task.GroupId });
            }
            // TODO: Return errors
            return RedirectToAction("StudentIndex",
                new { Error = ModelState.ToString() });
        }

        public async Task<IActionResult> VerifyTask(GroupDTO.UpdateTaskDTO updateTaskDTO)
        {
            if (ModelState.IsValid)
            {

                var task = await _context.Task
                                        .Where(sc => sc.TaskId == updateTaskDTO.TaskID)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(s => s.Student)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(c => c.Class)
                                        .FirstOrDefaultAsync();

                List<StudentClass> memberList = task.StudentClass.ToList();

                //Only setting the status for now
                task.Description = updateTaskDTO.Description;
                task.TaskGradedByLeader = updateTaskDTO.Grade;
                task.Status = Models.TaskStatus.Complete;

                _context.UpdateRange(memberList);

                await _context.SaveChangesAsync();
                return RedirectToAction("StudentIndex",
                    new { classCode = memberList.FirstOrDefault().Class.Code, GroupId = task.GroupId });
            }
            // TODO: Return errors
            return RedirectToAction("StudentIndex",
                new { Error = ModelState.ToString() });
        }

        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> GradeGroup(GroupDTO.GradeGroupDTO gradeGroupDTO)
        {
            if (ModelState.IsValid)
            {
                float totalscore = (float)((gradeGroupDTO.TeacherGrade * 0.9) + (gradeGroupDTO.LeaderAGVGrade * 0.1));
                var memberList = await _context.Group.Where(id => id.Id == gradeGroupDTO.GroupID)
                                                .Include(s => s.Students)
                                                .ThenInclude(c => c.Class)
                                                .Select(s => s.Students)
                                                .FirstOrDefaultAsync();
                _context.UpdateRange(memberList);

                foreach (var member in memberList)
                {
                    member.Score = totalscore;
                }
                await _context.SaveChangesAsync();
                return RedirectToAction("TeacherIndex",
                    new { Class = memberList.FirstOrDefault().Class.Code, GroupId = memberList.FirstOrDefault().GroupID });
            }
            return View(gradeGroupDTO);
        }

        //------------------Actions ends--------------------->>

    }
}
