namespace IssueService.DAL
{
    public class EfDbInitializer
    {
        private readonly ApplicationDbContext _dataContext;

        public EfDbInitializer(ApplicationDbContext dataContext)
        {
            _dataContext = dataContext;
        }
        
        public void InitializeDb()
        {
            // _dataContext.Database.EnsureDeleted();
            // EnsureCreated применяет миграции, но на всякий случай, чтобы дз засчитали
            _dataContext.Database.EnsureCreated();
            // _dataContext.Database.Migrate();

            // _dataContext.AddRange(FakeDataFactory.Issues);
            // _dataContext.SaveChanges();
        }
    }
}
