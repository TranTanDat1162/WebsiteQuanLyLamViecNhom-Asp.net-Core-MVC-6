using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;
using Task = System.Threading.Tasks.Task;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class AdminStudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BaseApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;
        public AdminStudentController(ApplicationDbContext context, UserManager<BaseApplicationUser> usermanager, ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = usermanager;
            _logger = logger;
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult ProjectDetail()
        {
            return View();
        }
        public async Task OnPostAsync(Student student)
        {
            if (!ModelState.IsValid)
            {
                var user = student;

                //await _userStore.SetUserNameAsync(user, teacher.Email, CancellationToken.None);
                //await _emailStore.SetEmailAsync(user, teacher.Email, CancellationToken.None);
                user.UserName = student.StudentCode;

                var result = await _userManager.CreateAsync(user, student.DOB.ToString("ddMMyyyy"));

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "Student");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogInformation("Problems here!.");
                }
            }
        }
        // STUDENT
        // GET: Student
        [Route("Admin/Student")]
        public async Task<IActionResult> StudentList()
        {
            var studentList = await _context.Student.ToListAsync();
            return _context.Teacher != null ?
                        View("~/Views/Admin/StudentAccounts/Index.cshtml", studentList) :
                        Problem("Entity set 'ApplicationDbContext.Student'  is null.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Student/Lock/{id?}")]
        public async Task<IActionResult?> StudentLock(string id, bool isLocked)
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            //var teacher = await _context.Teacher.FindAsync(id);
            var student = await _context.Student.Where(x => x.StudentCode.Contains(id)).FirstOrDefaultAsync();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (student == null)
            {
                return NotFound();
            }

            student.IsLocked = isLocked;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(student);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StudentExist(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //System.Diagnostics.Debug.WriteLine("Teacher "+id+" new IsLocked value " + isLocked);
                return RedirectToAction("Student", "Admin"); ;
            }
            //return View("~/Views/Admin/StudentAccounts/Edit.cshtml", student);
            return null;
        }
        [Route("Admin/Student/Create")]
        public IActionResult StudentCreate()
        {
            return View("~/Views/Admin/StudentAccounts/Create.cshtml");
        }
        [Route("Admin/Student/Create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StudentCreate([Bind("StudentId,FirstName,LastName,Email,DOB,IsLocked")] Student student)
        {
            student.StudentCode = "205051051";

            if (!ModelState.IsValid)
            {
                try
                {
                    await OnPostAsync(student);
                }
                catch
                {
                    _logger.LogInformation("Student created with problems.", student.StudentCode);
                }
                //_context.Add(student);
                //await _context.SaveChangesAsync();
            }
            return RedirectToAction("StudentList", "Student");
        }
        private bool StudentExist(int? id)
        {
            return (_context.Student?.Any(e => e.StudentId == id)).GetValueOrDefault();
        }
    }
}
