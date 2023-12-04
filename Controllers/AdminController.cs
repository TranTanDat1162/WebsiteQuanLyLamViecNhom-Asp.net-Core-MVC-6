using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses;
using WebsiteQuanLyLamViecNhom.Models;
using Diacritics.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.WebUtilities;
using System.Text.Encodings.Web;
using System.Text;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    //[Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BaseApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;
        private readonly SignInManager<BaseApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserStore<BaseApplicationUser> _userStore;
        private readonly IUserEmailStore<BaseApplicationUser> _emailStore;

        public AdminController(ApplicationDbContext context, UserManager<BaseApplicationUser> usermanager, ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = usermanager;
            _logger = logger;
        }
        public IActionResult Index()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
        public IActionResult AdminClass()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
        //public IActionResult StudentList()
        //{
        //    ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
        //    return View();
        //}
        //---------------------------------------------
        public async Task<IActionResult> OnPostAsync(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                var user = teacher;

                //await _userStore.SetUserNameAsync(user, teacher.Email, CancellationToken.None);
                //await _emailStore.SetEmailAsync(user, teacher.Email, CancellationToken.None);
                user.UserName = teacher.Email;

                var result = await _userManager.CreateAsync(user, teacher.TeacherCode);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "Teacher");
                    return RedirectToAction("LecturerList", "Admin");

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogInformation("Problems here!." + ModelState.ToString());
                }
            }
            // If we got this far, something failed, redisplay form
            return RedirectToAction("Index", "Admin");
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

        //private IdentityUser CreateUser()
        //{
        //    try
        //    {
        //        return Activator.CreateInstance<Teacher>();
        //    }
        //    catch
        //    {
        //        throw new InvalidOperationException($"Can't create an instance of '{nameof(IdentityUser)}'. " +
        //            $"Ensure that '{nameof(IdentityUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
        //            $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
        //    }
        //}

        //----------------------------------------//
        // GET: Admin
        public async Task<IActionResult> AdminList()
        {
            return _context.Admin != null ?
                        View("~/Views/Admin/Action/Index.cshtml", await _context.Admin.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Admin'  is null.");
        }

        // GET: Admin/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // GET: Admin/Create
        public IActionResult Create()
        {
            return View("~/Views/Admin/Action/Create.cshtml");
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Password")] Admin admin)
        {
            if (ModelState.IsValid)
            {
                _context.Add(admin);
                await _context.SaveChangesAsync();
                return RedirectToAction("AdminList", "Admin");
            }
            return View("~/Views/Admin/Action/Index.cshtml", admin);
        }

        // GET: Admin/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin.FindAsync(id);
            if (admin == null)
            {
                return NotFound();
            }
            return View(admin);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Password")] Admin admin)
        {
            if (id != admin.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(admin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!AdminExists(admin.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(admin);
        }

        // GET: Admin/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Admin == null)
            {
                return NotFound();
            }

            var admin = await _context.Admin
                .FirstOrDefaultAsync(m => m.Id == id);
            if (admin == null)
            {
                return NotFound();
            }

            return View(admin);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Admin == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Admin'  is null.");
            }
            var admin = await _context.Admin.FindAsync(id);
            if (admin != null)
            {
                _context.Admin.Remove(admin);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AdminExists(int id)
        {
            return (_context.Admin?.Any(e => e.Id == id)).GetValueOrDefault();
        }

        //----------------------------------------//

        // GET: Lecturer
        public async Task<IActionResult> LecturerList()
        {
            var teacherDynamicModels = new TeacherDynamicModels
            {
                Teachers = await _context.Teacher.ToListAsync(),
                Teacher = null
            };
            return _context.Teacher != null ?
                        View("~/Views/Admin/Lecturer/Index.cshtml", teacherDynamicModels) :
                        Problem("Entity set 'ApplicationDbContext.Teacher'  is null.");
        }

        // GET: Lecturer/Details/5
        public async Task<IActionResult> LecturerDetails(int? id)
        {
            if (id == null || _context.Teacher == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Lecturer/Details.cshtml", teacher);
        }

        // GET: Lecturer/Create
        public IActionResult LecturerCreate()
        {
            return View("~/Views/Admin/Lecturer/Create.cshtml");
        }

        // POST: Lecturer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LecturerCreate([Bind("TeacherId,FirstName,LastName,Email,DOB,ImgPfp,IsLocked,TeacherCode")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                //"autogenlastname":
                //  taking the only the first letter of each words in Teacher's Last name 
                string autogenlastname = string.Concat(teacher.LastName.Split(' ').Select(s => s[0]));

                //"autogenTeacherId":
                //  using the Removediacritic to turn vietnamese alphabet into non-accent alphabet
                string autogenTeacherId = (teacher.FirstName.ToUpper() +
                    autogenlastname + 
                    teacher.DOB.ToString("ddMMyyyy")).RemoveDiacritics();

                teacher.TeacherCode = autogenTeacherId;

                //If user put in imgpfp in form then it will upload to designated folder id
                if (teacher.ImgPfp != null)
                {
                    GDriveServices gDriveServices = new GDriveServices();
                    UploadHelper uploadHelper = new UploadHelper();

                    byte[] data = uploadHelper.ConvertToByteArray(teacher.ImgPfp);
                    var fileID =
                    gDriveServices.UploadFile(autogenTeacherId, data, "1n680aa3fmW9qkZwrd7A1C5k0nf7DhkeP");

                    teacher.ImgId = fileID;
                }

                //_context.Add(teacher);
                //await _context.SaveChangesAsync();
                try
                {
                    await OnPostAsync(teacher);
                }
                catch 
                {
                    _logger.LogInformation("Teacher created with problems.",teacher.TeacherCode);
                }
                return  RedirectToAction("LecturerList", "Admin");
            }
            return View("~/Views/Admin/Lecturer/Create.cshtml", teacher);
        }
        // GET: Lecturer/Edit/5
        public async Task<IActionResult> LecturerEdit(int? id)
        {
            if (id == null || _context.Teacher == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher == null)
            {
                return NotFound();
            }
            return View("~/Views/Admin/Lecturer/Edit.cshtml", teacher);
        }

        // POST: Lecturer/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LecturerEdit(int id, [Bind("TeacherId,FullName,Email,DOB,IsLocked")] Teacher teacher)
        {
            if (id != teacher.TeacherId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.TeacherId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View("~/Views/Admin/Lecturer/Edit.cshtml", teacher);
        }

        // POST: Lecturer/Edit/5
        // This was only for editting the IsLocked field
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult?> LecturerLock(string id, bool isLocked)
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            //var teacher = await _context.Teacher.FindAsync(id);
            var teacher = await _context.Teacher.Where(x => x.TeacherCode.Contains(id)).FirstOrDefaultAsync();
            #pragma warning restore CS8602 // Dereference of a possibly null reference.
            if (teacher == null)
            {
                return NotFound();
            }

            teacher.IsLocked = isLocked;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(teacher);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeacherExists(teacher.TeacherId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //System.Diagnostics.Debug.WriteLine("Teacher "+id+" new IsLocked value " + isLocked);
                return RedirectToAction("LecturerList", "Admin"); ;
            }
            //return View("~/Views/Admin/Lecturer/Edit.cshtml", teacher);
            return null;
        }

        // POST: Lecturer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LecturerDeleteConfirmed(int id)
        {
            if (_context.Teacher == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Teacher'  is null.");
            }
            var teacher = await _context.Teacher.FindAsync(id);
            if (teacher != null)
            {
                _context.Teacher.Remove(teacher);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeacherExists(int? id)
        {
            return (_context.Teacher?.Any(e => e.TeacherId == id)).GetValueOrDefault();
        }

        // GET: Lecturer/Delete/5
        public async Task<IActionResult> LecturerDelete(int? id)
        {
            if (id == null || _context.Teacher == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.TeacherId == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Lecturer/Delete.cshtml", teacher);
        }

        // STUDENT

        // GET: Student
        public async Task<IActionResult> StudentList()
        {
            var studentList = await _context.Student.ToListAsync();
            return _context.Teacher != null ?
                        View("~/Views/Admin/StudentAccounts/Index.cshtml", studentList) :
                        Problem("Entity set 'ApplicationDbContext.Student'  is null.");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
                    if (!TeacherExists(student.StudentId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //System.Diagnostics.Debug.WriteLine("Teacher "+id+" new IsLocked value " + isLocked);
                return RedirectToAction("StudentList", "Admin"); ;
            }
            //return View("~/Views/Admin/StudentAccounts/Edit.cshtml", student);
            return null;
        }

        public IActionResult StudentCreate()
        {
            return View("~/Views/Admin/StudentAccounts/Create.cshtml");
        }

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
            return RedirectToAction("StudentList", "Admin");
        }
    }
}
