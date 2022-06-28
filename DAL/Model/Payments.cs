using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DAL.Model
{
    public class Payments
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public int Amount { get; set; }
        public string Status { get; set; }
        public DateTime CreatedDate{get;set;}

        public Payments()
        {
            CreatedDate = DateTime.Now;
        }
    }
}
