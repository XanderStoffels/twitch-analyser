using System;

namespace Persistor.Data
{
    public class TwitchChatMessage
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}