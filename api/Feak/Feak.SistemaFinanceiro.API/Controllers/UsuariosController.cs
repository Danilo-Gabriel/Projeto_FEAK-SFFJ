using Application.services;
using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Feak.SistemaFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioDomainService _usuarioDomainService;
    
    private readonly ObjectCompareService _compareService;

    
    
    // [HttpPost("comparar")]
    // public IActionResult Comparar([FromBody] UsuarioDTO novo)
    // {
    //     var antigo = new UsuarioDTO
    //     {
    //         NomeLogin = "Danilo",
    //         NomeCompleto = "danilo@email.com",
    //         Idade = 30
    //     };
    //
    //     var result = _compareService.Compare(antigo, novo);
    //
    //     if (result.AreEqual)
    //         return Ok("Nenhuma alteração encontrada.");
    //
    //     var diferencas = result.Differences.Select(d => new
    //     {
    //         Campo = DisplayNameResolver.GetDisplayName<UsuarioDTO>(d.PropertyName),
    //         ValorAntigo = d.Object1Value,
    //         ValorNovo = d.Object2Value
    //     });
    //
    //     return Ok(diferencas);
    // }
    public UsuariosController(IUsuarioDomainService usuarioDomainService,  ObjectCompareService compareService)
    {
        _usuarioDomainService = usuarioDomainService;
        _compareService = compareService;
    }
    
    
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> CadastrarUsuario([FromBody] UsuarioRequest request)
    {
        return Ok(await _usuarioDomainService.CadastrarUsuario(request));
    }
    
    [HttpPut]
    public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> AtualizarUsuario([FromBody] UsuarioDTO request)
    {
        return Ok(await _usuarioDomainService.AtualizarUsuario(request));
    }

    [HttpGet]
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