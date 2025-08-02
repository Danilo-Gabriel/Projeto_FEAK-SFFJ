using FeakApi.models.repository.interfaces;
using Microsoft.EntityFrameworkCore;
using System;

namespace FeakApi.models.repository
{
    public class EfRepository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public EfRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>();
        }

        Task IRepository<T>.AddAsync(T entity)
        {
            throw new NotImplementedException();
        }

        Task<List<T>> IRepository<T>.GetAllAsync()
        {
            throw new NotImplementedException();
        }

        Task<T> IRepository<T>.GetByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Remove(T entity)
        {
            throw new NotImplementedException();
        }

        Task IRepository<T>.SaveChangesAsync()
        {
            throw new NotImplementedException();
        }

        void IRepository<T>.Update(T entity)
        {
            throw new NotImplementedException();
        }
    }
}
