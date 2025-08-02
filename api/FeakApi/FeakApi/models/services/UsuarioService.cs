using FeakApi.models.entity;
using FeakApi.models.repository;
using Microsoft.EntityFrameworkCore;

namespace FeakApi.models.services
{
    public class UsuarioService
    {

        private readonly UsuarioRepository _repo;

        public UsuarioService(UsuarioRepository repo)
        {
            _repo = repo;
        }

        public Task<Usuario> ObterUsuarioComPedidos(int id)
        {
            return _repo.GetByIdAsync(id);
        }

    }
}
