using ClosedXML.Excel;
using System.Globalization;
using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;
using DomainService.Interfaces;
using DomainService.Interfaces.Services;

namespace DomainService.services;

public class ProdutoDomainService : BaseDomainService<Produto>, IProdutoDomainService
{
    private readonly IProdutoRepository _produtoRepository;

    public ProdutoDomainService(IProdutoRepository produtoRepository) : base(produtoRepository)
    {
        _produtoRepository = produtoRepository;
    }

    public async Task<ServiceResponse<ProdutoDTO>> CadastrarProduto(ProdutoRequest dados)
    {
        var serviceResponse = new ServiceResponse<ProdutoDTO>();

        try
        {
            var codigoBarras = dados.CodigoBarras.Trim();
            var descricao = dados.Descricao.Trim();
            var existente = await _produtoRepository.GetByDescricaoAsync(descricao);
            if (existente != null && existente.DhExclusao == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "Já existe um produto com a descrição informada.";
                return serviceResponse;
            }

            var produtoCodigoExistente = await _produtoRepository.GetByCodigoBarrasAsync(codigoBarras);
            if (produtoCodigoExistente != null && produtoCodigoExistente.DhExclusao == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "Já existe um produto com o código de barras informado.";
                return serviceResponse;
            }

            var produto = dados.ToEntity();
            produto.CodigoBarras = codigoBarras;
            produto.Descricao = descricao;
            serviceResponse.Dados = (await _produtoRepository.CadastrarProduto(produto)).ToDTO();
            serviceResponse.Mensagem = "Produto cadastrado com sucesso.";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = $"Erro ao cadastrar produto: {ex.Message}";
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<ProdutoDTO>> AtualizarProduto(ProdutoDTO dados)
    {
        var serviceResponse = new ServiceResponse<ProdutoDTO>();

        try
        {
            var produto = await _produtoRepository.GetByIdAsync(dados.Id);
            if (produto == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "Produto não encontrado.";
                return serviceResponse;
            }

            var codigoBarras = dados.CodigoBarras.Trim();
            var descricao = dados.Descricao.Trim();
            var duplicado = await _produtoRepository.GetByDescricaoAsync(descricao);
            if (duplicado != null && duplicado.Id != dados.Id && duplicado.DhExclusao == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "Já existe outro produto com a descrição informada.";
                return serviceResponse;
            }

            var codigoDuplicado = await _produtoRepository.GetByCodigoBarrasAsync(codigoBarras);
            if (codigoDuplicado != null && codigoDuplicado.Id != dados.Id && codigoDuplicado.DhExclusao == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "Já existe outro produto com o código de barras informado.";
                return serviceResponse;
            }

            produto.CodigoBarras = codigoBarras;
            produto.Descricao = descricao;
            produto.PrecoCusto = dados.PrecoCusto;
            produto.PrecoVenda = dados.PrecoVenda;
            produto.EstoqueAtual = dados.EstoqueAtual;
            produto.DhExclusao = null;

            serviceResponse.Dados = (await _produtoRepository.AtualizarProduto(produto)).ToDTO();
            serviceResponse.Mensagem = "Produto atualizado com sucesso.";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = $"Erro ao atualizar produto: {ex.Message}";
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<ProdutoDTO>> ObterProdutoPorCodigoBarras(string codigoBarras)
    {
        var serviceResponse = new ServiceResponse<ProdutoDTO>();

        try
        {
            var produto = await _produtoRepository.GetByCodigoBarrasAsync(codigoBarras.Trim());
            if (produto == null || produto.DhExclusao != null)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "Produto não encontrado para o código informado.";
                return serviceResponse;
            }

            serviceResponse.Dados = produto.ToDTO();
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<List<ProdutoDTO>>> ObterProdutos()
    {
        var serviceResponse = new ServiceResponse<List<ProdutoDTO>>();

        try
        {
            var produtos = await _produtoRepository.ObterProdutos();
            serviceResponse.Dados = produtos
                .Where(x => x.DhExclusao == null)
                .OrderBy(x => x.Descricao)
                .Select(x => x.ToDTO())
                .ToList();
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<ProdutoDTO>> ObterProdutoPorId(Guid id)
    {
        var serviceResponse = new ServiceResponse<ProdutoDTO>();

        try
        {
            var produto = await _produtoRepository.GetByIdAsync(id);
            if (produto == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "Produto não encontrado.";
                return serviceResponse;
            }

            serviceResponse.Dados = produto.ToDTO();
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<ProdutoDTO>> InativarProdutoPorId(Guid id)
    {
        var serviceResponse = new ServiceResponse<ProdutoDTO>();

        try
        {
            serviceResponse.Dados = (await _produtoRepository.InativarProduto(id)).ToDTO();
            serviceResponse.Mensagem = "Produto removido com sucesso.";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = ex.Message;
        }

        return serviceResponse;
    }

    public async Task<ServiceResponse<List<ProdutoDTO>>> ImportarProdutos(Stream arquivoStream)
    {
        var serviceResponse = new ServiceResponse<List<ProdutoDTO>> { Dados = new List<ProdutoDTO>() };

        try
        {
            using var workbook = new XLWorkbook(arquivoStream);
            var worksheet = workbook.Worksheets.FirstOrDefault();
            if (worksheet == null)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "A planilha está vazia.";
                return serviceResponse;
            }

            var ultimaLinha = worksheet.LastRowUsed()?.RowNumber() ?? 0;
            if (ultimaLinha < 2)
            {
                serviceResponse.Success = false;
                serviceResponse.Mensagem = "A planilha não possui linhas para importação.";
                return serviceResponse;
            }

            for (var linha = 2; linha <= ultimaLinha; linha++)
            {
                var codigoBarras = worksheet.Cell(linha, 1).GetString().Trim();
                var descricao = worksheet.Cell(linha, 2).GetString().Trim();
                if (string.IsNullOrWhiteSpace(descricao))
                {
                    continue;
                }

                var precoCusto = ParseDecimal(worksheet.Cell(linha, 3).Value.ToString());
                var precoVenda = ParseDecimal(worksheet.Cell(linha, 4).Value.ToString());
                var estoqueAtual = ParseInteger(worksheet.Cell(linha, 5).Value.ToString());

                var produtoExistente = !string.IsNullOrWhiteSpace(codigoBarras)
                    ? await _produtoRepository.GetByCodigoBarrasAsync(codigoBarras)
                    : await _produtoRepository.GetByDescricaoAsync(descricao);

                if (produtoExistente == null)
                {
                    var novoProduto = new Produto
                    {
                        Id = Guid.NewGuid(),
                        CodigoBarras = string.IsNullOrWhiteSpace(codigoBarras) ? $"PDV-{Guid.NewGuid():N}"[..14] : codigoBarras,
                        Descricao = descricao,
                        PrecoCusto = precoCusto,
                        PrecoVenda = precoVenda,
                        EstoqueAtual = estoqueAtual
                    };

                    serviceResponse.Dados.Add((await _produtoRepository.CadastrarProduto(novoProduto)).ToDTO());
                    continue;
                }

                var quantidadeImportada = estoqueAtual < 0 ? 0 : estoqueAtual;

                produtoExistente.CodigoBarras = string.IsNullOrWhiteSpace(codigoBarras) ? produtoExistente.CodigoBarras : codigoBarras;
                produtoExistente.Descricao = descricao;
                produtoExistente.PrecoCusto = precoCusto;
                produtoExistente.PrecoVenda = precoVenda;
                produtoExistente.EstoqueAtual += quantidadeImportada;
                produtoExistente.DhExclusao = null;

                serviceResponse.Dados.Add((await _produtoRepository.AtualizarProduto(produtoExistente)).ToDTO());
            }

            serviceResponse.Mensagem = $"Importação concluída com {serviceResponse.Dados.Count} produto(s).";
        }
        catch (Exception ex)
        {
            serviceResponse.Success = false;
            serviceResponse.Mensagem = $"Erro ao importar produtos: {ex.Message}";
        }

        return serviceResponse;
    }

    public List<ProdutoImportacaoDTO> ObterTemplateImportacao()
    {
        return new List<ProdutoImportacaoDTO>
        {
            new() { CodigoBarras = "7891000100101", Descricao = "Arroz Tipo 1 5kg", PrecoCusto = 22.5m, PrecoVenda = 31.9m, EstoqueAtual = 18 },
            new() { CodigoBarras = "7891000100102", Descricao = "Feijão Carioca 1kg", PrecoCusto = 5.8m, PrecoVenda = 8.9m, EstoqueAtual = 32 },
            new() { CodigoBarras = "7891000100103", Descricao = "Óleo de Soja 900ml", PrecoCusto = 4.9m, PrecoVenda = 7.5m, EstoqueAtual = 24 },
            new() { CodigoBarras = "7891000100104", Descricao = "Café Torrado 500g", PrecoCusto = 11.5m, PrecoVenda = 17.9m, EstoqueAtual = 14 }
        };
    }

    private static decimal ParseDecimal(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return 0;
        }

        var valorNormalizado = valor.Trim();

        if (decimal.TryParse(valorNormalizado, NumberStyles.Any, new CultureInfo("pt-BR"), out var valorPtBr))
        {
            return valorPtBr;
        }

        if (decimal.TryParse(valorNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out var valorInvariante))
        {
            return valorInvariante;
        }

        return 0;
    }

    private static int ParseInteger(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return 0;
        }

        var valorNormalizado = valor.Trim();

        if (int.TryParse(valorNormalizado, out var numeroInteiro))
        {
            return numeroInteiro;
        }

        if (decimal.TryParse(valorNormalizado, NumberStyles.Any, new CultureInfo("pt-BR"), out var numeroDecimalPtBr))
        {
            return (int)Math.Truncate(numeroDecimalPtBr);
        }

        if (decimal.TryParse(valorNormalizado, NumberStyles.Any, CultureInfo.InvariantCulture, out var numeroDecimalInvariante))
        {
            return (int)Math.Truncate(numeroDecimalInvariante);
        }

        return 0;
    }
}