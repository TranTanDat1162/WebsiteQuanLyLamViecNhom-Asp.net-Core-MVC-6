using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.CreateClassDTO;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.BaseApplicationUser> _userManager;
        private readonly ILogger<StudentController> _logger;

        static Student? viewModel = new Student
        {
            StudentCode = "Student"
        };

        public StudentController(ApplicationDbContext context, UserManager<Models.BaseApplicationUser> userManager, ILogger<StudentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }

        [Route("Student")]
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng đăng nhập
            viewModel = await _context.Student.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));

            // Kiểm tra xem người dùng có tồn tại không
            if (viewModel != null)
            {
                ViewData["Student"] = viewModel;
                Student? currentStudent = await _context.Student
                    .Include(t => t.ClassList)
                    .ThenInclude(t => t.Class)
                    .FirstOrDefaultAsync(t => t.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));
                CreateClassDTO ClassList = new CreateClassDTO();
                foreach(var studentClass in currentStudent.ClassList)
                {
                    ClassList.ClassListDTO.Add(studentClass.Class);
                }
                return View(ClassList);
            }

            // Xử lý trường hợp không có người dùng đăng nhập
            return NotFound();
        }

        public IActionResult ProjectDetail()
        {
            return View();
        }
    }
}
