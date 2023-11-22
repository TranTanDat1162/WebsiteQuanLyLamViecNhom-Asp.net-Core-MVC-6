using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class AccountController : Controller
    {
        // Giao diện đăng nhập
        public IActionResult Login()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            //return RedirectToPage("/Account/Login", new { area = "Identity" });
            return View();
        }
        // Giao diện nhập email để lấy mật khẩu
        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
        // Giao diện khi mail gửi thành công
        public IActionResult MailConfirm()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
        // Giao diện đổi lại mật khẩu mới
        public IActionResult ResetPassword()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
        // Giao diện đổi mật khẩu thành công
        public IActionResult PasswordConfirm()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
    }
}
