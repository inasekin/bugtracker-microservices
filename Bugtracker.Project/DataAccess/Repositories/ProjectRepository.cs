using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using BugTracker.Domain;

namespace BugTracker.DataAccess.Repositories
{
    // TODO: Enitity Framework repository
    public class ProjectRepository : IProjectRepository<Project> 
    {
        public Task<Project> GetAsync(Guid id)
        {
	        return Task.FromResult<Project>(null);
        }

        public Task<IEnumerable<Project>> GetAllAsync(Guid id)
        {
	        return Task.FromResult<IEnumerable<Project>>(null);
        }

        public void Add(Project entity)
        {
        }

        public void Remove(Project entity)
        {
        }
    }
}
