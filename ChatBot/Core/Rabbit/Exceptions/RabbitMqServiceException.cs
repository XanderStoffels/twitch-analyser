using System;

namespace ChatBot.Core.Rabbit.Exceptions
{
    public class RabbitMqServiceException : Exception
    {
        public RabbitMqServiceException()
        {
        }

        public RabbitMqServiceException(string message) : base(message)
        {
        }

        public RabbitMqServiceException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}