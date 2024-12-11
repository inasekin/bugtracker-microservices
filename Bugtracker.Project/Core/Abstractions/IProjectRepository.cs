using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.DataAccess
{
    public interface IProjectRepository<T> where T : class
    {
	    Task<T> GetAsync(Guid id);
    	Task<IEnumerable<T>> GetAllAsync(Guid id);
        void Add(T entity);
        void Remove(T entity);
     }
}
