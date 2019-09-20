using System;

namespace StatApi.Data
{
    public class ChatMessage
    {
        public string Id { get; set; }
        public string Message { get; set; }
        public DateTime ReceivedOn { get; set; }

        public TwitchUser Sender { get; set; }
        public string SenderId { get; set; }
        public Channel Channel { get; set; }
        public string ChannelId { get; set; }
    }
}