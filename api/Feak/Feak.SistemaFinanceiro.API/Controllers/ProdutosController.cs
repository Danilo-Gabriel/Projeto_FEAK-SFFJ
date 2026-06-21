using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Interfaces.Services;
using Feak.SistemaFinanceiro.API.Models.Requests;
using Feak.SistemaFinanceiro.DomainService.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace Feak.SistemaFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProdutosController : ControllerBase
{
    private readonly IProdutoDomainService _produtoDomainService;

    public ProdutosController(IProdutoDomainService produtoDomainService)
    {
        _produtoDomainService = produtoDomainService;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<ProdutoDTO>>> CadastrarProduto([FromBody] ProdutoRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(await _produtoDomainService.CadastrarProduto(request));
    }

    [HttpPut]
    public async Task<ActionResult<ServiceResponse<ProdutoDTO>>> AtualizarProduto([FromBody] ProdutoDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(await _produtoDomainService.AtualizarProduto(request));
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<ProdutoDTO>>>> ObterProdutos()
    {
        return Ok(await _produtoDomainService.ObterProdutos());
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ServiceResponse<ProdutoDTO>>> ObterProdutoPorId(Guid id)
    {
        return Ok(await _produtoDomainService.ObterProdutoPorId(id));
    }

    [HttpGet("codigo-barras/{codigoBarras}")]
    public async Task<ActionResult<ServiceResponse<ProdutoDTO>>> ObterProdutoPorCodigoBarras(string codigoBarras)
    {
        return Ok(await _produtoDomainService.ObterProdutoPorCodigoBarras(codigoBarras));
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<ProdutoDTO>>> InativarProduto(Guid id)
    {
        return Ok(await _produtoDomainService.InativarProdutoPorId(id));
    }

    [HttpPost("importar")]
    [Consumes("multipart/form-data")]
    public async Task<ActionResult<ServiceResponse<List<ProdutoDTO>>>> ImportarProdutos([FromForm] ImportarProdutosRequest request)
    {
        if (request.Arquivo == null || request.Arquivo.Length == 0)
        {
            return BadRequest(new ServiceResponse<List<ProdutoDTO>>
            {
                Success = false,
                Mensagem = "Selecione um arquivo de importação."
            });
        }

        await using var stream = request.Arquivo.OpenReadStream();
        return Ok(await _produtoDomainService.ImportarProdutos(stream));
    }

    [HttpGet("template-importacao")]
    public IActionResult BaixarTemplateImportacao()
    {
        var template = _produtoDomainService.ObterTemplateImportacao();
        return ExcelFileHelper.CriarArquivo(template, "template_importacao_produtos", "Produtos");
    }
}
