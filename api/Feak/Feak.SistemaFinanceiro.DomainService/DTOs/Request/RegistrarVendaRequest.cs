using System.ComponentModel.DataAnnotations;

namespace DomainService.DTOs.Request;

public class RegistrarVendaRequest
{
    [Required(ErrorMessage = "Consumidor obrigatório")]
    public string Consumidor { get; set; } = string.Empty;

    [Required(ErrorMessage = "Forma de pagamento obrigatória")]
    public string FormaPagamento { get; set; } = string.Empty;

    public decimal Acrescimo { get; set; }

    [MinLength(1, ErrorMessage = "Informe pelo menos um item")]
    public List<RegistrarVendaItemRequest> Itens { get; set; } = new();
}