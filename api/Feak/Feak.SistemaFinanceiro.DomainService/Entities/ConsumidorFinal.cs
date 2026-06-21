using System.ComponentModel.DataAnnotations.Schema;
using DomainService.DTOs;

namespace DomainService.Entities;

public class ConsumidorFinal : BaseEntity
{
    [Column("nome")]
    public string Nome { get; set; } = string.Empty;

    public ConsumidorFinalDTO ToDTO() => new()
    {
        Id = Id,
        Nome = Nome,
        DhInclusao = DhInclusao
    };
}
