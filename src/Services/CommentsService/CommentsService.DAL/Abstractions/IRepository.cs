using CommentsService.Domain.Models;

namespace CommentsService.DAL.Abstractions
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetByIdAsync(Guid id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(Guid id, T entity);
        Task DeleteAsync(Guid id);
    }
}
