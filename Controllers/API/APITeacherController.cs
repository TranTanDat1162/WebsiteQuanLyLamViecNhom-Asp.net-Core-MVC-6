using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebsiteQuanLyLamViecNhom.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class APITeacherController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public APITeacherController(ApplicationDbContext context)
        {
            _context = context;
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
                    t.Email,
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
        public async Task<ActionResult<Teacher>>Post (Teacher teacher)
        {
            if (_context.Teacher == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Admin'  is null.");
            }
            _context.Teacher.Add(teacher);
            await _context.SaveChangesAsync();

            //return CreatedAtAction("PostTeacher", new { id = admin.Id }, admin);
            return Ok();
        }

        // PUT api/<APITeacherController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<APITeacherController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
