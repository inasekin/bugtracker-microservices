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

        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Project>()
                .HasKey(i => i.Id);
            modelBuilder.Entity<Project>()
                .HasMany(p=>p.UserRoles)
                .WithOne(u=>u.Project);
            modelBuilder.Entity<Project>()
                .HasMany(p=>p.IssueTypes)
                .WithOne(t=>t.Project);
            modelBuilder.Entity<Project>()
                .HasMany(p=>p.IssueCategories)
                .WithOne()
                .HasForeignKey(e => e.ProjectId);
            modelBuilder.Entity<Project>()
                .HasMany(p=>p.Versions)
                .WithOne(v=>v.Project);

            modelBuilder.Entity<ProjectUserRoles>()
                .HasKey(i => i.Id);
//            modelBuilder.Entity<ProjectUserRoles>()                
  //              .HasOne<Project>();
            //modelBuilder.Entity<ProjectUserRoles>()
            //    .HasAlternateKey(i => new { i.ProjectId, i.UserId, i.RoleId });

            modelBuilder.Entity<ProjectIssueType>()
                .HasKey(i => i.Id);
    //        modelBuilder.Entity<ProjectIssueType>()
      //          .HasOne<Project>();
            //modelBuilder.Entity<ProjectIssueType>()
            //    .HasAlternateKey(i => new { i.ProjectId, i.IssueTypeId });
                    
            modelBuilder.Entity<ProjectIssueCategory>()
                .HasKey(i => i.Id);
//            modelBuilder.Entity<ProjectIssueCategory>()
  //              .HasOne<Project>();

            modelBuilder.Entity<ProjectVersion>()
                .HasKey(i => i.Id);
        }
    }
}   