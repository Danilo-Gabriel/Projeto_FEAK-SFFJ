using FeakApi.models.entity;
using Microsoft.EntityFrameworkCore;

namespace FeakApi.models.repository
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options){}

        public DbSet<Usuario> Usuarios { get; set; }

        public DbSet<Produto> Produtos{ get; set; }
    }
}
