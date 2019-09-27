using System;

namespace Shared
{
    public class TwitchChatMessage
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Channel { get; set; }
        public string Message { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}