using BugTracker.DAL.Repositories;

namespace BugTracker.DAL
{
    public interface IUnitOfWork
    {
        IBugTrackerRepository BugTrackerRepository { get; }
        void SaveChanges();
        Task SaveChangesAsync();
    }
}
