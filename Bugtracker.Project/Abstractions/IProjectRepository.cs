using System.Linq.Expressions;

namespace BugTracker.DAL
{
    public interface IProjectRepository<T> where T : class
    {
	Task<T> GetAsync(Guid id);
	Task<IEnumerable<T>> GetAllAsync(Guid id);

        void Add(T entity);
        void Remove(T entity);

        Task SaveChangesAsync();
    }
}
