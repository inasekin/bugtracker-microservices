using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BugTracker.DAL
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly DbSet<T> _entities;

        public Repository(DbContext context)
        {
            _entities = context.Set<T>();
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> predicate)
        {
            return _entities.Where(predicate);
        }
        public void Add(T entity)
        {
            _entities.Add(entity);
        }
        public void Remove(T entity)
        {
            _entities.Remove(entity);
        }
    }
}
