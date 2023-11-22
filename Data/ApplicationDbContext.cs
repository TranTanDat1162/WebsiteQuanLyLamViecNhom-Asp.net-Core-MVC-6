using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebsiteQuanLyLamViecNhom.Models;

namespace WebsiteQuanLyLamViecNhom.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WebsiteQuanLyLamViecNhom.Models.Teacher>? Teacher { get; set; }
        public DbSet<WebsiteQuanLyLamViecNhom.Models.Admin>? Admin { get; set; }
    }
}   