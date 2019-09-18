using System.Collections.Generic;

namespace Persistor.Data
{
    public class TwitchUser
    {
        public string Id { get; set; }
        public List<TwitchUsername> Usernames { get; set; }
        public List<ChatMessage> ChatMessages { get; set; }

        
        public TwitchUser()
        {
            Usernames = new List<TwitchUsername>();
            ChatMessages = new List<ChatMessage>();
        }

    }
}