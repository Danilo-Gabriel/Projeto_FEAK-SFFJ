using System.ComponentModel.DataAnnotations.Schema;
using DomainService.DTOs;

namespace DomainService.Entities;

public class VendaItem : BaseEntity
{
    [Column("venda_id")]
    public Guid VendaId { get; set; }

    [Column("produto_id")]
    public Guid ProdutoId { get; set; }

    [Column("codigo_produto")]
    public string CodigoProduto { get; set; } = string.Empty;

    [Column("descricao_produto")]
    public string DescricaoProduto { get; set; } = string.Empty;

    [Column("quantidade")]
    public int Quantidade { get; set; }

    [Column("preco_unitario")]
    public decimal PrecoUnitario { get; set; }

    [Column("desconto_valor")]
    public decimal DescontoValor { get; set; }

    [Column("desconto_percentual")]
    public decimal DescontoPercentual { get; set; }

    [Column("total_item")]
    public decimal TotalItem { get; set; }

    public Venda? Venda { get; set; }
    public Produto? Produto { get; set; }

    public VendaItemDTO ToDTO()
    {
        return new VendaItemDTO
        {
            Id = Id,
            VendaId = VendaId,
            ProdutoId = ProdutoId,
            CodigoProduto = CodigoProduto,
            DescricaoProduto = DescricaoProduto,
            Quantidade = Quantidade,
            PrecoUnitario = PrecoUnitario,
            DescontoValor = DescontoValor,
            DescontoPercentual = DescontoPercentual,
            TotalItem = TotalItem
        };
    }
}