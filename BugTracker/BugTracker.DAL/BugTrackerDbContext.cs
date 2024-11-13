using BugTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.DAL
{
    public class BugTrackerDbContext : DbContext
    {
        public DbSet<Issue> Issues { get; set; }

        public BugTrackerDbContext()
        {
            
        }

        public BugTrackerDbContext(DbContextOptions<BugTrackerDbContext> options)
            :base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Issue>().ToTable("Issue");
            modelBuilder.Entity<Issue>().HasKey("IssueId");
            modelBuilder.Entity<Issue>().Property(it => it.IssueId).HasDefaultValueSql("NEXT VALUE FOR IssueIdSequence");
            modelBuilder.HasSequence<int>("IssueIdSequence").IncrementsBy(1).HasMin(1).HasMax(100000).StartsAt(1);
            base.OnModelCreating(modelBuilder);
        }
    }
}
