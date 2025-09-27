using DomainService.Entities;
using DomainService.Interfaces;
using Feak.SistemaFinanceiro.Persistencia.Context;
using Microsoft.EntityFrameworkCore;

namespace Feak.SistemaFinanceiro.Persistencia.services;

public class UsuarioRepository : BaseRepository<Usuario>, IUsuarioRepository
{
    public UsuarioRepository(ApplicationDbcontext context) : base(context)
    {
        
    }
    
    public async Task<Usuario?> GetByIdAsync(Guid id)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.Id == id);
    }
    
    public async  Task<List<Usuario>> ObterUsuarios()
    {
        return await _context.Usuarios.ToListAsync();
    }

    public async Task<Usuario?> GetByNomeLoginAsync(string NomeLogin)
    {
        return await _context.Usuarios
            .FirstOrDefaultAsync(u => u.NomeLogin == NomeLogin);
    }

    public async Task<Usuario> inativarUsuario(Guid id)
    {
        var usuario = await _context.Usuarios.FindAsync(id);
        if (usuario == null)
        {
            throw new Exception("Usuário não encontrado");
        }

        usuario.DhExclusao = DateTime.Now;

        _context.Usuarios.Update(usuario);
        await _context.SaveChangesAsync();

        return usuario;
    }

    public async Task<Usuario> AtualizarUsuario(Usuario dados)
    {
        _context.Usuarios.Update(dados);
        await _context.SaveChangesAsync();

        return dados;
    }

    public async Task<Usuario> CadastrarUsuario(Usuario dados)
    {
        await _context.Usuarios.AddAsync(dados);
        await _context.SaveChangesAsync();

        return dados;
    }
}