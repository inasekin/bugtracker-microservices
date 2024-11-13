using BugTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.DAL.Repositories
{
    public class BugTrackerRepository : Repository<TaskItem> ,IBugTrackerRepository
    {
        public BugTrackerDbContext dbContext;
        public BugTrackerRepository(BugTrackerDbContext dbContext)
            : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public void AddToContext(TaskItem taskItem)
        {
            this.dbContext.Add(taskItem);
        }

        public DbSet<TaskItem> TaskItems()
        {
            return this.dbContext.TaskItems;
        }
    }
}
