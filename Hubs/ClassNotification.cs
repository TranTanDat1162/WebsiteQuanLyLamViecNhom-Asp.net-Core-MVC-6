using Google.Apis.Drive.v3.Data;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
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
            await Clients.All.SendAsync("ReceiveMessage", User.LastName + " " + User.FirstName, message);

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
                .Where(r => r.RoomID == RoomId )
                .Include(u => u.User)
                .ToList();

            foreach(var line in chatLog) {
                await Clients.All.SendAsync("ReceiveMessage"
                    ,line.User.LastName + " " + line.User.FirstName, line.MessageLine);
            }
        }
    }
}
