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

        public Task<Usuario> ObterUsuarioPorId(int id)
        {
            return _repo.GetByIdAsync(id);
        }

        public Task<Usuario> ObterUsuarioNomeLogin(string NomeLogin)
        {
            return _repo.GetByNomeLoginAsync(NomeLogin);
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
