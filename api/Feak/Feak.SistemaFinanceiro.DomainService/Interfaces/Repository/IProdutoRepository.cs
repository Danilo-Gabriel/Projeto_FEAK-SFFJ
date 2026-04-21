using DomainService.Entities;

namespace DomainService.Interfaces;

public interface IProdutoRepository : IBaseRepository<Produto>
{
    Task<Produto?> GetByIdAsync(Guid id);
    Task<Produto?> GetByDescricaoAsync(string descricao);
    Task<Produto?> GetByCodigoBarrasAsync(string codigoBarras);
    Task<List<Produto>> ObterProdutos();
    Task<Produto> CadastrarProduto(Produto dados);
    Task<Produto> AtualizarProduto(Produto dados);
    Task<Produto> InativarProduto(Guid id);
}