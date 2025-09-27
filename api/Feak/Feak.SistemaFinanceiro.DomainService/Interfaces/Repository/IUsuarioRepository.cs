using DomainService.Entities;

namespace DomainService.Interfaces;

public interface IUsuarioRepository : IBaseRepository<Usuario>
{

    Task<Usuario> GetByIdAsync(Guid id);
    Task<List<Usuario>> ObterUsuarios();
    Task<Usuario> GetByNomeLoginAsync(string NomeLogin);
    Task<Usuario> inativarUsuario(Guid id);
    Task<Usuario> AtualizarUsuario(Usuario dados);

    Task<Usuario> CadastrarUsuario(Usuario dados);
}