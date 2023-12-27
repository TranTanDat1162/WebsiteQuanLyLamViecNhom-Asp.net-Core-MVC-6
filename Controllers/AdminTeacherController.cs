using Diacritics.Extensions;
using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using WebsiteQuanLyLamViecNhom.Models;
using Task = System.Threading.Tasks.Task;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class AdminTeacherController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BaseApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminTeacherController(ApplicationDbContext context, UserManager<BaseApplicationUser> usermanager, ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = usermanager;
            _logger = logger;
        }
        public async Task OnPostAsync(Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                var user = teacher;

                //await _userStore.SetUserNameAsync(user, teacher.Email, CancellationToken.None);
                //await _emailStore.SetEmailAsync(user, teacher.Email, CancellationToken.None);
                user.UserName = teacher.Email;
                user.IsLocked = !teacher.IsLocked;
                var result = await _userManager.CreateAsync(user, teacher.TeacherCode);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(user, "Teacher");
                    return;

                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogInformation("Problems here!." + ModelState.ToString());
                }
            }
            return;
        }

        // GET: Lecturer
        [Route("Admin/Teacher")]
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
        //WIP
        [Route("Admin/Teacher/Details")]
        public async Task<IActionResult> LecturerDetails(string? id)
        {
            if (id == null || _context.Teacher == null)
            {
                return NotFound();
            }

            var teacher = await _context.Teacher
                .FirstOrDefaultAsync(m => m.Id == id);
            if (teacher == null)
            {
                return NotFound();
            }

            return View("~/Views/Admin/Lecturer/Details.cshtml", teacher);
        }

        // GET: Lecturer/Create
        [Route("Admin/Teacher/Create")]
        public IActionResult LecturerCreate()
        {
            return View("~/Views/Admin/Lecturer/Create.cshtml");
        }

        // POST: Lecturer/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Teacher/Create")]
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
                teacher.IsLocked = !teacher.IsLocked;
                teacher.UserName = teacher.Email;

                //If user put in imgpfp in form then it will upload to designated folder id
                if (teacher.ImgPfp != null)
                {
                    GDriveServices gDriveServices = new GDriveServices();
                    UploadHelper uploadHelper = new UploadHelper();

                    byte[] data = uploadHelper.ConvertToByteArray(teacher.ImgPfp);
                    var fileID =
                    gDriveServices.UploadFile(autogenTeacherId, data, "1n680aa3fmW9qkZwrd7A1C5k0nf7DhkeP");

                    
                    teacher.ImgId = (string)(fileID?.GetType().GetProperty("FileId")?.GetValue(fileID));
                }

                //_context.Add(teacher);
                //await _context.SaveChangesAsync();
                var result = await _userManager.CreateAsync(teacher, teacher.TeacherCode);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User created a new account with password.");
                    await _userManager.AddToRoleAsync(teacher, "Teacher");
                        
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                    _logger.LogInformation("Problems here!." + ModelState.ToString());
                }
                return RedirectToAction("LecturerList", "AdminTeacher");
            }
            return RedirectToAction("LecturerList", "AdminTeacher");
        }

        // POST: Lecturer/Lock/5
        // This was only for editting the IsLocked field
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Admin/Teacher/Lock/{id?}")]
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
                    if (!TeacherExists(teacher.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                //System.Diagnostics.Debug.WriteLine("Teacher "+id+" new IsLocked value " + isLocked);
                return RedirectToAction("LecturerList", "AdminTeacher");
            }
            //return View("~/Views/Admin/Lecturer/Edit.cshtml", teacher);
            return null;
        }
        private bool TeacherExists(string? id)
        {
            return (_context.Teacher?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
