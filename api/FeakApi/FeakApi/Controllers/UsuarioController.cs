using FeakApi.models.services;
using Microsoft.AspNetCore.Mvc;

namespace FeakApi.Controllers
{

    [ApiController]
    [Route("api/[controller]")]
    public class UsuarioController : ControllerBase
    {
        private readonly UsuarioService _service;

        public UsuarioController(UsuarioService service)
        {
            _service = service;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var usuario = await _service.ObterUsuarioComPedidos(id);
            if (usuario == null) return NotFound();
            return Ok(usuario);
        }

    }
}
