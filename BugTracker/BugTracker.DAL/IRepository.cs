using System.Linq.Expressions;

namespace BugTracker.DAL
{
    public interface IRepository<T> where T : class
    {
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void Remove(T entity);
    }
}
