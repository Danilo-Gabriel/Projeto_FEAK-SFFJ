using ClosedXML.Excel;
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

    public async Task<byte[]> ExportarRelatorioExcel()
    {
        var vendas = await _vendaRepository.ListarVendasAsync();
        var vendasFechadas = vendas
            .Where(x => x.DhExclusao == null && !x.Cancelada)
            .OrderByDescending(x => x.DhInclusao)
            .ToList();

        using var workbook = new XLWorkbook();

        var resumoWorksheet = workbook.Worksheets.Add("Resumo Produtos");
        resumoWorksheet.Cell(1, 1).Value = "Codigo Produto";
        resumoWorksheet.Cell(1, 2).Value = "Descricao Produto";
        resumoWorksheet.Cell(1, 3).Value = "Quantidade Vendida";
        resumoWorksheet.Cell(1, 4).Value = "Valor Total Vendido";
        resumoWorksheet.Cell(1, 5).Value = "Qtde Vendas";

        var resumoProdutos = vendasFechadas
            .SelectMany(venda => venda.Itens)
            .GroupBy(item => new { item.ProdutoId, item.CodigoProduto, item.DescricaoProduto })
            .Select(group => new
            {
                group.Key.CodigoProduto,
                group.Key.DescricaoProduto,
                QuantidadeVendida = group.Sum(x => x.Quantidade),
                ValorTotalVendido = group.Sum(x => x.TotalItem),
                QuantidadeVendas = group.Select(x => x.VendaId).Distinct().Count()
            })
            .OrderByDescending(x => x.QuantidadeVendida)
            .ThenBy(x => x.DescricaoProduto)
            .ToList();

        for (var linha = 0; linha < resumoProdutos.Count; linha++)
        {
            var item = resumoProdutos[linha];
            resumoWorksheet.Cell(linha + 2, 1).Value = item.CodigoProduto;
            resumoWorksheet.Cell(linha + 2, 2).Value = item.DescricaoProduto;
            resumoWorksheet.Cell(linha + 2, 3).Value = item.QuantidadeVendida;
            resumoWorksheet.Cell(linha + 2, 4).Value = item.ValorTotalVendido;
            resumoWorksheet.Cell(linha + 2, 5).Value = item.QuantidadeVendas;
        }

        var vendasWorksheet = workbook.Worksheets.Add("Detalhe Vendas");
        vendasWorksheet.Cell(1, 1).Value = "Venda";
        vendasWorksheet.Cell(1, 2).Value = "Data";
        vendasWorksheet.Cell(1, 3).Value = "Operador";
        vendasWorksheet.Cell(1, 4).Value = "Consumidor";
        vendasWorksheet.Cell(1, 5).Value = "Pagamento";
        vendasWorksheet.Cell(1, 6).Value = "Codigo Produto";
        vendasWorksheet.Cell(1, 7).Value = "Descricao Produto";
        vendasWorksheet.Cell(1, 8).Value = "Quantidade";
        vendasWorksheet.Cell(1, 9).Value = "Preco Unitario";
        vendasWorksheet.Cell(1, 10).Value = "Desconto";
        vendasWorksheet.Cell(1, 11).Value = "Total Item";

        var linhaDetalhe = 2;
        foreach (var venda in vendasFechadas)
        {
            foreach (var item in venda.Itens)
            {
                vendasWorksheet.Cell(linhaDetalhe, 1).Value = venda.NumeroVenda;
                vendasWorksheet.Cell(linhaDetalhe, 2).Value = venda.DhInclusao.ToLocalTime();
                vendasWorksheet.Cell(linhaDetalhe, 3).Value = venda.Operador;
                vendasWorksheet.Cell(linhaDetalhe, 4).Value = venda.Consumidor;
                vendasWorksheet.Cell(linhaDetalhe, 5).Value = venda.FormaPagamento;
                vendasWorksheet.Cell(linhaDetalhe, 6).Value = item.CodigoProduto;
                vendasWorksheet.Cell(linhaDetalhe, 7).Value = item.DescricaoProduto;
                vendasWorksheet.Cell(linhaDetalhe, 8).Value = item.Quantidade;
                vendasWorksheet.Cell(linhaDetalhe, 9).Value = item.PrecoUnitario;
                vendasWorksheet.Cell(linhaDetalhe, 10).Value = item.DescontoValor;
                vendasWorksheet.Cell(linhaDetalhe, 11).Value = item.TotalItem;
                linhaDetalhe++;
            }
        }

        foreach (var worksheet in workbook.Worksheets)
        {
            var headerRange = worksheet.Range(1, 1, 1, worksheet.LastColumnUsed()?.ColumnNumber() ?? 1);
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.FromHtml("#DCEFE7");
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Columns().AdjustToContents();
        }

        resumoWorksheet.Column(4).Style.NumberFormat.Format = "R$ #,##0.00";
        vendasWorksheet.Column(2).Style.DateFormat.Format = "dd/MM/yyyy HH:mm";
        vendasWorksheet.Column(9).Style.NumberFormat.Format = "R$ #,##0.00";
        vendasWorksheet.Column(10).Style.NumberFormat.Format = "R$ #,##0.00";
        vendasWorksheet.Column(11).Style.NumberFormat.Format = "R$ #,##0.00";

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
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