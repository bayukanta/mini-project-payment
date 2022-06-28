using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using DAL.Model;
using DAL.Repository;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using BLL.Eventhub;

namespace BLL.Service
{
    public class PaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        private readonly IApproveEventSenderFactory _msgSenderFactory;
        public PaymentService(IUnitOfWork unitOfWork, IConfiguration config, IApproveEventSenderFactory msg)
        {
            _unitOfWork = unitOfWork;
            _msgSenderFactory = msg;
            _config = config;
        }

        public async Task<List<Payments>> GetAll()
        {
            return await _unitOfWork.PaymentRepository.GetAll().ToListAsync();
        }

        public async Task<List<Payments>> GetAllPaymentByUserAsync(Guid userId)
        {
            return await _unitOfWork.PaymentRepository.GetAll().Where(x=> x.UserId == userId).ToListAsync();
        }

        public async Task<Payments> GetPaymentByOrderId(Guid orderId)
        {
            return await _unitOfWork.PaymentRepository.GetSingleAsync(x => x.OrderId == orderId);
        }

        public async Task Pay(Payments payment)
        {
            bool isExist = _unitOfWork.PaymentRepository
                .GetAll()
                .Where(x=> x.OrderId == payment.OrderId)
                .Where(x=> x.UserId == payment.UserId)
                .Any();
            if (!isExist)
            {
                throw new Exception($"Payment with id {payment.Id} not exist");
                
            }
            payment.Status = "Paid";
            _unitOfWork.PaymentRepository.Edit(payment);
            await _unitOfWork.SaveAsync();
            await SendToEventhub(payment);
        }

        private async Task SendToEventhub(Payments payment)
        {
            string topic = _config.GetValue<string>("EventHub:ApprovedEvent");

            //create event hub producer
            using IApproveEventSender message = _msgSenderFactory.Create(_config, topic);

            //create batch
            await message.CreateEventBatchAsync();

            //add message, ini bisa banyak sekaligus
            message.AddMessage(payment);

            //send message
            await message.SendMessage();
        }
    }
}
