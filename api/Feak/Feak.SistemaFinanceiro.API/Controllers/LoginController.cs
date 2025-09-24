using DomainService.DTOs;
using DomainService.Interfaces.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Feak.SistemaFinanceiro.API.Controllers;

[Route("api/login")]
[ApiController]
public class LoginController : ControllerBase
{
    //todo class inteira será refatorada para implementação do keycloak

    private readonly IUsuarioDomainService _usuarioDomainService;

    private readonly PasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

    public LoginController(IUsuarioDomainService usuarioService) {

        _usuarioDomainService = usuarioService;
    }

    [HttpPost]
    public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> AuthUser(LoginDTO request)
    {
        if (request == null) {
            return BadRequest(new ServiceResponse<UsuarioDTO>()
            {
                Dados = null,
                Mensagem = "Dados invalidos",
                Success = false
            });
        }

        ServiceResponse<UsuarioDTO> response = new ServiceResponse<UsuarioDTO>();

        var usuario = await _usuarioDomainService.ObterUsuarioNomeLogin(request.NomeLogin);
        if (usuario == null)
        {
            response.Mensagem = "Usuario não encontrado!";
            response.Success = false;
            return BadRequest(response);
        }
        var result = (_passwordHasher.VerifyHashedPassword(null, usuario.Dados.Senha, request.Senha)) == PasswordVerificationResult.Success;

        if (!result)
        {
            response.Mensagem = "Nome ou senha incorretas!";
            response.Success = false;
            return BadRequest(response);
        }

        response.Dados = usuario.Dados;

        return Ok(response);
    }
}