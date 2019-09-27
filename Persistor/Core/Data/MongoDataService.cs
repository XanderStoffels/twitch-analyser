using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Shared;

namespace Persistor.Core.Data
{
    public class MongoDataService : IDataService
    {
        private static readonly string DB_NAME = "twitch";
        private static readonly string TABLE_NAME = "messages";
        private readonly IMongoDatabase _db;
        
        public MongoDataService(IConfiguration config)
        {
            var client = new MongoClient(config["ConnectionString"]);
            this._db = client.GetDatabase(DB_NAME);
        }
        
        public void SaveMessage(MessageModel message)
        {
            var collection = _db.GetCollection<MessageModel>(TABLE_NAME);
            collection.InsertOne(message);
        }

        public void SaveMessages(IEnumerable<MessageModel> messages)
        {
            var collection = _db.GetCollection<MessageModel>(TABLE_NAME);
            collection.InsertMany(messages);
        }

        public Task SaveMessageAsync(MessageModel message)
        {
            var collection = _db.GetCollection<MessageModel>(TABLE_NAME);
            return collection.InsertOneAsync(message);
        }

        public Task SaveMessagesAsync(IEnumerable<MessageModel> messages)
        {
            var collection = _db.GetCollection<MessageModel>(TABLE_NAME);
            return collection.InsertManyAsync(messages);
        }


    }
}