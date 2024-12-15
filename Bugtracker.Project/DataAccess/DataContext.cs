using Microsoft.EntityFrameworkCore;
using BugTracker.Domain;

namespace Bugtracker.DataAccess
{
    public class DataContext
        : DbContext
    {
        public DbSet<Project> Projects { get; set; }

        public DbSet<ProjectIssueCategory> ProjectIssueCategories { get; set; }
        
        public DbSet<ProjectIssueType> ProjectIssueTypes { get; set; }
        
        public DbSet<ProjectUserRoles> ProjectUserRoles { get; set; }

        public DbSet<ProjectVersion> ProjectVersions { get; set; }

        public DataContext()
        {
            
        }
        
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<ProjectUserRoles>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<ProjectUserRoles>()
                .HasAlternateKey(i => new { i.ProjectId, i.UserId, i.RoleId });

            modelBuilder.Entity<ProjectIssueType>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<ProjectIssueType>()
                .HasAlternateKey(i => new { i.ProjectId, i.IssueTypeId });

            modelBuilder.Entity<ProjectIssueCategory>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<ProjectVersion>()
                .HasKey(i => i.Id);
        }
    }
}   