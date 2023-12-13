using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Data
{
    public class ApplicationDbContext : IdentityDbContext<BaseApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<Models.Task>? Task { get; set; }
        public DbSet<Group>? Group { get; set; }
        public DbSet<Project>? Project{ get; set; }
        public DbSet<StudentClass>? StudentClass { get; set; }
        public DbSet<Class>? Class { get; set; }
        public DbSet<Student>? Student  { get; set; }
        public DbSet<Teacher>? Teacher { get; set; }
        public DbSet<Admin>? Admin { get; set; }
    }
}   