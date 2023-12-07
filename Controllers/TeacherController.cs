using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;
using System.Security.Claims;
using Google.Apis.Drive.v3.Data;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        string teacherCode;
        string teacherImgId;
        static Teacher viewModel;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }
        
        public async Task<IActionResult> Index()
        {
            // Lấy thông tin người dùng đăng nhập
            var user = await _context.Teacher.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            

            // Kiểm tra xem người dùng có tồn tại không
            if (user != null)
            {
                //var email = user.Email;
                teacherCode = user.TeacherCode;
                teacherImgId = user.ImgId;

                viewModel = new Teacher
                {
                    TeacherCode = teacherCode,
                    ImgId = teacherImgId
                };

                return View(viewModel);
            }

            // Xử lý trường hợp không có người dùng đăng nhập
            return NotFound();
        }

        public IActionResult TeacherClass()
        {
            return View(viewModel);
        }
    }
}
