using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BaseApplicationUser> _userManager;

        static Teacher? viewModel = new Teacher
        {
            TeacherCode = "Teacher",
            ImgId = null
        };

        public ProjectController(ApplicationDbContext context, UserManager<BaseApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        static Student? viewModelStudent = new Student
        {
            StudentCode = "Student"
        };

        static Teacher? viewModelTeacher = new Teacher
        {
            TeacherCode = "Teacher"
        };

        //TO-DO:
        //https://stackoverflow.com/questions/37554536/ho-do-i-show-a-button-that-links-to-a-page-only-if-the-user-is-authorized-to-vie
        // Return View for Teacher
        [Route("Teacher/{id?}/{GroupId?}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherIndex(string id, string GroupId)
        {
            try
            {
                viewModelTeacher = await _context.Teacher.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewData["Teacher"] = viewModelTeacher;
                //var result = await _context.Class.Where(t => t.Code == id).FirstOrDefaultAsync();
                Group? group = await _context.Group.Where(t => t.Id == GroupId)
                                    .Include(p => p.Project)
                                    .Include(s => s.Students)
                                    .ThenInclude(sc => sc.Student)
                                    .FirstOrDefaultAsync();
                GroupDTO groupDTO = new();
                StudentClass? leader = group.Students.Where(l => l.Student.StudentCode == group.LeaderID)
                                            .FirstOrDefault();

                var currentGroup = new GroupDTO.GroupVM
                {
                    LeaderID = group.LeaderID,
                    MOTD = group.MOTD,
                    ProjectId = group.ProjectId,
                    ProjectName = group.Project.Name,
                    memberList = group.Students,
                    LeaderName = leader.Student.LastName + " " + leader.Student.FirstName
            };
                var taskList = await _context.Task
                                             .Where(p => p.GroupId == GroupId)
                                             .ToListAsync();
                if (taskList.Count > 0)
                    currentGroup.Tasks = taskList;

                groupDTO.GroupViewModel = currentGroup;

                return View(groupDTO);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        // Return View for Student
        [Route("Project/Student/{id?}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentIndex(string id)
        {
            viewModelStudent = await _context.Student.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["Student"] = viewModelStudent;
            var result = await _context.Class.Where(t => t.Code == id).FirstOrDefaultAsync();

            return View("~/Views/Project/Student.cshtml", result);
        }

        //------------------Actions starts------------------->>


        public async Task<IActionResult> CreateTask(Models.Task task)
        {
            return View();
        }


        //------------------Actions ends--------------------->>

    }
}
