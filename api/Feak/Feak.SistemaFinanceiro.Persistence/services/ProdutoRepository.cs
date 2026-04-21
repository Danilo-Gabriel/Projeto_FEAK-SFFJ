using DomainService.Entities;
using DomainService.Interfaces;
using Feak.SistemaFinanceiro.Persistencia.Context;
using Microsoft.EntityFrameworkCore;

namespace Feak.SistemaFinanceiro.Persistencia.services;

public class ProdutoRepository : BaseRepository<Produto>, IProdutoRepository
{
    public ProdutoRepository(ApplicationDbcontext context) : base(context)
    {
    }

    public async Task<Produto?> GetByIdAsync(Guid id)
    {
        return await _context.Produtos.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<Produto?> GetByDescricaoAsync(string descricao)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(x => x.Descricao.ToLower() == descricao.ToLower());
    }

    public async Task<Produto?> GetByCodigoBarrasAsync(string codigoBarras)
    {
        return await _context.Produtos
            .FirstOrDefaultAsync(x => x.CodigoBarras.ToLower() == codigoBarras.ToLower());
    }

    public async Task<List<Produto>> ObterProdutos()
    {
        return await _context.Produtos.ToListAsync();
    }

    public async Task<Produto> CadastrarProduto(Produto dados)
    {
        dados.DhInclusao = DateTime.UtcNow;
        await _context.Produtos.AddAsync(dados);
        await _context.SaveChangesAsync();
        return dados;
    }

    public async Task<Produto> AtualizarProduto(Produto dados)
    {
        _context.Produtos.Update(dados);
        await _context.SaveChangesAsync();
        return dados;
    }

    public async Task<Produto> InativarProduto(Guid id)
    {
        var produto = await _context.Produtos.FindAsync(id);
        if (produto == null)
        {
            throw new Exception("Produto não encontrado");
        }

        produto.DhExclusao = DateTime.UtcNow;
        _context.Produtos.Update(produto);
        await _context.SaveChangesAsync();
        return produto;
    }
}