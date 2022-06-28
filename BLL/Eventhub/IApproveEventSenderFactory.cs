using Microsoft.Extensions.Configuration;

namespace BLL.Eventhub
{
    public interface IApproveEventSenderFactory
    {
        IApproveEventSender Create(IConfiguration config, string eventHubName);
    }
}
