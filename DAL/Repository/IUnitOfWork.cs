using Microsoft.EntityFrameworkCore.Storage;
using DAL.Model;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IUnitOfWork
    {
        public IBaseRepository<Payments> PaymentRepository { get; }
        void Save();
        Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken));
        IDbContextTransaction StartNewTransaction();
        Task<IDbContextTransaction> StartNewTransactionAsync();
        Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = default(CancellationToken));

        string GetUsernameFromAuthorizationToken();
    }
}
