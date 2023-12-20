using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BaseApplicationUser> _userManager;

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

        public async Task<IActionResult> Index(string id)
        {
            return View();
        }
        //TO-DO:
        //https://stackoverflow.com/questions/37554536/ho-do-i-show-a-button-that-links-to-a-page-only-if-the-user-is-authorized-to-vie
        // Return View for Teacher
        [Route("Project/Teacher/{id?}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherIndex(string id)
        {
            viewModelTeacher = await _context.Teacher.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["Teacher"] = viewModelTeacher;
            var result = await _context.Class.Where(t => t.Code == id).FirstOrDefaultAsync();
            
            return View("~/Views/Project/Index.cshtml",result);
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
    }
}
