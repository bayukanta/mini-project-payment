﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace DAL.Repository
{
    public interface IBaseRepository<T> where T : class
    {
        T Add(T entity);
        Task<T> AddAsync(T entity, CancellationToken cancellationToken = default(CancellationToken));
        void AddRange(IEnumerable<T> entities);
        void Edit(T entity);
        void Delete(T entity);
        void Delete(Expression<Func<T, bool>> where);
        Task<T> GetByIdAsync(Guid id);
        Task<T> GetSingleAsync(Expression<Func<T, bool>> where);
        IQueryable<T> GetAll();
        IQueryable<T> GetAllIncluding(params Expression<Func<T, object>>[] includeProperties);
        IQueryable<T> GetMany(Expression<Func<T, bool>> where);
        bool IsExist(Expression<Func<T, bool>> predicate);
    }
}
