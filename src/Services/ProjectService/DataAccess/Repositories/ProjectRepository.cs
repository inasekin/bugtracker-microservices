using System.Threading.Tasks;
using System;
using BugTracker.Domain;
using Bugtracker.DataAccess;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

namespace BugTracker.DataAccess.Repositories
{
    public class ProjectRepository : Repository<Project>, IRepository<Project>
    {
        public ProjectRepository(DataContext dataContext) : base(dataContext) {
        }

        override public Task<Project> GetAsync(Guid id)
        {
            return _dbSet
                .Where(p => p.Id == id)       
                .Include(p => p.Versions)
                .Include(p => p.IssueTypes)
                .Include(p => p.IssueCategories)
                .Include(p => p.UserRoles)
                .AsSplitQuery()
                .FirstOrDefaultAsync();
        }
    }
}
