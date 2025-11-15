using DomainService.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Feak.SistemaFinanceiro.Persistencia.Helper;

public static class PaginacaoHelper
{
    public static async Task<PaginacaoResponse<T>> PaginarAsync<T>(this IQueryable<T> queryable,
        PaginacaoGenericaDTO paginacao)
    {
        var totalItens = await queryable.CountAsync();
        
        var itens = await queryable
            .Skip((paginacao.PaginaAtual - 1) * paginacao.TamanhoPagina)
            .Take(paginacao.TamanhoPagina)
            .ToListAsync();

        return new PaginacaoResponse<T>
        {
            PaginaAtual = paginacao.PaginaAtual,
            TamanhoPagina = paginacao.TamanhoPagina,
            TotalItens = totalItens,
            Itens = itens
        };
    } 
}