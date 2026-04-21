using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Feak.SistemaFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class VendasController : ControllerBase
{
    private readonly IVendaDomainService _vendaDomainService;

    public VendasController(IVendaDomainService vendaDomainService)
    {
        _vendaDomainService = vendaDomainService;
    }

    [HttpGet("exportar-excel")]
    public async Task<IActionResult> ExportarExcel()
    {
        var arquivo = await _vendaDomainService.ExportarRelatorioExcel();
        var nomeArquivo = $"relatorio-vendas-{DateTime.Now:yyyyMMdd-HHmmss}.xlsx";
        return File(
            arquivo,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            nomeArquivo);
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<VendaDTO>>>> ListarVendas()
    {
        return Ok(await _vendaDomainService.ListarVendas());
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<VendaDTO>>> RegistrarVenda([FromBody] RegistrarVendaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(await _vendaDomainService.RegistrarVenda(request));
    }

    [HttpPut("{id:guid}/cancelar")]
    public async Task<ActionResult<ServiceResponse<VendaDTO>>> CancelarVenda(Guid id)
    {
        return Ok(await _vendaDomainService.CancelarVenda(id));
    }
}