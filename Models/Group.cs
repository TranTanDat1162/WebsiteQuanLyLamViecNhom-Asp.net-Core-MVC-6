using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public class Group
    {
        public string Id { get; set; }
        public string MOTD { get; set; }
        public int ProjectId { get; set; }

        [ForeignKey("ProjectId")]
        public virtual Project Project { get; set; }
        public virtual ICollection<StudentClass> Students { get; set; }
    }
}
