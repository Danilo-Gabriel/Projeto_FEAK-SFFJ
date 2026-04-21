using System.ComponentModel.DataAnnotations.Schema;
using DomainService.DTOs;

namespace DomainService.Entities;

public class Venda : BaseEntity
{
    [Column("numero_venda")]
    public string NumeroVenda { get; set; } = string.Empty;

    [Column("operador")]
    public string Operador { get; set; } = string.Empty;

    [Column("consumidor")]
    public string Consumidor { get; set; } = string.Empty;

    [Column("forma_pagamento")]
    public string FormaPagamento { get; set; } = string.Empty;

    [Column("subtotal")]
    public decimal Subtotal { get; set; }

    [Column("desconto_total")]
    public decimal DescontoTotal { get; set; }

    [Column("acrescimo")]
    public decimal Acrescimo { get; set; }

    [Column("total")]
    public decimal Total { get; set; }

    [Column("cancelada")]
    public bool Cancelada { get; set; }

    [Column("dh_cancelamento")]
    public DateTime? DhCancelamento { get; set; }

    public ICollection<VendaItem> Itens { get; set; } = new List<VendaItem>();

    public VendaDTO ToDTO()
    {
        return new VendaDTO
        {
            Id = Id,
            NumeroVenda = NumeroVenda,
            Operador = Operador,
            Consumidor = Consumidor,
            FormaPagamento = FormaPagamento,
            Subtotal = Subtotal,
            DescontoTotal = DescontoTotal,
            Acrescimo = Acrescimo,
            Total = Total,
            Cancelada = Cancelada,
            DhInclusao = DhInclusao,
            DhCancelamento = DhCancelamento,
            Itens = Itens.Select(item => item.ToDTO()).ToList()
        };
    }
}