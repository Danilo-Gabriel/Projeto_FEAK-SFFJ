using DomainService.DTOs;
using DomainService.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feak.SistemaFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuarioController : ControllerBase
{
    private readonly IUsuarioDomainService _usuarioDomainService;

    public UsuarioController(IUsuarioDomainService usuarioDomainService)
    {
        _usuarioDomainService = usuarioDomainService;
    }
    
    
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> CadastrarUsuario([FromBody] UsuarioDTO request)
    {
        return Ok(await _usuarioDomainService.CadastrarUsuario(request));
    }
    
    [HttpPut]
    public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> AtualizarUsuario([FromBody] UsuarioDTO request)
    {
        return Ok(await _usuarioDomainService.AtualizarUsuario(request));
    }

    [HttpGet]
    [Authorize]
    public async Task<ActionResult<ServiceResponse<List<UsuarioDTO>>>> ObterUsuarios()
    {
        return Ok(await _usuarioDomainService.ObterUsuarios());
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> InativarUsuario(Guid id)
    {
        return Ok(await _usuarioDomainService.InativarUsuarioPorId(id));
    }
}