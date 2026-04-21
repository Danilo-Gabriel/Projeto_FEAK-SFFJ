namespace DomainService.DTOs;

public class VendaDTO
{
    public Guid Id { get; set; }
    public string NumeroVenda { get; set; } = string.Empty;
    public string Operador { get; set; } = string.Empty;
    public string Consumidor { get; set; } = string.Empty;
    public string FormaPagamento { get; set; } = string.Empty;
    public decimal Subtotal { get; set; }
    public decimal DescontoTotal { get; set; }
    public decimal Acrescimo { get; set; }
    public decimal Total { get; set; }
    public bool Cancelada { get; set; }
    public DateTime DhInclusao { get; set; }
    public DateTime? DhCancelamento { get; set; }
    public List<VendaItemDTO> Itens { get; set; } = new();
}