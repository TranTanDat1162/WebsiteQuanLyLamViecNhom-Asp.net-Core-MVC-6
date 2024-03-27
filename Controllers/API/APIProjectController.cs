using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;
using static WebsiteQuanLyLamViecNhom.Controllers.API.APIStudentController;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.GroupDTO;
using Task = WebsiteQuanLyLamViecNhom.Models.Task;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WebsiteQuanLyLamViecNhom.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class APIProjectController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Models.BaseApplicationUser> _userManager;
        private readonly ILogger<StudentController> _logger;

        public APIProjectController(ApplicationDbContext context, UserManager<Models.BaseApplicationUser> userManager, ILogger<StudentController> logger)
        {
            _context = context;
            _userManager = userManager;
            _logger = logger;
        }
        // GET: api/<ProjectController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<ProjectController>/5
        [HttpGet("{groupid}")]
        public async Task<object> GetAsync(string groupid)
        {
            _logger.LogInformation("GroupId: {groupid}", groupid);
            
            Group? group = await _context.Group
                        .Where(t => t.Id == groupid)
                        .Include(p => p.Project)
                        .Include(s => s.Students)               
                .FirstOrDefaultAsync();

            if (group != null) {
                var APIGroupDTO = new
                {
                    projectId = group.Project.Id,
                    group.Project.Name,
                    group.Project.Deadline,
                    group.Project.ClassId,
                    group.Project.Requirements,
                    group.Project.fileIDJSON,
                    group.LeaderID,
                    memberCount = group.Students.Count,
                    memberList = group.Students.ToList().Select(i=> i.StudentId),
                    group.MOTD
                };

                return new { status = 200, message = "Project's data fetched", Data =  APIGroupDTO};
            }

            return new { status = 404, message = "Project not found" };
        }

        // POST api/<ProjectController>
        [HttpPost("Task/{groupid}")]
        public async Task<object> PostNewTask(string groupid, [FromBody] TaskDTO task)
        {
            _logger.LogInformation("GroupId: {groupid}", groupid);

            Group? group = await _context.Group
                        .Where(t => t.Id == groupid)
                        .Include(p => p.Project)
                        .Include(s => s.Students)
                .FirstOrDefaultAsync();

            if (ModelState.IsValid && group != null)
            {

                List<StudentClass> memberList = new List<StudentClass>();

                foreach (var studentid in task.memberList)
                {
                    var member = await _context.StudentClass
                                        .Where(sc => sc.StudentId == studentid
                                                && sc.GroupID == task.GroupID)
                                        .Include(s => s.Student)
                                        .Include(g => g.Group)
                                        .Include(t => t.Tasks)
                                        .Include(c => c.Class)
                                        .FirstOrDefaultAsync();

                    //Đề phòng thôi
                    if (member != null)
                    {
                        memberList.Add(member);
                    }
                }

                Models.Task newtask = new Models.Task()
                {
                    TaskName = task.TaskName,
                    StudentClass = memberList,
                    DeadLineDate = task.Deadline,
                    Description = task.Description,
                    Status = Models.TaskStatus.OnGoing,
                    Group = memberList.FirstOrDefault().Group
                };

                _context.Add(newtask);
                foreach (var member in memberList)
                {
                    member.Tasks.Add(newtask);
                }
                _context.UpdateRange(memberList);

                await _context.SaveChangesAsync();
                return new { status = 200, message = "Project's data fetched", Data = newtask.TaskId };
            }
            // TODO: Return errors
            return new { status = 404, message = "Something went wrong" };

        }

        // PUT api/<ProjectController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ProjectController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
