using Microsoft.EntityFrameworkCore;
namespace FileService.DAL.Repositories;
public abstract class UnitOfWorkBase(DbContext dbContext) : IUnitOfWork
{
    private readonly DbContext _dbContext = dbContext;

    public Task SaveChanges(CancellationToken cancellationToken)
    {
        return _dbContext.SaveChangesAsync(cancellationToken);
    }
}
