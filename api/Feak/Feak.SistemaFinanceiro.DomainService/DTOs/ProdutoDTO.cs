using System.ComponentModel;
using DomainService.Entities;

namespace DomainService.DTOs;

public class ProdutoDTO
{
    public Guid Id { get; set; }

    [DisplayName("Código barras")]
    public string CodigoBarras { get; set; } = string.Empty;

    [DisplayName("Descrição")]
    public string Descricao { get; set; } = string.Empty;

    [DisplayName("Preço de custo")]
    public decimal PrecoCusto { get; set; }

    [DisplayName("Preço de venda")]
    public decimal PrecoVenda { get; set; }

    [DisplayName("Estoque atual")]
    public int EstoqueAtual { get; set; }

    public DateTime DhInclusao { get; set; }

    public DateTime? DhExclusao { get; set; }

    public Produto ToEntity()
    {
        return new Produto
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