using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using DomainService.Entities;

namespace DomainService.DTOs;

public class ProdutoDTO
{
    public Guid Id { get; set; }

    [DisplayName("Código barras")]
    public string CodigoBarras { get; set; } = string.Empty;

    [DisplayName("Descrição")]
    [Required(ErrorMessage = "Descrição obrigatória")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Descrição obrigatória")]
    public string Descricao { get; set; } = string.Empty;

    [DisplayName("Preço de custo")]
    [Range(0, double.MaxValue, ErrorMessage = "Preço de custo inválido")]
    public decimal PrecoCusto { get; set; }

    [DisplayName("Preço de venda")]
    [Range(0, double.MaxValue, ErrorMessage = "Preço de venda inválido")]
    public decimal PrecoVenda { get; set; }

    [DisplayName("Estoque atual")]
    [Range(0, int.MaxValue, ErrorMessage = "Estoque inválido")]
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
