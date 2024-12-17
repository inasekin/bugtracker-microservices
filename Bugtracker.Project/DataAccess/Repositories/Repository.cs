using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using BugTracker.Domain;
using Bugtracker.DataAccess;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.DataAccess.Repositories
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly DataContext _dataContext;
        protected readonly DbSet<T> _dbSet;

        public Repository(DataContext dataContext)
        {
            _dataContext = dataContext;
            _dbSet = _dataContext.Set<T>();
        }

        virtual public Task<T> GetAsync(Guid id) 
        {
            return _dbSet.Where(p => p.Id == id)       
                .FirstOrDefaultAsync();
        }

        virtual public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.ToListAsync();
        }

        virtual public T Add(T entity)
        {
            return _dbSet.Add(entity).Entity;
        }

        virtual public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        virtual public void Remove(T entity)
        {
            _dbSet.Remove(entity);
        }
    }
}
