﻿using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.HelperClasses.TempModels;
using WebsiteQuanLyLamViecNhom.Models;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.GroupDTO;
using static WebsiteQuanLyLamViecNhom.HelperClasses.TempModels.ProjectDTO;

namespace WebsiteQuanLyLamViecNhom.Controllers
{
    public class ProjectController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<BaseApplicationUser> _userManager;

        static Teacher? viewModel = new Teacher
        {
            TeacherCode = "Teacher",
            ImgId = null
        };

        public ProjectController(ApplicationDbContext context, UserManager<BaseApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        static Student? viewModelStudent = new Student
        {
            StudentCode = "Student"
        };

        static Teacher? viewModelTeacher = new Teacher
        {
            TeacherCode = "Teacher"
        };  

        //TO-DO:
        //https://stackoverflow.com/questions/37554536/ho-do-i-show-a-button-that-links-to-a-page-only-if-the-user-is-authorized-to-vie
        // Return View for Teacher
        [Route("Teacher/Project/{Class?}/{GroupId?}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherIndex(string Class, string GroupId)
        {
            try
            {
                viewModelTeacher = await _context.Teacher.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
                ViewData["Teacher"] = viewModelTeacher;
                //var result = await _context.Class.Where(t => t.Code == id).FirstOrDefaultAsync();
                Group? group = await _context.Group.Where(t => t.Id == GroupId)
                                    .Include(p => p.Project)
                                    .Include(s => s.Students)
                                    .ThenInclude(sc => sc.Student)
                                    .FirstOrDefaultAsync();
                GroupDTO groupDTO = new();
                StudentClass? leader = group.Students.Where(l => l.Student.StudentCode == group.LeaderID)
                                            .FirstOrDefault();

                var currentGroup = new GroupDTO.GroupVM
                {
                    LeaderID = group.LeaderID,
                    MOTD = group.MOTD,
                    ProjectId = group.ProjectId,
                    ProjectName = group.Project.Name,
                    memberList = group.Students,
                    LeaderName = leader.Student.LastName + " " + leader.Student.FirstName,
                    CurrentClass = group.Project.Class,
                    ProjectAttachmentsJSON = group.Project.fileIDJSON

                };
                var taskList = await _context.Task
                                             .Where(p => p.GroupId == GroupId)
                                                                                    .Include(sc => sc.StudentClass)
                                         .ThenInclude(s => s.Student)
                                             .ToListAsync();
                if (taskList.Count > 0)
                    currentGroup.Tasks = taskList;

                groupDTO.GroupViewModel = currentGroup;

                return View(groupDTO);
            }
            catch (Exception ex)
            {
                return View(ex.Message);
            }
        }

        // Return View for Student
        [Route("Student/Project/{ClassCode?}")]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> StudentIndex(string ClassCode)
        {
            viewModelStudent = await _context.Student.FindAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
            ViewData["Student"] = viewModelStudent;

            Group? group = await _context.Group
                        .Where(t => t.Project.Class.Code == ClassCode)
                        .Include(p => p.Project)
                        .ThenInclude(c => c.Class)
                        .Include(s => s.Students)
                        .ThenInclude(sc => sc.Student)
                        .FirstOrDefaultAsync();

            GroupDTO groupDTO = new();
            StudentClass? leader = group.Students.Where(l => l.Student.StudentCode == group.LeaderID)
                                        .FirstOrDefault();

            var currentGroup = new GroupDTO.GroupVM
            {
                LeaderID = group.LeaderID,
                MOTD = group.MOTD,
                ProjectId = group.ProjectId,
                ProjectName = group.Project.Name,
                memberList = group.Students,
                LeaderName = leader.Student.LastName + " " + leader.Student.FirstName,
                CurrentClass = group.Project.Class,
                ProjectAttachmentsJSON = group.Project.fileIDJSON

            };

            var taskList = await _context.Task
                                         .Where(p => p.GroupId == group.Id)
                                         .Include(sc => sc.StudentClass)
                                         .ThenInclude(s => s.Student)
                                         .ToListAsync();
            if (taskList.Count > 0)
                currentGroup.Tasks = taskList;

            groupDTO.GroupViewModel = currentGroup;

            return View(groupDTO);

        }

        //------------------Actions starts------------------->>


        public async Task<IActionResult> CreateTask(GroupDTO.TaskDTO createTaskDTO)
        {
            if (ModelState.IsValid)
            {

                List<StudentClass> memberList = new List<StudentClass>();

                foreach (var studentid in createTaskDTO.memberList)
                {
                    var member = await _context.StudentClass
                                        .Where(sc => sc.StudentId == studentid)
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
                    TaskName = createTaskDTO.TaskName,
                    StudentClass = memberList,
                    DeadLineDate = createTaskDTO.Deadline,
                    Description = createTaskDTO.Description,
                    Status = Models.TaskStatus.OnGoing,
                    Group = memberList.FirstOrDefault().Group
                };

                _context.Add(newtask);
                foreach(var member in memberList)
                {
                    member.Tasks.Add(newtask);
                }
                _context.UpdateRange(memberList);

                await _context.SaveChangesAsync();
                return RedirectToAction("StudentIndex", 
                    new { classCode = memberList.FirstOrDefault().Class.Code });
            }
            // TODO: Return errors
            return RedirectToAction("StudentIndex",
                new { Error = ModelState.ToString() });
        }

        public async Task<IActionResult> UpdateTask(GroupDTO.UpdateTaskDTO updateTaskDTO)
        {
            if (ModelState.IsValid)
            {

                var task = await _context.Task
                                        .Where(sc => sc.TaskId == updateTaskDTO.TaskID)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(s => s.Student)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(c => c.Class)
                                        .FirstOrDefaultAsync();

                List<StudentClass> memberList = task.StudentClass.ToList();

                //Only setting the status for now
                task.Description = updateTaskDTO.Description;
                task.Status = Models.TaskStatus.Pending;

                _context.UpdateRange(memberList);

                await _context.SaveChangesAsync();
                return RedirectToAction("StudentIndex",
                    new { classCode = memberList.FirstOrDefault().Class.Code });
            }
            // TODO: Return errors
            return RedirectToAction("StudentIndex",
                new { Error = ModelState.ToString() });
        }

        public async Task<IActionResult> VerifyTask(GroupDTO.UpdateTaskDTO updateTaskDTO)
        {
            if (ModelState.IsValid)
            {

                var task = await _context.Task
                                        .Where(sc => sc.TaskId == updateTaskDTO.TaskID)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(s => s.Student)
                                        .Include(sc => sc.StudentClass)
                                            .ThenInclude(c => c.Class)
                                        .FirstOrDefaultAsync();

                List<StudentClass> memberList = task.StudentClass.ToList();

                //Only setting the status for now
                task.Description = updateTaskDTO.Description;
                task.TaskGradedByLeader = updateTaskDTO.Grade;
                task.Status = Models.TaskStatus.Complete;

                _context.UpdateRange(memberList);

                await _context.SaveChangesAsync();
                return RedirectToAction("StudentIndex",
                    new { classCode = memberList.FirstOrDefault().Class.Code });
            }
            // TODO: Return errors
            return RedirectToAction("StudentIndex",
                new { Error = ModelState.ToString() });
        }

        public async Task<IActionResult> GradeGroup(GroupDTO.UpdateTaskDTO gradeGroupDTO)
        {
            return View(gradeGroupDTO);
        }

        //------------------Actions ends--------------------->>

    }
}
