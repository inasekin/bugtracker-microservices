using BugTracker.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.DataAccess
{
    public interface IProjectRepository
    {
	    Task<Project> GetAsync(Guid id);
    	Task<IEnumerable<Project>> GetAllAsync();
        void Add(Project entity);
        void Remove(Project entity);
    }
}
