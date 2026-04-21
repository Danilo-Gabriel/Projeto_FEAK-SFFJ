using Application.services;
using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Interfaces.Services;
using Feak.SistemaFinanceiro.DomainService.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Feak.SistemaFinanceiro.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsuariosController : ControllerBase
{
    private readonly IUsuarioDomainService _usuarioDomainService;
    
    private readonly ObjectCompareService _compareService;
    
    private readonly ISecurityContext  _securityContext;

    
    
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
    public UsuariosController(IUsuarioDomainService usuarioDomainService,  ObjectCompareService compareService, ISecurityContext securityContext)
    {
        _usuarioDomainService = usuarioDomainService;
        _compareService = compareService;
        _securityContext = securityContext;
    }
     
    
    [HttpPost]
    public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> CadastrarUsuario([FromBody] UsuarioRequest request)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);
        
        var result = await _usuarioDomainService.CadastrarUsuario(request);
        
        return Ok(result);
    }
    
    [HttpPut]
    public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> AtualizarUsuario([FromBody] UsuarioAtualizacaoRequest request)
    {
        if(!ModelState.IsValid)
            return BadRequest(ModelState);

        return Ok(await _usuarioDomainService.AtualizarUsuario(request));
    }

    [HttpGet]
    //[Authorize]
    public async Task<ActionResult<ServiceResponse<List<UsuarioDTO>>>> ObterUsuarios()
    {
        var email = _securityContext.GetEmail();
        var nome = _securityContext.GetUserName(); 
        return Ok(await _usuarioDomainService.ObterUsuarios());
        
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> InativarUsuario(Guid id)
    {
        return Ok(await _usuarioDomainService.InativarUsuarioPorId(id));
    }
}