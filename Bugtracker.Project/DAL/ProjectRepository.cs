using BugTracker.Domain;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.DAL.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private DbContext dbContext;

        public ProjectRepository(DbContext dbContext)
            : base(dbContext)
        {
            this.dbContext = dbContext;
        }

	Task<T> GetAsync(Guid id)
	{
		return Task.FromResult(null);
	}

	Task<IEnumerable<T>> GetAllAsync(Guid id)
	{
		return Task.FromResult(null);
	}

        void Add(T entity)
	{
	}

        void Remove(T entity)
	{
	}

        Task SaveChangesAsync()
	{
		return Task.FromResult(0);
	}
    }
}
