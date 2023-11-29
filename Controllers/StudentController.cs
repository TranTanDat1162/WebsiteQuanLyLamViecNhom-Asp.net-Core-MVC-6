using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ClassList()
        {
            return View();
        }
        public IActionResult ProjectDetail()
        {
            return View();
        }
    }
}
