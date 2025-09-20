using FeakApi.models.dtos;
using FeakApi.models.services;
using Microsoft.AspNetCore.Mvc;

namespace FeakApi.Controllers
{

    [ApiController]
    [Route("api/produto")]
    public class ProdutoController : ControllerBase
    {
        private readonly ProdutoService _service;

        public ProdutoController(ProdutoService service)
        {
            _service = service;
        }


        [HttpPost]
        public async Task<ActionResult<ServiceResponse<ProdutoDTO>>> CadastrarUsuario([FromBody] ProdutoDTO request)
        {
            return Ok(await _service.CadastrarUsuario(request));
        }

        //[HttpPut]
        //public async Task<ActionResult<ServiceResponse<ProdutoDTO>>> AtualizarUsuario([FromBody] ProdutoDTO request)
        //{
        //    return Ok(await _service.AtualizarUsuario(request));
        //}

        //[HttpGet]
        //public async Task<ActionResult<ServiceResponse<List<ProdutoDTO>>>> ObterUsuarios()
        //{
        //    return Ok(await _service.ObterUsuarios());
        //}

        //[HttpDelete("{id}")]
        //public async Task<ActionResult<ServiceResponse<ProdutoDTO>>> InativarUsuario(int id)
        //{
        //    return Ok(await _service.InativarUsuarioPorId(id));
        //}
    }
}
