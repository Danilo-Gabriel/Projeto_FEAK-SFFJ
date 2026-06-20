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
        var sequencia = await _context.Database
            .SqlQueryRaw<long>("SELECT nextval('feak_sf.numero_venda_seq') AS \"Value\"")
            .SingleAsync();

        return $"VD{sequencia:000000}";
    }

    public async Task<List<Venda>> ListarVendasAsync()
    {
        return await _context.Vendas
            .Include(x => x.Itens)
            .Where(x => x.DhExclusao == null)
            .OrderByDescending(x => x.DhInclusao)
            .ToListAsync();
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

    public async Task<Venda?> ObterVendaComItensAsync(Guid vendaId)
    {
        return await _context.Vendas
            .Include(x => x.Itens)
            .FirstOrDefaultAsync(x => x.Id == vendaId);
    }

    public async Task<Venda> CancelarVendaAsync(Venda venda)
    {
        await using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var produtoIds = venda.Itens.Select(x => x.ProdutoId).Distinct().ToList();
            var produtos = await _context.Produtos.Where(x => produtoIds.Contains(x.Id)).ToListAsync();

            foreach (var item in venda.Itens)
            {
                var produto = produtos.FirstOrDefault(x => x.Id == item.ProdutoId);
                if (produto == null)
                {
                    continue;
                }

                produto.EstoqueAtual += item.Quantidade;
                _context.Produtos.Update(produto);
            }

            _context.Vendas.Update(venda);
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return venda;
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }
}
