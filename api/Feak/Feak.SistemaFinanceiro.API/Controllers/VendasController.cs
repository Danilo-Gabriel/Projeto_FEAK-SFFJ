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

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<VendaDTO>>> RegistrarVenda([FromBody] RegistrarVendaRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        return Ok(await _vendaDomainService.RegistrarVenda(request));
    }
}