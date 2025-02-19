using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace CommentsService.DAL
{
    public class MongoDBContext
    {
        public readonly IMongoDatabase Database;

        public MongoDBContext(IOptions<MongoDBSettings> mongoDBSettings)
        {
            var client = new MongoClient(mongoDBSettings.Value.Connection);
            Database = client.GetDatabase(mongoDBSettings.Value.DatabaseName);
        }
    }
}
