using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
    }
}
