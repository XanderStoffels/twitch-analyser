using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Persistor.Core.Data
{
    public class MessageModel
    {
        [BsonId]
        public Guid Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}