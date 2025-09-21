using DomainService.Entities;

namespace DomainService.Interfaces.Services;

public interface IBaseDomainService<TEntity> where TEntity : class
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync (TEntity entity);
    
    Task<TEntity> GetByIdAsync(Guid id);
    Task<List<TEntity>> GetAllAsync();
}