namespace Persistor.Core
{
    public interface IMessageHandler
    {
        void Handle(byte[] e);
    }
}