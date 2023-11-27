using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class TeacherController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult TeacherClassList()
        {
            return View();
        }
        public IActionResult TeacherClass()
        {
            //TO-DO: return class info
            return View();
        }
    }
}
