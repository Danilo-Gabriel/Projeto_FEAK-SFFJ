using DomainService.DTOs;
using DomainService.Entities;

namespace DomainService.Interfaces.Services;

public interface IUsuarioDomainService : IBaseDomainService<Usuario>
{

    Task<ServiceResponse<UsuarioDTO>> AtualizarUsuario(UsuarioDTO dados);
    Task<ServiceResponse<UsuarioDTO>> ObterUsuarioPorId(Guid id);

    Task<ServiceResponse<UsuarioDTO>> ObterUsuarioNomeLogin(string NomeLogin);

    Task<ServiceResponse<List<UsuarioDTO>>> ObterUsuarios();

    Task<ServiceResponse<UsuarioDTO>> InativarUsuarioPorId(Guid id);

    Task<ServiceResponse<UsuarioDTO>> CadastrarUsuario(UsuarioDTO dados);
}