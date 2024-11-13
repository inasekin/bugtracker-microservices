using BugTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.DAL
{
    public class BugTrackerDbContext : DbContext
    {
        public DbSet<TaskItem> TaskItems { get; set; }

        public BugTrackerDbContext()
        {
            
        }

        public BugTrackerDbContext(DbContextOptions<BugTrackerDbContext> options)
            :base(options) 
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TaskItem>().ToTable("TaskItem");
            modelBuilder.Entity<TaskItem>().HasKey("TaskItemId");
            modelBuilder.Entity<TaskItem>().Property(it => it.TaskItemId).HasDefaultValueSql("NEXT VALUE FOR TaskItemIdSequence");
            modelBuilder.HasSequence<int>("TaskItemIdSequence").IncrementsBy(1).HasMin(1).HasMax(100000).StartsAt(1);
            base.OnModelCreating(modelBuilder);
        }
    }
}
