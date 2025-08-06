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

        public async Task<Usuario> ObterUsuarioPorId(int id)
        {
            return await _repo.GetByIdAsync(id);
        }

        public async Task<Usuario> ObterUsuarioNomeLogin(string NomeLogin)
        {
            return await _repo.GetByNomeLoginAsync(NomeLogin);
        }

        public async Task<List<UsuarioDTO>> ObterUsuarios()
        {
            
            var usuarios = await _repo.ObterUsuarios();
            return usuarios.Select(x => x.toDTO()).ToList();
     
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
