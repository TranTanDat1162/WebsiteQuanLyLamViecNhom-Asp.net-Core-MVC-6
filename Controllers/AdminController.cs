using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Data;
using Google.Apis.Drive.v3.Data;
using WebsiteQuanLyLamViecNhom.Models.ViewModel;
using WebsiteQuanLyLamViecNhom.Data.Migrations;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.BaseApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, UserManager<Models.BaseApplicationUser> usermanager, ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = usermanager;
            _logger = logger;
        }

        //static AdminDashboardViewModel? viewModel = new AdminDashboardViewModel
        //{
        //    NumberOfStudentAccounts = 2000,
        //    NumberOfTeacherAccounts = 100
        //};

        public async Task<IActionResult> Index()
        {
            //var numberOfStudentAccounts = _userManager.Users
            //                                .Where(x => x.)
            //                                .Count();
            //var numberOfTeacherAccounts = await _userManager.Users
            //                                .Where(x =>  User.IsInRole("Teacher"))
            //                                .CountAsync();

           
            var Total = await _userManager.Users.ToListAsync();
            int numberOfStudentAccounts = 0, numberOfTeacherAccounts = 0;
            
            foreach(var acc in Total)
            {
                var roles = await _userManager.GetRolesAsync(acc);
                if (roles.Contains("Student"))
                {
                    numberOfStudentAccounts++;
                }
                else if (roles.Contains("Teacher"))
                {
                    numberOfTeacherAccounts++;
                }
            }
            // Tạo model
            ViewData["NumberOfAccounts"] = new AdminDashboardViewModel()
            {
                NumberOfStudentAccounts = numberOfStudentAccounts,
                NumberOfTeacherAccounts = numberOfTeacherAccounts
            };

            //ViewData["NumberOfStudentAccounts"] = numberOfStudentAccounts;
            //ViewData["NumberOfTeacherfAccounts"] = numberOfTeacherAccounts;
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
        public IActionResult AdminClass()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }

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
    }

}
