using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebsiteQuanLyLamViecNhom.Data;
using WebsiteQuanLyLamViecNhom.Models;
using Task = System.Threading.Tasks.Task;

namespace WebsiteQuanLyLamViecNhom.Hubs
{
    public class ClassNotification : Hub
    {
        //public async Task SendMessage(string user, string message)
        //{

        //    await Clients.All.SendAsync("ReceiveMessage", user, message);
        //}
        private readonly ApplicationDbContext _context;

        public ClassNotification(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task SendMessage(string userId,string roomId, string message)
        {
            //return this.serviceProvider.GetRequiredService<IDatabaseManager
            var User = await _context.Users.FindAsync(userId);
            await Clients.Group(roomId).SendAsync("ReceiveMessage", User.LastName + " " + User.FirstName, message, userId);

            if (User != null)
            {
                Message newLine = new Message()
                {
                    RoomID = roomId,
                    User = User,
                    MessageLine = message,
                    UserId = userId
                };
                await _context.Messages.AddAsync(newLine);
                _context.SaveChanges();
            }
        }

        public async Task GetChatHistory(string RoomId)
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now.AddDays(3);

            var chatLog = _context.Messages
                .Where(r => r.RoomID == RoomId)
                .Include(u => u.User)
                .ToList();

            foreach (var line in chatLog)
            {
                await Clients.Group(RoomId).SendAsync("ReceiveMessage"
                    , line.User.LastName + " " + line.User.FirstName, line.MessageLine,line.User.Id);
            }
        }
        public async Task GetClassNotification(string studentId)
        {
            var student = await _context.StudentClass
                .Where(s => s.StudentId == studentId)
                .Include(c => c.Class)
                    .ThenInclude(t => t.Teacher)
                .ToListAsync();

            foreach (var classInfo in student)
            {
                var chatLog = await _context.Messages
                .Where(r => r.RoomID == classInfo.ClassId.ToString())
                .OrderByDescending(t => t.Timestamp)
                .FirstOrDefaultAsync();

                if (chatLog != null)
                    await Clients.User(studentId).SendAsync("ReceiveNotification",
                       classInfo.Class.Code,
                       chatLog.MessageLine,
                       classInfo.Class.Teacher.ImgId,
                       chatLog.Timestamp.ToString("hh:mm tt"));

            }
        }
        public async Task AddToGroup(string RoomId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, RoomId);
        }
    }
}
