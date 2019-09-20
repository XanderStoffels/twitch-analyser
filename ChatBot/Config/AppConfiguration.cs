namespace ChatBot.Config
{
    public class AppConfiguration
    {
        public string Username { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public string ClientId { get; set; }
        public int StreamPollingInterval { get; set; }
        public int StartupTimeout { get; set; }
        public string RabbitMqChannel { get; set; }
        public int MaxConcurrentChannels { get; set; }
    }
}