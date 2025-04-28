using FileService.DAL.Entities;
using Microsoft.EntityFrameworkCore;

namespace FileService.DAL.Repositories.Implementation;
public class RepositoryBase<T> : IRepository<T> where T : EntityBase
{
    protected readonly FileDbContext DbContext;
    protected readonly DbSet<T> DbSet;

    public RepositoryBase(FileDbContext dbContext)
    {
        DbContext = dbContext;
        DbSet = DbContext.Set<T>();
    }

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken)
    {
        var entry = await DbSet.AddAsync(entity, cancellationToken);
        var res = await GetAsync(entry.Entity.Id, cancellationToken);
        return res!;
    }

    public IEnumerable<T> GetAll(int take, int skip)
    {
        return DbSet
            .OrderBy(t => t.Id)
            .Skip(skip)
            .Take(take)
            .AsEnumerable();
    }

    public async Task<T?> GetAsync(Guid guid, CancellationToken cancellationToken)
    {
        var entity = await DbSet.FindAsync([guid], cancellationToken);

        return entity;
    }

    public async Task RemoveAsync(Guid guid, CancellationToken cancellationToken)
    {
        var entity = await DbSet.FindAsync([guid], cancellationToken: cancellationToken);
        if (entity != null)
        {
            DbSet.Remove(entity);
            await DbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
