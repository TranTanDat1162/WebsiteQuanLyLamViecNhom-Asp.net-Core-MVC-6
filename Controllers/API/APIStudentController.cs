using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using NuGet.Configuration;
using System.Security.Claims;
using System.Text.Json;
using System.Text.Json.Serialization;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using WebsiteQuanLyLamViecNhom.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebsiteQuanLyLamViecNhom.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIStudentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.BaseApplicationUser> _userManager;
        private readonly ILogger<StudentController> _logger;


        public APIStudentController(ApplicationDbContext context, UserManager<Models.BaseApplicationUser> userManager, ILogger<StudentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        public class ClassListAPIDTO
        {
            public List<Object> StudentClasses = new List<Object>();
        }


        // GET: api/<StudentController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<StudentController>/5    
        [HttpGet("{userid}")]
        public async Task<ActionResult<object>> GetAsync(string userid)
        {
            _logger.LogInformation("Userid: {userid}", userid);
            //var settings = new JsonSerializerOptions { ReferenceHandler = ReferenceHandler.Preserve };

            // Lấy thông tin người dùng đăng nhập
            var tempuser = await _context.Student.FindAsync(userid);

            // Kiểm tra xem người dùng có tồn tại không
            if (tempuser != null)
            {
                Student? currentStudent = await _context.Student
                    .Include(t => t.ClassList)
                        .ThenInclude(t => t.Class)
                            .ThenInclude(t => t.Teacher)
                    .FirstOrDefaultAsync(t => t.Id == userid);

                CreateClassDTO ClassList = new CreateClassDTO();
                ClassListAPIDTO classListAPIDTO = new ClassListAPIDTO();
                if (currentStudent != null)
                {
                    var currentClasses = await _context.StudentClass
                        .Where(s => s.StudentId == currentStudent.Id)
                        .Include(t => t.Class)
                        .Select(c => new
                        {
                            c.Id,
                            c.GroupID,
                            c.StudentId,
                            c.ClassId,
                            c.Class.SubjectName,
                            c.Class.SubjectId,
                            c.Class.Teacher.Email,
                            c.Class.TeacherId,
                            c.Class.ClassGroup,
                            c.Class.MOTD
                        })
                        .ToListAsync();

                    foreach (var studentClass in currentClasses)
                    {
                        classListAPIDTO.StudentClasses.Add(studentClass);
                    }

                    ClassList.crumbs = new List<List<string>>()
                    {
                        new List<string>() { "/Student", "Home" }
                    };

                    // Kiểm tra nếu người dùng chưa cập nhật email
                    if (string.IsNullOrEmpty(tempuser.Email))
                    {
                        _logger.LogInformation("APIStudent logged in.");
                        return new { status = 200, message = "Account's email havn't verified"};
                    }

                    return new { status = 200, message = "Class list fetched", Data = classListAPIDTO.StudentClasses };
                }

                return new { status = 200, message = "Class list empty", Data = classListAPIDTO.StudentClasses };
                }

            return new { status = 404, message = "User not found" };
        }

        // POST api/<StudentController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<StudentController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<StudentController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
