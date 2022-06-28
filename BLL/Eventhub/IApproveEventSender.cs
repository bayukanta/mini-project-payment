using System;
using System.Threading.Tasks;

namespace BLL.Eventhub
{
    public interface IApproveEventSender : IDisposable
    {
        Task CreateEventBatchAsync();
        bool AddMessage(object data);
        Task SendMessage();
    }
}
