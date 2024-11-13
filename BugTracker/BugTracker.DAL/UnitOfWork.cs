using BugTracker.DAL.Repositories;

namespace BugTracker.DAL
{
    public class UnitOfWork : IUnitOfWork
    {
        private BugTrackerDbContext _dbContext;

        public UnitOfWork(BugTrackerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IBugTrackerRepository BugTrackerRepository
        {
            get
            {
                return new BugTrackerRepository(_dbContext);
            }
        }

        public void SaveChanges()
        {
            this._dbContext.SaveChanges();
        }

        public async Task SaveChangesAsync()
        {
            await this._dbContext.SaveChangesAsync();
        }
    }
}
