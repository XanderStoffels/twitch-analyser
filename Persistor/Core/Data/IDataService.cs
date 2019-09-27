using System.Collections.Generic;
using System.Threading.Tasks;
using Shared;

namespace Persistor.Core.Data
{
    public interface IDataService
    {
        void SaveMessage(MessageModel message);
        void SaveMessages(IEnumerable<MessageModel> messages);
        
        Task SaveMessageAsync(MessageModel message);
        Task SaveMessagesAsync(IEnumerable<MessageModel> messages);
    }
}