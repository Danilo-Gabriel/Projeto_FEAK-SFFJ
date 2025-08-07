using FeakApi.models.dtos;
using FeakApi.models.services;
using Microsoft.AspNetCore.Mvc;

namespace FeakApi.Controllers
{

    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<ActionResult<ServiceResponse<UsuarioDTO>>> CadastrarUsuario([FromBody] UsuarioDTO request)
        {
            return Ok(await _service.CadastrarUsuario(request));
        }

        [HttpGet]
        public async Task<ActionResult<ServiceResponse<List<UsuarioDTO>>>> ObterUsuarios()
        {
            return Ok(await _service.ObterUsuarios());
        }
    }
}
