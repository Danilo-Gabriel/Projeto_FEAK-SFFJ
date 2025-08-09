using FeakApi.models.dtos;
using FeakApi.models.repository;
using FeakApi.models.services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace FeakApi.Controllers
{
    [Route("api/login")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        private readonly UsuarioService _usuarioService;

        private readonly PasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

        public LoginController(UsuarioService usuarioService) {

            _usuarioService = usuarioService;
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

            var usuario = await _usuarioService.ObterUsuarioNomeLogin(request.NomeLogin);
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

            response.Dados = usuario.Dados.toDTO();

            return Ok(response);
        }
    }
}
