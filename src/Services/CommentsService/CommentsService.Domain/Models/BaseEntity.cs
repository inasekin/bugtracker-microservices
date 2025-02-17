using MongoDB.Bson.Serialization.Attributes;

namespace CommentsService.Domain.Models
{
    public abstract class BaseEntity
    {
        [BsonId]
        public Guid Id { get; set; }
    }
}
