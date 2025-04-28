using FileService.DAL.Entities;

namespace FileService.DAL.Repositories;
public interface IRepository<T> where T : EntityBase
{
    Task<T?> GetAsync(Guid guid, CancellationToken cancellationToken = default);
    IEnumerable<T> GetAll(int take, int skip);
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    Task RemoveAsync(Guid guid, CancellationToken cancellationToken = default);
}
