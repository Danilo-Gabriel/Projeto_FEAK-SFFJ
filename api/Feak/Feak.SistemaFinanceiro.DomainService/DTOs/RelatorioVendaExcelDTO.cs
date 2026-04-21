using System.ComponentModel.DataAnnotations;

namespace DomainService.DTOs;

public class RelatorioVendaExcelDTO
{
    [Display(Name = "Operador", Order = 1)]
    public string Operador { get; set; } = string.Empty;

    [Display(Name = "Data da Venda", Order = 2)]
    [DisplayFormat(DataFormatString = "dd/MM/yyyy HH:mm")]
    public DateTime DataVenda { get; set; }

    [Display(Name = "Desconto", Order = 3)]
    [DisplayFormat(DataFormatString = "R$ #,##0.00")]
    public decimal Desconto { get; set; }

    [Display(Name = "Valor Total", Order = 4)]
    [DisplayFormat(DataFormatString = "R$ #,##0.00")]
    public decimal ValorTotal { get; set; }
}