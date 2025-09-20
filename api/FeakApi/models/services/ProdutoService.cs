using FeakApi.models.dtos;
using FeakApi.models.entity;
using FeakApi.models.repository;
using Microsoft.AspNetCore.Identity;


namespace FeakApi.models.services
{
    public class ProdutoService
    {

        private readonly ProdutoRepository _repo;

        public ProdutoService(ProdutoRepository repo)
        {
            _repo = repo;
        }


        //public async Task<ServiceResponse<ProdutoDTO>> AtualizarUsuario(ProdutoDTO dados)
        //{
        //    var serviceResponse = new ServiceResponse<ProdutoDTO>();

        //    try
        //    {
               

        //        //serviceResponse.Dados = (await _repo.AtualizarUsuario(usuario)).toDTO();
        //        serviceResponse.Success = true;
        //        serviceResponse.Mensagem = "Usuário atualizado com sucesso.";
        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Mensagem = $"Erro ao atualizar usuário: {ex.Message}";
        //        serviceResponse.Success = false;
        //    }

        //    return serviceResponse;
        //}


        //public async Task<ServiceResponse<Produto>> ObterUsuarioPorId(int id)
        //{
        //    ServiceResponse<Produto> serviceResponse = new ServiceResponse<Produto>();

        //    try
        //    {
        //        serviceResponse.Dados = await _repo.GetByIdAsync(id);

        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Mensagem = ex.Message;
        //        serviceResponse.Success = false;
        //    }

        //    return serviceResponse;
   
        //}

        //public async Task<ServiceResponse<Produto>> ObterUsuarioNomeLogin(string NomeLogin)
        //{
        //    ServiceResponse<Produto> serviceResponse = new ServiceResponse<Produto>();

        //    try
        //    {
        //        serviceResponse.Dados = await _repo.GetByNomeLoginAsync(NomeLogin);

        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Mensagem = ex.Message;
        //        serviceResponse.Success = false;
        //    }

        //    return serviceResponse;
  
        //}

        //public async Task<ServiceResponse<List<ProdutoDTO>>> ObterUsuarios()
        //{
        //    ServiceResponse<List<ProdutoDTO>> serviceResponse = new ServiceResponse<List<ProdutoDTO>>();

        //    try
        //    {
        //        var usuarios = await _repo.ObterUsuarios();
        //        serviceResponse.Dados = usuarios.Where(x => x.Ativo == true).Select(x => x.toDTO()).OrderBy(x => x.NomeCompleto).ToList();

        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Mensagem = ex.Message;
        //        serviceResponse.Success = false;
        //    }

        //    return serviceResponse;
        //}
      
     
    

        //public async Task<ServiceResponse<ProdutoDTO>> InativarUsuarioPorId(int id)
        //{
        //    ServiceResponse<ProdutoDTO> serviceResponse = new ServiceResponse<ProdutoDTO>();

        //    try
        //    {
        //        serviceResponse.Dados = (await _repo.InativaProduto(id)).toDTO();

        //    }
        //    catch (Exception ex)
        //    {
        //        serviceResponse.Mensagem = ex.Message;
        //        serviceResponse.Success = false;
        //    }

        //    return serviceResponse;

        //}
        public async Task<ServiceResponse<ProdutoDTO>> CadastrarUsuario(ProdutoDTO dados)
        {
            ServiceResponse<ProdutoDTO> serviceResponse = new ServiceResponse<ProdutoDTO>();

            try
            {
                serviceResponse.Dados = (await _repo.CadastrarProduto(dados.ToEntity())).toDTO();

            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Success = false;
            }

            return serviceResponse;
        }

    }
}
