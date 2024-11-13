using BugTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.DAL.Repositories
{
    public interface IBugTrackerRepository
    {
        public DbSet<Issue> TaskItems();
        public void AddToContext(Issue taskItem);
    }
}
