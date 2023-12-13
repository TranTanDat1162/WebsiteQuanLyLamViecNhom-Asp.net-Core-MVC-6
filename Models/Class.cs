using System.ComponentModel.DataAnnotations;

namespace WebsiteQuanLyLamViecNhom.Models
{
    public enum RoleProject
    {
        CreateByTeacher,
        CreateByStudent,
        StudentChoose
    }
    public enum RoleGroup
    {
        AssignByTeacher,
        AssignByStudent
    }
    public class Class
    {
        [Key]
        public int Id { get; set; }
        public string Code { get; set; }
        public string Semester { get; set; }
        public int Year { get; set; }
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string ClassGroup { get; set; }
        public string MOTD { get; set; }
        public string? ProjectRequirements{ get; set; }
        public RoleGroup RoleGroup {  get; set; }
        public RoleProject RoleProject { get; set; }
        public DateTime OpenDate { get; set; }
        public Boolean IsLocked { get; set; }
        public string TeacherId { get; set; }
        public Teacher Teacher { get; set; }
    }
}
