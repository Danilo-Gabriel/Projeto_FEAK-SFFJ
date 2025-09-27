using DomainService.Entities;
using DomainService.Interfaces;
using Feak.SistemaFinanceiro.Persistencia.Context;

namespace Feak.SistemaFinanceiro.Persistencia.services;

public class BaseRepository<TEntity> : IBaseRepository<TEntity> where TEntity : BaseEntity
{
    protected readonly ApplicationDbcontext _context;

    //todo implementar regras
    public BaseRepository(ApplicationDbcontext context)
    {
        _context = context;
    }

    public Task<TEntity> CreateAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> UpdateAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> DeleteAsync(TEntity entity)
    {
        throw new NotImplementedException();
    }

    public Task<TEntity> GetByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<List<TEntity>> GetAllAsync()
    {
        throw new NotImplementedException();
    }
}