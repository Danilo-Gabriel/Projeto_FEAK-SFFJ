using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;

namespace DomainService.Interfaces.Services;

public interface IUsuarioDomainService : IBaseDomainService<Usuario>
{

    Task<ServiceResponse<UsuarioDTO>> AtualizarUsuario(UsuarioAtualizacaoRequest dados);
    Task<ServiceResponse<UsuarioDTO>> ObterUsuarioPorId(Guid id);

    Task<ServiceResponse<UsuarioDTO>> ObterUsuarioNomeLogin(string NomeLogin);

    Task<ServiceResponse<List<UsuarioDTO>>> ObterUsuarios();

    Task<ServiceResponse<UsuarioDTO>> InativarUsuarioPorId(Guid id);

    Task<ServiceResponse<UsuarioDTO>> CadastrarUsuario(UsuarioRequest dados);
}