using FeakApi.models.dtos;
using FeakApi.models.entity;
using Microsoft.EntityFrameworkCore;

namespace FeakApi.models.repository
{
    public class UsuarioRepository
    {

        private readonly AppDbContext _context;
        
        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Usuario> GetByIdAsync(int id)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Usuario>> ObterUsuarios()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario> GetByNomeLoginAsync(string NomeLogin)
        {
            return await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NomeLogin == NomeLogin);
        }

        public async Task<Usuario> CadastrarUsuario(Usuario dados) 
        {

            await _context.Usuarios.AddAsync(dados);
            await _context.SaveChangesAsync();

            return dados;
        }

        public async Task<Usuario> AtualizarUsuario(Usuario dados)
        {
            _context.Usuarios.Update(dados);
            await _context.SaveChangesAsync();

            return dados;
        }


        public async Task<Usuario> inativarUsuario(int id)
        {

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
            {
                throw new Exception("Usuário não encontrado");
            }

            usuario.Ativo = false;

            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();

            return usuario;
        }


    }
}
