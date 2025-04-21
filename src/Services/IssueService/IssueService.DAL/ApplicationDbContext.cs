using IssueService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using FileInfo = IssueService.Domain.Models.FileInfo;

namespace IssueService.DAL;
public class ApplicationDbContext : DbContext
{
    public DbSet<Issue> Issues { get; set; }

    public DbSet<FileInfo> Files { get; set; }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
      
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Issue>()
            .HasKey(i => i.Id);

        modelBuilder.Entity<Issue>()
            .HasMany(i => i.Files);
            //.WithOne(v => v.Issue);

        modelBuilder.Entity<FileInfo>()
            .HasKey(i => i.Id);
    }
}
