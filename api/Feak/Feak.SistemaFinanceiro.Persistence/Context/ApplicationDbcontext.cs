using DomainService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Feak.SistemaFinanceiro.Persistencia.Context;

public class ApplicationDbcontext : DbContext
{
    public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
}