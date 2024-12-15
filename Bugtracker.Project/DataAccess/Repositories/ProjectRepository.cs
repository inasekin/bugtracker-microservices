using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using BugTracker.Domain;
using Bugtracker.DataAccess;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BugTracker.DataAccess.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly DataContext _dataContext;

        public ProjectRepository(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public Task<Project> GetAsync(Guid id)
        {
            return _dataContext.Projects
                .Where(p => p.Id == id)       
                .Include(p => p.IssueVersion)
                .Include(p => p.IssueTypes)
                .Include(p => p.IssueCategories)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Project>> GetAllAsync()
        {
            return await _dataContext.Projects.ToListAsync();
        }

        public void Add(Project entity)
        {
            _dataContext.Add(entity);
        }

        public void Remove(Project entity)
        {
            _dataContext.Remove(entity);
        }
    }
}
