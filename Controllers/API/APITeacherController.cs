using Diacritics.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;
using static WebsiteQuanLyLamViecNhom.Controllers.API.APITeacherController;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebsiteQuanLyLamViecNhom.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class APITeacherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BaseApplicationUser> _userManager;
        private readonly ILogger<AdminController> _logger;

        public APITeacherController(ApplicationDbContext context, UserManager<BaseApplicationUser> usermanager, ILogger<AdminController> logger)
        {
            _context = context;
            _userManager = usermanager;
            _logger = logger;
        }
        public class TeacherDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public DateTime DOB { get; set; }
            public string Email { get; set; }
            public bool IsLocked { get; set; }
        }
        // GET: api/<APITeacherController>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Teacher>>> Get()
        {
            var teachers = await _context.Teacher
                .Select(t => new
                {
                    t.Id,
                    t.TeacherCode,
                    t.ImgId,
                    DOB = t.DOB.ToShortDateString(),
                    t.UserName,
                    t.Email,
                    t.IsLocked
                })
                .ToListAsync();

            if (!teachers.Any())
            {
                return NotFound();
            }

            return Ok(teachers);
        }

        // GET api/<APITeacherController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Teacher>> Get(string id)
        {
            if (_context.Teacher == null)
            {
                return NotFound();
            }
            var teacher = await _context.Teacher
                .Where(t => t.Id == id)
                .Select(t => new
                {
                    t.Id,
                    t.TeacherCode,
                    t.ImgId,
                    DOB = t.DOB.ToShortDateString(),
                    t.UserName,
                    t.FirstName,
                    t.LastName,
                    t.IsLocked
                })
                .FirstOrDefaultAsync();

            if (teacher == null)
            {
                return NotFound();
            }

            return Ok(teacher);
        }

        // POST api/<APITeacherController>
        [HttpPost]
        public async Task<ActionResult<Teacher>> Post([FromForm] Teacher teacher)
        {                                
            if (_context.Teacher == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Admin'  is null.");
            }
            string autogenlastname = string.Concat(teacher.LastName.Split(' ').Select(s => s[0]));

            string autogenTeacherId = (teacher.FirstName.ToUpper() +
            autogenlastname +
            teacher.DOB.ToString("ddMMyyyy")).RemoveDiacritics();

            teacher.TeacherCode = autogenTeacherId;
            teacher.UserName = teacher.Email;
            teacher.IsLocked = !teacher.IsLocked;

            //_context.Teacher.Add(teacher);
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

            return CreatedAtAction("PostTeacher", new { id = teacher.Id }, teacher);
            //return Ok();
        }

        // PUT api/<APITeacherController>/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(string id,[FromForm] TeacherDto teacherDTO)
        {

            // Retrieve the existing teacher from the database
            var teacher = await _context.Teacher.FindAsync(id);

            if (teacher == null)
            {
                return NotFound();
            }
            //------------------------------------------------
            // Update the teacher properties based on the DTO
            teacher.FirstName = teacherDTO.FirstName;
            teacher.LastName = teacherDTO.LastName;
            teacher.DOB = teacherDTO.DOB;
            teacher.Email = teacherDTO.Email;
            teacher.IsLocked = teacherDTO.IsLocked;

            teacher.UserName = teacher.Email;
            teacher.IsLocked = !teacher.IsLocked;

            //------------------------------------------------
            string autogenlastname = string.Concat(teacher.LastName.Split(' ').Select(s => s[0]));

            string autogenTeacherId = (teacher.FirstName.ToUpper() +
            autogenlastname +
            teacher.DOB.ToString("ddMMyyyy")).RemoveDiacritics();

            teacher.TeacherCode = autogenTeacherId;
            //------------------------------------------------

            _context.Entry(teacher).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TeacherExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // DELETE api/<APITeacherController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            if (_context.Teacher == null)
            {
                return NotFound();
            }
            var teacher = await _context.Teacher.Where(t => t.Id == id).FirstOrDefaultAsync();
            if (teacher == null)
            {
                return NotFound();
            }
            _context.Teacher.Remove(teacher);
            await _context.SaveChangesAsync();

            return Ok();
        }
        private bool TeacherExists(string id)
        {
            return (_context.Teacher?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
