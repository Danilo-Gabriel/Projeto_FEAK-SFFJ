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
    }
}
