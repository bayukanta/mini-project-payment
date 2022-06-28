using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Consumer;
using Azure.Messaging.EventHubs.Processor;
using Azure.Messaging.EventHubs.Producer;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using DAL.Model;
using DAL.Repository;
using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace BLL.Service
{
    public class PaymentListener : IHostedService, IDisposable
    {
        private readonly EventProcessorClient processor;
        private readonly ILogger _logger;
        private IUnitOfWork _unitOfWork;
        private IServiceScopeFactory _serviceScopeFactory;

        public PaymentListener(
            IConfiguration config, 
            ILogger<PaymentListener> logger, 
            IServiceScopeFactory serviceScopeFactory)
        {
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
            string topic = config.GetValue<string>("EventHub:PaymentEvent");
            string azureContainername = config.GetValue<string>("AzureStorage:AzureContainerEventHub");
            string eventHubConn = config.GetValue<string>("EventHub:ConnectionString");
            string azStorageConn = config.GetValue<string>("AzureStorage:ConnectionString");
            string consumerGroup = EventHubConsumerClient.DefaultConsumerGroupName;

            BlobContainerClient storageClient = new BlobContainerClient(azStorageConn, azureContainername);

            processor = new EventProcessorClient(storageClient, consumerGroup, eventHubConn, topic);

            processor.ProcessEventAsync += ProcessEventHandler;
            processor.ProcessErrorAsync += ProcessErrorHandler;
        }
        public async Task ProcessEventHandler(ProcessEventArgs eventArgs)
        {
            await eventArgs.UpdateCheckpointAsync(eventArgs.CancellationToken);
            var string_data = Encoding.UTF8.GetString(eventArgs.Data.Body.ToArray());
            JObject json_data = JObject.Parse(string_data);
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                IUnitOfWork _unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
                var oid = new Guid(json_data["OrderId"].ToString());
                var uid = new Guid(json_data["UserId"].ToString());
                bool isExist = _unitOfWork.PaymentRepository.GetAll().Where(x => x.OrderId == oid).Where(x => x.UserId == uid).Any();
                int price = json_data["OrderPrice"].ToObject<int>();
                if (!isExist)
                {
                    
                    var payment = new Payments()
                    {
                        OrderId = new Guid(json_data["OrderId"].ToString()),
                        UserId = new Guid(json_data["UserId"].ToString()),
                        Amount = price,
                        Status = "Unpaid"
                       
                    };
                    _unitOfWork.PaymentRepository.Add(payment);
                    await _unitOfWork.SaveAsync();
                }
                else if (isExist)
                {
                    var paymentFromDb = _unitOfWork.PaymentRepository.GetAll().Where(x => x.OrderId == oid).First();
                    paymentFromDb.Amount += price;
                    _unitOfWork.PaymentRepository.Edit(paymentFromDb);
                    await _unitOfWork.SaveAsync();
                }
                
                
                
                //Do your stuff
            }
        }

        public async Task ProcessErrorHandler(ProcessErrorEventArgs eventArgs)
        {
            _logger.LogError(eventArgs.Exception.Message);

        }


        public async Task StartAsync(CancellationToken cancellationToken)
        {
            await processor.StartProcessingAsync();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await processor.StopProcessingAsync();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~MessageListernerService()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
