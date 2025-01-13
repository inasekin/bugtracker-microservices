using Microsoft.EntityFrameworkCore;
using UserService.Domain.Models;

namespace UserService.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<UserResponse> Users { get; set; } = null!;
    }
}