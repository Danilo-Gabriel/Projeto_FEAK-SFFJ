using System.ComponentModel.DataAnnotations;
using DomainService.Entities;

namespace DomainService.DTOs.Request;

public class ProdutoRequest
{
    public string CodigoBarras { get; set; } = string.Empty;

    [Required(ErrorMessage = "Campo obrigatório")]
    [RegularExpression(@".*\S.*", ErrorMessage = "Descrição obrigatória")]
    public string Descricao { get; set; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Preço de custo inválido")]
    public decimal PrecoCusto { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Preço de venda inválido")]
    public decimal PrecoVenda { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Estoque inválido")]
    public int EstoqueAtual { get; set; }

    public Produto ToEntity()
    {
        return new Produto
        {
            Id = Guid.NewGuid(),
            CodigoBarras = CodigoBarras.Trim(),
            Descricao = Descricao.Trim(),
            PrecoCusto = PrecoCusto,
            PrecoVenda = PrecoVenda,
            EstoqueAtual = EstoqueAtual
        };
    }
}
