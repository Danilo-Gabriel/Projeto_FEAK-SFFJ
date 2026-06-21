using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Feak.SistemaFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ConsumidoresFinaisController : ControllerBase
{
    private readonly IConsumidorFinalDomainService _service;

    public ConsumidoresFinaisController(IConsumidorFinalDomainService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<ServiceResponse<List<ConsumidorFinalDTO>>>> Listar() =>
        Ok(await _service.ListarAtivos());

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<ConsumidorFinalDTO>>> Cadastrar([FromBody] ConsumidorFinalRequest request) =>
        Ok(await _service.Cadastrar(request));
}
