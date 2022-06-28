using Microsoft.EntityFrameworkCore;
using DAL.Model;

namespace DAL
{
    public class PaymentDbContext : DbContext
    {
        public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder builder)
        {

        }
        public DbSet<Payments> Payments { get; set; }

    }
}
