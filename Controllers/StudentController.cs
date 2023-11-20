using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class StudentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
