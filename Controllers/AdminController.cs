﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    [Authorize(Roles ="Admin")]
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
        public async Task<IActionResult> LecturerCreate([Bind("TeacherId,FullName,Email,DOB,ImgPfp,IsLocked")] Teacher teacher)
        {
            if (ModelState.IsValid)
            {
                if(teacher.ImgPfp != null)
                {
                    GDriveServices gDriveServices = new GDriveServices();
                    UploadHelper uploadHelper = new UploadHelper();

                    byte[] data = uploadHelper.ConvertToByteArray(teacher.ImgPfp);
                    var fileID =
                    gDriveServices.UploadFile(teacher.TeacherId.ToString(),data,"1n680aa3fmW9qkZwrd7A1C5k0nf7DhkeP");
                }
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

        // POST: Lecturer/Edit/5
        // This was only for editting the IsLocked field
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult?> LecturerLock(int id, bool isLocked)
        {
            #pragma warning disable CS8602 // Dereference of a possibly null reference.
            var teacher = await _context.Teacher.FindAsync(id);
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
