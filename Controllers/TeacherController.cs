using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TeacherController(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var teacher = await _context.Teacher.FirstOrDefaultAsync(); 
            string teacherCode = teacher?.TeacherCode;
            string teacherImgId = teacher?.ImgId;

            var viewModel = new Teacher
            {
                TeacherCode = teacherCode,
                ImgId = teacherImgId
            };

            return View(viewModel);
        }
        public IActionResult TeacherClass()
        {
            return View();
        }
    }
}
