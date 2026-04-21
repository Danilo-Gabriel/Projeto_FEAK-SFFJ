using System.ComponentModel;

namespace DomainService.DTOs;

public class ProdutoImportacaoDTO
{
    [DisplayName("CodigoBarras")]
    public string CodigoBarras { get; set; } = string.Empty;

    [DisplayName("Descricao")]
    public string Descricao { get; set; } = string.Empty;

    [DisplayName("PrecoCusto")]
    public decimal PrecoCusto { get; set; }

    [DisplayName("PrecoVenda")]
    public decimal PrecoVenda { get; set; }

    [DisplayName("EstoqueAtual")]
    public int EstoqueAtual { get; set; }
}