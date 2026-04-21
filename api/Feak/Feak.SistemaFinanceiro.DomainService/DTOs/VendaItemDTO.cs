namespace DomainService.DTOs;

public class VendaItemDTO
{
    public Guid Id { get; set; }
    public Guid VendaId { get; set; }
    public Guid ProdutoId { get; set; }
    public string CodigoProduto { get; set; } = string.Empty;
    public string DescricaoProduto { get; set; } = string.Empty;
    public int Quantidade { get; set; }
    public decimal PrecoUnitario { get; set; }
    public decimal DescontoValor { get; set; }
    public decimal DescontoPercentual { get; set; }
    public decimal TotalItem { get; set; }
}