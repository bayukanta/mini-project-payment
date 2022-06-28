
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using DAL.Model;

namespace DAL.Repository
{
    public class UnitOfWork : IDisposable, IUnitOfWork
    {
        private readonly PaymentDbContext dbContext; 


        public IBaseRepository<Payments> PaymentRepository { get; }


        private IHttpContextAccessor _contextAccessor;

        public UnitOfWork(PaymentDbContext context,
            IHttpContextAccessor httpContextAccessor)
        {
            dbContext = context;
            PaymentRepository = new BaseRepository<Payments>(context);
            _contextAccessor = httpContextAccessor;
        }
        public void Save()
        {
            dbContext.SaveChanges();
        }

        public Task SaveAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbContext.SaveChangesAsync(cancellationToken);
        }
        public IDbContextTransaction StartNewTransaction()
        {
            return dbContext.Database.BeginTransaction();
        }

        public Task<IDbContextTransaction> StartNewTransactionAsync()
        {
            return dbContext.Database.BeginTransactionAsync();
        }

        public Task<int> ExecuteSqlCommandAsync(string sql, object[] parameters, CancellationToken cancellationToken = default(CancellationToken))
        {
            return dbContext.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
        }
        public string GetUsernameFromAuthorizationToken()
        {
            var nameClaim = _contextAccessor.HttpContext.User.FindFirst("username");
            if (nameClaim == null) return "";
            else return nameClaim.Value;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    dbContext?.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}
