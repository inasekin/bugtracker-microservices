using System.Threading.Tasks;
using Bugtracker.DataAccess;
using BugTracker.DataAccess;

namespace DataAccess
{
    public class ProjectRepositoryUnitOfWork : IUnitOfWork
    {
        private readonly DataContext _dataContext;

        public ProjectRepositoryUnitOfWork(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task SaveChangesAsync()
        {
            return _dataContext.SaveChangesAsync();
        }
    }
}
