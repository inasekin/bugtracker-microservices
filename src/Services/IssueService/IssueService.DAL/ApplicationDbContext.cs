using IssueService.Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace IssueService.DAL;
public class ApplicationDbContext : DbContext
{
    public DbSet<Issue> Issues { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
      
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Issue>()
            .HasKey(i => i.Id);
    }
}
