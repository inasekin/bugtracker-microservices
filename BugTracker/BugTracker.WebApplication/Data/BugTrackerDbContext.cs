using Microsoft.EntityFrameworkCore;
using BugTracker.WebApplication.Models;

namespace BugTracker.WebApplication.Data
{
    public class BugTrackerDbContext : DbContext
    {
        public BugTrackerDbContext(DbContextOptions<BugTrackerDbContext> options) : base(options) { }

        public DbSet<UserResponse> Users { get; set; }
    }
}