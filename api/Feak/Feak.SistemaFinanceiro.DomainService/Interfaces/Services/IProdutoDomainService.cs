using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;

namespace DomainService.Interfaces.Services;

public interface IProdutoDomainService : IBaseDomainService<Produto>
{
    Task<ServiceResponse<ProdutoDTO>> CadastrarProduto(ProdutoRequest dados);
    Task<ServiceResponse<ProdutoDTO>> AtualizarProduto(ProdutoDTO dados);
    Task<ServiceResponse<List<ProdutoDTO>>> ObterProdutos();
    Task<ServiceResponse<ProdutoDTO>> ObterProdutoPorId(Guid id);
    Task<ServiceResponse<ProdutoDTO>> ObterProdutoPorCodigoBarras(string codigoBarras);
    Task<ServiceResponse<ProdutoDTO>> InativarProdutoPorId(Guid id);
    Task<ServiceResponse<List<ProdutoDTO>>> ImportarProdutos(Stream arquivoStream);
    List<ProdutoImportacaoDTO> ObterTemplateImportacao();
}