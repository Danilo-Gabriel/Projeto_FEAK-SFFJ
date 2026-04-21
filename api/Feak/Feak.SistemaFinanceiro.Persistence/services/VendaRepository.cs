using DomainService.Entities;
using DomainService.Interfaces;
using Feak.SistemaFinanceiro.Persistencia.Context;
using Microsoft.EntityFrameworkCore;

namespace Feak.SistemaFinanceiro.Persistencia.services;

public class VendaRepository : BaseRepository<Venda>, IVendaRepository
{
    public VendaRepository(ApplicationDbcontext context) : base(context)
    {
    }

    public async Task<string> GerarNumeroVendaAsync()
    {
        var ultimaVenda = await _context.Vendas
            .OrderByDescending(x => x.DhInclusao)
            .Select(x => x.NumeroVenda)
            .FirstOrDefaultAsync();

        var sequencia = 1;
        if (!string.IsNullOrWhiteSpace(ultimaVenda) && int.TryParse(ultimaVenda.Replace("VD", string.Empty), out var numeroAtual))
        {
            sequencia = numeroAtual + 1;
        }

        return $"VD{sequencia:000000}";
    }

    public async Task<Venda> RegistrarVendaAsync(Venda venda, List<VendaItem> itens)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var produtoIds = itens.Select(x => x.ProdutoId).Distinct().ToList();
            var produtos = await _context.Produtos.Where(x => produtoIds.Contains(x.Id)).ToListAsync();

            foreach (var item in itens)
            {
                var produto = produtos.First(x => x.Id == item.ProdutoId);
                produto.EstoqueAtual -= item.Quantidade;
                _context.Produtos.Update(produto);
            }

            await _context.Vendas.AddAsync(venda);
            await _context.VendaItens.AddRangeAsync(itens);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            venda.Itens = itens;
            return venda;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}