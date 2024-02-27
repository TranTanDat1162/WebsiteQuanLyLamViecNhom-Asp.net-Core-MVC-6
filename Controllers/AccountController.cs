using Microsoft.AspNetCore.Mvc;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class AccountController : Controller
    {
        // Giao diện đăng nhập
        public IActionResult Index()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return RedirectToPage("/Account/Login", new { area = "Identity" });
        }
        // Giao diện nhập email để lấy mật khẩu
        public IActionResult ForgotPassword()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return RedirectToPage("/Account/ForgotPassword", new { area = "Identity" });
        }
        // Giao diện khi mail gửi thành công
        public IActionResult MailConfirm()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
        // Giao diện đổi lại mật khẩu mới
        public IActionResult ResetPassword(string UserId, string code)
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return RedirectToPage("/Account/ResetPassword", new { area = "Identity", UserId, code });
        }
        // Giao diện đổi mật khẩu thành công
        public IActionResult PasswordConfirm()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
    }
}
