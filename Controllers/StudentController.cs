using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.CreateClassDTO;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.BaseApplicationUser> _userManager;
        private readonly ILogger<StudentController> _logger;

        static Student? viewModel = new Student
        {
            StudentCode = "Student"
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

                //Teacher? currentTeacher = await _context.Teacher
                //    .Include(t => t.ClassList)
                //    .ThenInclude(t => t.ClassGroup)
                //    .FirstOrDefaultAsync(t => t.Id == User.FindFirst(ClaimTypes.NameIdentifier));

                CreateClassDTO ClassList = new CreateClassDTO();
                foreach(var studentClass in currentStudent.ClassList)
                {
                    ClassList.ClassListDTO.Add(studentClass.Class);
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
                StudentProfile = new CreateClassDTO();

                Student? currentStudent = await _context.Student
                    .Include(t => t.ClassList)
                        .ThenInclude(t => t.Class)
                            .ThenInclude(t => t.Teacher)
                    .FirstOrDefaultAsync(t => t.Id == User.FindFirstValue(ClaimTypes.NameIdentifier));

                if (currentStudent != null)
                {
                    var currentClasses = await _context.StudentClass
                        .Where(s => s.StudentId == currentStudent.Id)
                        .Include(t => t.Class)
                        .ToListAsync();

                    //I dont think this is useful here,
                    //I think you should relocate this studentDTO to a different area
                    //so its not heavily dependent on the createClassDTO

                    foreach (var studentClass in currentClasses)
                    {
                        StudentProfile.StudentClassListDTO.Add(studentClass);
                    }

                    // Update breadcrumbs for the "/Student/Profile" route
                    StudentProfile.crumbs = new List<List<string>>()
                    {
                        new List<string>() { "/Student", "Home" },
                        new List<string>() { "/Student/Profile", "Profile" }  // Updated breadcrumb
                    };

                    return View(StudentProfile);
                }

                return View(StudentProfile);
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

                    bool isEmailUnique = await _userManager.FindByEmailAsync(studentDTO.Email) == null;

                    // Kiểm tra xem người dùng có tồn tại không
                    if (loggedInStudent != null && isEmailUnique)
                    {
                        // Cập nhật email cho người dùng
                        loggedInStudent.Email = studentDTO.Email;

                        // Chuẩn hóa và cập nhật NormalizedEmail
                        loggedInStudent.NormalizedEmail = studentDTO.Email?.ToUpperInvariant();

                        // Nếu người dùng đã chọn ảnh mới
                        if (studentDTO.StudentImgPfp != null)
                        {
                            GDriveServices gDriveServices = new GDriveServices();
                            UploadHelper uploadHelper = new UploadHelper();

                            byte[] data = uploadHelper.ConvertToByteArray(studentDTO.StudentImgPfp);
                            var fileID = gDriveServices.UploadFile(loggedInUserId, data, "1n680aa3fmW9qkZwrd7A1C5k0nf7DhkeP");

                            loggedInStudent.StudentImgId = (string)(fileID?.GetType().GetProperty("FileId")?.GetValue(fileID));
                        }

                        if(studentDTO.Email != null && !loggedInStudent.EmailConfirmed)
                        {
                            //Sử dụng các phương thức của microsoft:
                            //Generate cái token (code) và link để chứa cái token đó để gửi qua cha ng dùng
                            string returnUrl = null ?? Url.Content("~/");

                            var code = await _userManager.GenerateEmailConfirmationTokenAsync(loggedInStudent);

                            string EmailConfirmationUrl = Url.Page(
                                "/Account/ConfirmEmail",
                                pageHandler: null,
                                values: new { area = "Identity", userId = loggedInUserId, code, returnUrl },
                                protocol: Request.Scheme);

                            await _emailSender.SendEmailAsync(loggedInStudent.Email, "Xác nhận tài khoản", EmailConfirmationUrl);
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
            viewModel = await _context.Student.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            if (viewModel != null)
            {
                ViewData["Student"] = viewModel; // Lấy info student trước để đưa vào view
            }

            if (!ModelState.IsValid)
            {
                return View("Profile", StudentProfile); // Trả về view Profile với một đối tượng StudentProfile được tạo ở trc đó
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
                changePasswordDTO.IsCurrentPasswordValid = false;
                return View("Profile", new CreateClassDTO()); // Trả về view Profile với một đối tượng CreateClassDTO mới
            }
            else
            {
                changePasswordDTO.IsCurrentPasswordValid = true;
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

        [HttpPost]
        public async Task<IActionResult> CheckCurrentPassword(string currentPassword)
        {
            // Lấy thông tin người dùng đăng nhập
            var loggedInUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(loggedInUserId);

            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{loggedInUserId}'.");
            }

            // Kiểm tra mật khẩu hiện tại của người dùng
            var passwordCheckResult = await _userManager.CheckPasswordAsync(user, currentPassword);
            return Json(new { isValid = passwordCheckResult });
        }


        public IActionResult ProjectDetail()
        {
            return View();
        }
    }
}
