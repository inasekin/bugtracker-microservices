using CommentsService.DAL.Abstractions;
using CommentsService.Domain.Models;
using MongoDB.Driver;

namespace CommentsService.DAL.Repositories
{
    public class MongoRepository<T> : IRepository<T> where T : BaseEntity
    {
        private readonly IMongoCollection<T> _collection;

        public MongoRepository(IMongoDatabase database, string collectionName)
        {
            _collection = database.GetCollection<T>(collectionName);
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _collection.Find(Builders<T>.Filter.Empty).ToListAsync();
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _collection.Find(Builders<T>.Filter.Eq(e => e.Id, id)).FirstOrDefaultAsync();
        }

        public async Task<T> AddAsync(T entity)
        {
            await _collection.InsertOneAsync(entity);
            return entity;
        }

        public async Task UpdateAsync(Guid id, T entity)
        {
            await _collection.ReplaceOneAsync(Builders<T>.Filter.Eq(e => e.Id, id), entity);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _collection.DeleteOneAsync(Builders<T>.Filter.Eq(e => e.Id, id));
        }
    }
}
