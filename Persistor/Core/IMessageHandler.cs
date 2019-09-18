using System.Threading.Tasks;

namespace Persistor.Core
{
    public interface IMessageHandler
    {
        Task Handle(byte[] e);
    }
}