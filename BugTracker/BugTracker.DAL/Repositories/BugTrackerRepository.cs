using BugTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.DAL.Repositories
{
    public class BugTrackerRepository : Repository<Issue> ,IBugTrackerRepository
    {
        public BugTrackerDbContext dbContext;
        public BugTrackerRepository(BugTrackerDbContext dbContext)
            : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public void AddToContext(Issue taskItem)
        {
            this.dbContext.Add(taskItem);
        }

        public DbSet<Issue> TaskItems()
        {
            return this.dbContext.Issues;
        }
    }
}
