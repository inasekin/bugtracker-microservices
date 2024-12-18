using Microsoft.EntityFrameworkCore;
using System;
using System.Threading.Tasks;

namespace Bugtracker.DataAccess
{
    public class ProjectsEfDbInitializer
    {
        private readonly DataContext _dataContext;

        public ProjectsEfDbInitializer(DataContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public void InitializeDb()
        {
            //_dataContext.Database.EnsureDeleted();
            // EnsureCreated применяет миграции, но на всякий случай, чтобы дз засчитали
            _dataContext.Database.EnsureCreated();
            //_dataContext.Database.Migrate();

            //_dataContext.AddRange(FakeDataFactory.Employees);
            //_dataContext.SaveChanges();
            
            //_dataContext.AddRange(FakeDataFactory.Preferences);
            //_dataContext.SaveChanges();
            
            //_dataContext.AddRange(FakeDataFactory.Customers);
            //_dataContext.SaveChanges();
            
            //_dataContext.AddRange(FakeDataFactory.Partners);
            //_dataContext.SaveChanges();
        }
    }
}