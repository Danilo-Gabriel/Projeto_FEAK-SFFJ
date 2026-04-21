using System.ComponentModel.DataAnnotations;

namespace DomainService.DTOs.Request;

public class RegistrarVendaItemRequest
{
    [Required(ErrorMessage = "Produto obrigatório")]
    public Guid ProdutoId { get; set; }

    [Required(ErrorMessage = "Código do produto obrigatório")]
    public string CodigoProduto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Descrição do produto obrigatória")]
    public string DescricaoProduto { get; set; } = string.Empty;

    [Range(1, int.MaxValue, ErrorMessage = "Quantidade inválida")]
    public int Quantidade { get; set; }

    [Range(0, double.MaxValue, ErrorMessage = "Preço unitário inválido")]
    public decimal PrecoUnitario { get; set; }

    public decimal DescontoValor { get; set; }

    public decimal DescontoPercentual { get; set; }
}