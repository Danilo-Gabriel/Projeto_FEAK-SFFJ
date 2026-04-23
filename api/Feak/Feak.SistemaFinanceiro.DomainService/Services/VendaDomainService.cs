using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;
using Feak.SistemaFinanceiro.DomainService.Helpers;
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

    public async Task<byte[]> ExportarRelatorioExcel()
    {
        var vendas = await _vendaRepository.ListarVendasAsync();
        var vendasFechadas = vendas
            .Where(x => x.DhExclusao == null)
            .OrderByDescending(x => x.DhInclusao)
            .Select(x => new RelatorioVendaExcelDTO
            {
                NumeroVenda = x.NumeroVenda,
                Operador = x.Operador,
                Consumidor = x.Consumidor,
                FormaPagamento = x.FormaPagamento,
                Produtos = x.Itens.Count == 0
                    ? "Sem itens"
                    : string.Join(" | ", x.Itens.Select(item => $"{item.Quantidade}x {item.DescricaoProduto}")),
                DataVenda = x.DhInclusao.ToLocalTime(),
                Desconto = x.DescontoTotal,
                ValorTotal = x.Total,
                Status = x.Cancelada ? "Cancelada" : "Fechada"
            })
            .ToList();

        return ExcelHelper<RelatorioVendaExcelDTO>.ExportarExcel(vendasFechadas, "Relatorio Vendas");
    }

    public async Task<ServiceResponse<List<VendaDTO>>> ListarVendas()
    {
        var serviceResponse = new ServiceResponse<List<VendaDTO>>();

        try
        {
            var vendas = await _vendaRepository.ListarVendasAsync();
            serviceResponse.Dados = vendas.Select(x => x.ToDTO()).ToList();
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = $"Erro ao listar vendas: {ex.Message}";
        }

        return serviceResponse;
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
                Operador = request.Operador.Trim(),
                Consumidor = request.Consumidor.Trim(),
                FormaPagamento = request.FormaPagamento.Trim().ToUpperInvariant(),
                Subtotal = subtotal,
                DescontoTotal = descontoTotal,
                Acrescimo = request.Acrescimo,
                Total = subtotal + request.Acrescimo,
                Cancelada = false,
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

    public async Task<ServiceResponse<VendaDTO>> CancelarVenda(Guid vendaId)
    {
        var serviceResponse = new ServiceResponse<VendaDTO>();

        try
        {
            var venda = await _vendaRepository.ObterVendaComItensAsync(vendaId);
            if (venda == null || venda.DhExclusao != null)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "Venda não encontrada.";
                return serviceResponse;
            }

            if (venda.Cancelada)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "A venda já foi cancelada.";
                return serviceResponse;
            }

            venda.Cancelada = true;
            venda.DhCancelamento = DateTime.UtcNow;

            var vendaCancelada = await _vendaRepository.CancelarVendaAsync(venda);
            serviceResponse.Dados = vendaCancelada.ToDTO();
            serviceResponse.Mensagem = "Venda cancelada com sucesso.";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = $"Erro ao cancelar venda: {ex.Message}";
        }

        return serviceResponse;
    }
}