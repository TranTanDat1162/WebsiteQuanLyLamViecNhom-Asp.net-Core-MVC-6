using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class ProjectController : Controller
    {
        //TO-DO:
        //https://stackoverflow.com/questions/37554536/ho-do-i-show-a-button-that-links-to-a-page-only-if-the-user-is-authorized-to-vie
        // Return View for Teacher
        public IActionResult Index()
        {
            return View();
        }

        // Return View for Student
        public IActionResult Student()
        {
            return View();
        }
    }
}
