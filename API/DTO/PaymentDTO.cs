using System;

namespace API.DTO
{
    public class PaymentDTO
    {

        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Status { get; set; }
        public int Amount { get; set; }
        

    }
    
}
