using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
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
        public IActionResult StudentList()
        {
            ViewData["Title"] = "UEF - Quản lý làm việc nhóm";
            return View();
        }
        //----------------------------------------//

        // GET: Lecturer
        public async Task<IActionResult> LecturerList()
        {
            return _context.Teacher != null ?
                        View("~/Views/Admin/Lecturer/Index.cshtml", await _context.Teacher.ToListAsync()) :
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
        public async Task<IActionResult> LecturerCreate([Bind("TeacherId,FullName,Email,DOB,IsLocked")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                _context.Add(teacher);
                await _context.SaveChangesAsync();
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

        private bool TeacherExists(int id)
        {
            return (_context.Teacher?.Any(e => e.TeacherId == id)).GetValueOrDefault();
        }
    }
}
