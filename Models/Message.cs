using NuGet.Packaging.Signing;
using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class Message
    {
        [Key]
        public int Id { get; set; }
        public string RoomID { get; set; }
        public string UserId { get; set; }
        public BaseApplicationUser User { get; set; }
        public string? MessageLine { get; set; }
        public DateTime Timestamp { get; set; }
        public Message() 
        {
            Timestamp = DateTime.Now;
        }
    }
}
