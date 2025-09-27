using DomainService.Entities;

namespace DomainService.Interfaces;

public interface IUnitOfWork<TEntity> where TEntity : BaseEntity
{
    Task Commit(CancellationToken cancellationToken);
}