using System.ComponentModel.DataAnnotations;

namespace DomainService.DTOs;

public class RelatorioVendaExcelDTO
{
    [Display(Name = "Venda", Order = 1)]
    public string NumeroVenda { get; set; } = string.Empty;

    [Display(Name = "Operador", Order = 2)]
    public string Operador { get; set; } = string.Empty;

    [Display(Name = "Consumidor", Order = 3)]
    public string Consumidor { get; set; } = string.Empty;

    [Display(Name = "Pagamento", Order = 4)]
    public string FormaPagamento { get; set; } = string.Empty;

    [Display(Name = "Produtos", Order = 5)]
    public string Produtos { get; set; } = string.Empty;

    [Display(Name = "Data da Venda", Order = 6)]
    [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
    public DateTime DataVenda { get; set; }

    [Display(Name = "Desconto", Order = 7)]
    [DisplayFormat(DataFormatString = "R$ #,##0.00")]
    public decimal Desconto { get; set; }

    [Display(Name = "Valor Total", Order = 8)]
    [DisplayFormat(DataFormatString = "R$ #,##0.00")]
    public decimal ValorTotal { get; set; }

    [Display(Name = "Status", Order = 9)]
    public string Status { get; set; } = string.Empty;
}