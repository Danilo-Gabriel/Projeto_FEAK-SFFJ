using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;
using DomainService.Interfaces;
using DomainService.Interfaces.Services;

namespace DomainService.services;

public class VendaDomainService : BaseDomainService<Venda>, IVendaDomainService
{
    private readonly IProdutoRepository _produtoRepository;
    private readonly IVendaRepository _vendaRepository;

    public VendaDomainService(IVendaRepository vendaRepository, IProdutoRepository produtoRepository) : base(vendaRepository)
    {
        _vendaRepository = vendaRepository;
        _produtoRepository = produtoRepository;
    }

    public async Task<ServiceResponse<VendaDTO>> RegistrarVenda(RegistrarVendaRequest request)
    {
        var serviceResponse = new ServiceResponse<VendaDTO>();

        try
        {
            if (request.Itens == null || request.Itens.Count == 0)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "Informe pelo menos um item para a venda.";
                return serviceResponse;
            }

            var itensVenda = new List<VendaItem>();
            decimal subtotal = 0;
            decimal descontoTotal = 0;

            foreach (var itemRequest in request.Itens)
            {
                var produto = await _produtoRepository.GetByIdAsync(itemRequest.ProdutoId);
                if (produto == null || produto.DhExclusao != null)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Mensagem = $"Produto {itemRequest.DescricaoProduto} não encontrado.";
                    return serviceResponse;
                }

                if (produto.EstoqueAtual < itemRequest.Quantidade)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Mensagem = $"Estoque insuficiente para o produto {produto.Descricao}.";
                    return serviceResponse;
                }

                var subtotalItem = itemRequest.PrecoUnitario * itemRequest.Quantidade;
                var descontoPercentualValor = subtotalItem * (itemRequest.DescontoPercentual / 100);
                var descontoItem = itemRequest.DescontoValor + descontoPercentualValor;
                var totalItem = subtotalItem - descontoItem;

                if (totalItem < 0)
                {
                    serviceResponse.Success = false;
                    serviceResponse.Mensagem = $"Desconto inválido para o produto {produto.Descricao}.";
                    return serviceResponse;
                }

                subtotal += totalItem;
                descontoTotal += descontoItem;

                itensVenda.Add(new VendaItem
                {
                    Id = Guid.NewGuid(),
                    VendaId = Guid.Empty,
                    ProdutoId = produto.Id,
                    CodigoProduto = itemRequest.CodigoProduto,
                    DescricaoProduto = itemRequest.DescricaoProduto,
                    Quantidade = itemRequest.Quantidade,
                    PrecoUnitario = itemRequest.PrecoUnitario,
                    DescontoValor = itemRequest.DescontoValor,
                    DescontoPercentual = itemRequest.DescontoPercentual,
                    TotalItem = totalItem,
                    DhInclusao = DateTime.UtcNow
                });
            }

            var venda = new Venda
            {
                Id = Guid.NewGuid(),
                NumeroVenda = await _vendaRepository.GerarNumeroVendaAsync(),
                Consumidor = request.Consumidor.Trim(),
                FormaPagamento = request.FormaPagamento.Trim().ToUpperInvariant(),
                Subtotal = subtotal,
                DescontoTotal = descontoTotal,
                Acrescimo = request.Acrescimo,
                Total = subtotal + request.Acrescimo,
                DhInclusao = DateTime.UtcNow,
                Itens = itensVenda
            };

            foreach (var item in itensVenda)
            {
                item.VendaId = venda.Id;
            }

            var vendaRegistrada = await _vendaRepository.RegistrarVendaAsync(venda, itensVenda);
            serviceResponse.Dados = vendaRegistrada.ToDTO();
            serviceResponse.Mensagem = "Venda registrada com sucesso.";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = $"Erro ao registrar venda: {ex.Message}";
        }

        return serviceResponse;
    }
}