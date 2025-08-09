using FeakApi.models.dtos;
using FeakApi.models.entity;
using FeakApi.models.repository;
using Microsoft.AspNetCore.Identity;


namespace FeakApi.models.services
{
    public class UsuarioService
    {

        private readonly UsuarioRepository _repo;
        private readonly PasswordHasher<object> _passwordHasher = new PasswordHasher<object>();

        public UsuarioService(UsuarioRepository repo)
        {
            _repo = repo;
        }


        public async Task<ServiceResponse<UsuarioDTO>> AtualizarUsuario(UsuarioDTO dados)
        {
            var serviceResponse = new ServiceResponse<UsuarioDTO>();

            try
            {
                var usuario = await _repo.GetByIdAsync((int)dados.Id);

                if (usuario == null)
                {
                    serviceResponse.Mensagem = "Usuário não encontrado.";
                    serviceResponse.Success = false;
                    return serviceResponse;
                }

                usuario.NomeCompleto = dados.NomeCompleto;
                usuario.NomeLogin = dados.NomeLogin;
                //usuario.Ativo = dados.Ativo;

                serviceResponse.Dados = (await _repo.AtualizarUsuario(usuario)).toDTO();
                serviceResponse.Success = true;
                serviceResponse.Mensagem = "Usuário atualizado com sucesso.";
            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = $"Erro ao atualizar usuário: {ex.Message}";
                serviceResponse.Success = false;
            }

            return serviceResponse;
        }


        public async Task<ServiceResponse<Usuario>> ObterUsuarioPorId(int id)
        {
            ServiceResponse<Usuario> serviceResponse = new ServiceResponse<Usuario>();

            try
            {
                serviceResponse.Dados = await _repo.GetByIdAsync(id);

            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Success = false;
            }

            return serviceResponse;
   
        }

        public async Task<ServiceResponse<Usuario>> ObterUsuarioNomeLogin(string NomeLogin)
        {
            ServiceResponse<Usuario> serviceResponse = new ServiceResponse<Usuario>();

            try
            {
                serviceResponse.Dados = await _repo.GetByNomeLoginAsync(NomeLogin);

            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Success = false;
            }

            return serviceResponse;
  
        }

        public async Task<ServiceResponse<List<UsuarioDTO>>> ObterUsuarios()
        {
            ServiceResponse<List<UsuarioDTO>> serviceResponse = new ServiceResponse<List<UsuarioDTO>>();

            try
            {
                var usuarios = await _repo.ObterUsuarios();
                serviceResponse.Dados = usuarios.Where(x => x.Ativo == true).Select(x => x.toDTO()).OrderBy(x => x.NomeCompleto).ToList();

            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Success = false;
            }

            return serviceResponse;
        }
      
     
    

        public async Task<ServiceResponse<UsuarioDTO>> InativarUsuarioPorId(int id)
        {
            ServiceResponse<UsuarioDTO> serviceResponse = new ServiceResponse<UsuarioDTO>();

            try
            {
                serviceResponse.Dados = (await _repo.inativarUsuario(id)).toDTO();

            }
            catch (Exception ex)
            {
                serviceResponse.Mensagem = ex.Message;
                serviceResponse.Success = false;
            }

            return serviceResponse;

        }
        public async Task<ServiceResponse<UsuarioDTO>> CadastrarUsuario(UsuarioDTO dados)
        {
            ServiceResponse<UsuarioDTO> serviceResponse = new ServiceResponse<UsuarioDTO>();

            try
            {
                serviceResponse.Dados = (await _repo.CadastrarUsuario(dados.ToEntity(_passwordHasher.HashPassword(null, dados.Senha)))).toDTO();
             
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
