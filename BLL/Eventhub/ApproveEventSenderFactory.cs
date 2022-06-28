using Microsoft.Extensions.Configuration;

namespace BLL.Eventhub
{
    public class ApproveEventSenderFactory : IApproveEventSenderFactory
    {
        public IApproveEventSender Create(IConfiguration config, string eventHubName)
        {
            return new ApproveEventSender(config, eventHubName); ;
        }
    }
}

