using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.CreateClassDTO;
using WebsiteQuanLyLamViecNhom.Models;
using WebsiteQuanLyLamViecNhom.Data.Migrations;
using WebsiteQuanLyLamViecNhom.HelperClasses;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.BaseApplicationUser> _userManager;
        private readonly ILogger<StudentController> _logger;

        static Student? viewModel = new Student
        {
            StudentCode = "Student",
            FirstName = null,
            LastName = null,
            DOB = DateTime.MinValue,
            Email = null,
            StudentImgId = null
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
                            .ThenInclude(t => t.Teacher)
                    .FirstOrDefaultAsync(t => t.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                CreateClassDTO ClassList = new CreateClassDTO();
                if (currentStudent != null) {
                    var currentClasses = await _context.StudentClass
                        .Where(s => s.StudentId == currentStudent.Id)
                        .Include(t => t.Class)
                        .ToListAsync();

                    foreach (var studentClass in currentClasses)
                    {
                        ClassList.StudentClassListDTO.Add(studentClass);
                    }
                    ClassList.crumbs = new List<List<string>>()
                    {
                        new List<string>() { "/Student", "Home" }
                    };
                    return View(ClassList);

                }

                return View(ClassList);
            }

            // Xử lý trường hợp không có người dùng đăng nhập
            return NotFound();
        }

        [Route("Student/Profile")]
        public async Task<IActionResult> Profile()
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
                            .ThenInclude(t => t.Teacher)
                    .FirstOrDefaultAsync(t => t.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                CreateClassDTO ClassList = new CreateClassDTO();
                if (currentStudent != null)
                {
                    var currentClasses = await _context.StudentClass
                        .Where(s => s.StudentId == currentStudent.Id)
                        .Include(t => t.Class)
                        .ToListAsync();

                    foreach (var studentClass in currentClasses)
                    {
                        ClassList.StudentClassListDTO.Add(studentClass);
                    }

                    // Update breadcrumbs for the "/Student/Profile" route
                    ClassList.crumbs = new List<List<string>>()
                    {
                        new List<string>() { "/Student", "Home" },
                        new List<string>() { "/Student/Profile", "Profile" }  // Updated breadcrumb
                    };

                    return View(ClassList);
                }

                return View(ClassList);
            }

            // Xử lý trường hợp không có người dùng đăng nhập
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateProfile(CreateClassDTO.StudentDTO studentDTO)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy thông tin người dùng đăng nhập
                    var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var loggedInStudent = await _context.Student.FindAsync(loggedInUserId);

                    // Kiểm tra xem người dùng có tồn tại không
                    if (loggedInStudent != null)
                    {
                        // Cập nhật email và hình ảnh cho người dùng
                        loggedInStudent.Email = studentDTO.Email;

                        // Nếu người dùng đã chọn ảnh mới
                        if (studentDTO.StudentImgPfp != null)
                        {
                            GDriveServices gDriveServices = new GDriveServices();
                            UploadHelper uploadHelper = new UploadHelper();

                            byte[] data = uploadHelper.ConvertToByteArray(studentDTO.StudentImgPfp);
                            var fileID = gDriveServices.UploadFile(loggedInUserId, data, "1n680aa3fmW9qkZwrd7A1C5k0nf7DhkeP");

                            loggedInStudent.StudentImgId = (string)(fileID?.GetType().GetProperty("FileId")?.GetValue(fileID));
                        }

                        // Lưu thay đổi vào cơ sở dữ liệu
                        await _context.SaveChangesAsync();

                        // Chuyển hướng về trang profile sau khi cập nhật thành công
                        return RedirectToAction("Profile", "Student");
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Xử lý ngoại lệ khi có lỗi cập nhật cơ sở dữ liệu
                    ModelState.AddModelError("", "Có lỗi xảy ra khi cập nhật thông tin. Vui lòng thử lại.");
                }
            }

            // Nếu ModelState không hợp lệ, trả về trang cập nhật với thông báo lỗi
            return View("Profile", studentDTO);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdatePassword(CreateClassDTO.ChangePasswordDTO changePasswordDTO)
        {
            if (!ModelState.IsValid)
            {
                return View("Profile", new CreateClassDTO()); // Trả về view Profile với một đối tượng CreateClassDTO mới
            }

            // Lấy thông tin người dùng đăng nhập
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(loggedInUserId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{loggedInUserId}'.");
            }

            // Kiểm tra mật khẩu hiện tại của người dùng
            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, changePasswordDTO.CurrentPassword);
            if (!passwordCheckResult)
            {
                ModelState.AddModelError(string.Empty, "Mật khẩu hiện tại không đúng.");
                return View("Profile", new CreateClassDTO()); // Trả về view Profile với một đối tượng CreateClassDTO mới
            }

            // Thay đổi mật khẩu
            var changePasswordResult = await _userManager.ChangePasswordAsync(user, changePasswordDTO.CurrentPassword, changePasswordDTO.NewPassword);
            if (!changePasswordResult.Succeeded)
            {
                foreach (var error in changePasswordResult.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                return View("Profile", new CreateClassDTO()); // Trả về view Profile với một đối tượng CreateClassDTO mới
            }

            // Mật khẩu đã được thay đổi thành công, bạn có thể thực hiện các hành động khác ở đây

            // Chuyển hướng về trang profile sau khi thay đổi mật khẩu thành công
            return RedirectToAction("Profile");
        }

        public IActionResult ProjectDetail()
        {
            return View();
        }

    }
}
