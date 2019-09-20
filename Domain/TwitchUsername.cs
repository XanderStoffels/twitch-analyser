using System;

namespace Persistor.Data
{
    public class TwitchUsername
    {
        public string Id { get; set; }
        public string Username { get; set; }
        public DateTime FirstSeen { get; set; }
        public TwitchUser User { get; set; }
        public string UserId { get; set; }
    }
}