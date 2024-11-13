using BugTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.DAL.Repositories
{
    public interface IBugTrackerRepository
    {
        public DbSet<TaskItem> TaskItems();
        public void AddToContext(TaskItem taskItem);
    }
}
