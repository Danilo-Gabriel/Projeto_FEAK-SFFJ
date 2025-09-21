using System.ComponentModel.DataAnnotations;

namespace DomainService.Entities;

public abstract class BaseEntity
{
    [Key]
    public Guid Id {get; set;}
    public DateTime DhInclusao { get; set; }
    public DateTime? DhExclusao {get; set;}
}