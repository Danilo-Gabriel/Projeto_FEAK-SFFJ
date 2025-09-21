using DomainService.Entities;
using DomainService.Interfaces;
using DomainService.Interfaces.Services;

namespace DomainService.services;

public class BaseDomainService<TEntity> : IBaseDomainService<TEntity> where TEntity : BaseEntity
{
    public readonly IBaseRepository<TEntity> _repository;
    
    public BaseDomainService(IBaseRepository<TEntity> repository)
    {
        _repository = repository;
    }
    
    public async Task<TEntity> CreateAsync(TEntity entity)
    {
        return await _repository.CreateAsync(entity);
    }

    public async Task<TEntity> UpdateAsync(TEntity entity)
    {
        return await _repository.UpdateAsync(entity);
    }

    public async Task<TEntity> DeleteAsync(TEntity entity)
    {
        return await _repository.DeleteAsync(entity);
    }

    public async Task<TEntity> GetByIdAsync(Guid id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<List<TEntity>> GetAllAsync()
    {
        return await _repository.GetAllAsync();
    }
}