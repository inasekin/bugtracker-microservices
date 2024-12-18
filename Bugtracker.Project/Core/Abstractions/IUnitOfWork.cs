using BugTracker.Domain;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace BugTracker.DataAccess
{
    public interface IUnitOfWork
    {
        Task SaveChangesAsync();
    }
}
