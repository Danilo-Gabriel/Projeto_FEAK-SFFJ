using DomainService.Entities;

namespace DomainService.Interfaces;

public interface IBaseRepository<TEntity> where TEntity : BaseEntity 
{
    Task<TEntity> CreateAsync(TEntity entity);
    Task<TEntity> UpdateAsync(TEntity entity);
    Task<TEntity> DeleteAsync (TEntity entity);
    
    Task<TEntity> GetByIdAsync(Guid id);
    Task<List<TEntity>> GetAllAsync();
}