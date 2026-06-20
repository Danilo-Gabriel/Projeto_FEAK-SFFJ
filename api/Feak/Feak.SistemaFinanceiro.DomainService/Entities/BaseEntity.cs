using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DomainService.Entities;

public abstract class BaseEntity
{

    //MAPEAR PARA PADRÃO SNAKE_CASE COM  [Column("")]
    [Key]
    [Column("id")]
    public Guid Id {get; set;}
    [Column("dh_inclusao")]
    public DateTime DhInclusao { get; set; }
    [Column("dh_exclusao")]
    public DateTime? DhExclusao {get; set;}
}
