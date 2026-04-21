using System.ComponentModel.DataAnnotations.Schema;
using DomainService.DTOs;

namespace DomainService.Entities;

public class Produto : BaseEntity
{
    [Column("codigo_barras")]
    public string CodigoBarras { get; set; } = string.Empty;

    [Column("descricao")]
    public string Descricao { get; set; } = string.Empty;

    [Column("preco_custo")]
    public decimal PrecoCusto { get; set; }

    [Column("preco_venda")]
    public decimal PrecoVenda { get; set; }

    [Column("estoque_atual")]
    public int EstoqueAtual { get; set; }

    public ProdutoDTO ToDTO()
    {
        return new ProdutoDTO
        {
            Id = Id,
            CodigoBarras = CodigoBarras,
            Descricao = Descricao,
            PrecoCusto = PrecoCusto,
            PrecoVenda = PrecoVenda,
            EstoqueAtual = EstoqueAtual,
            DhInclusao = DhInclusao,
            DhExclusao = DhExclusao
        };
    }
}