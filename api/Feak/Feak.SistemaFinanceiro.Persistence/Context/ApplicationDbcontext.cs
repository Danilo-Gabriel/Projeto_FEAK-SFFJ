using DomainService.Entities;
using Microsoft.EntityFrameworkCore;

namespace Feak.SistemaFinanceiro.Persistencia.Context;

public class ApplicationDbcontext : DbContext
{
    public ApplicationDbcontext(DbContextOptions<ApplicationDbcontext> options) : base(options)
    {
    }

    public DbSet<Usuario> Usuarios { get; set; }
    public DbSet<Produto> Produtos { get; set; }
    public DbSet<Venda> Vendas { get; set; }
    public DbSet<VendaItem> VendaItens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Produto>()
            .HasIndex(x => x.CodigoBarras)
            .IsUnique();

        modelBuilder.Entity<Produto>()
            .Property(x => x.PrecoCusto)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Produto>()
            .Property(x => x.PrecoVenda)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Venda>()
            .Property(x => x.Subtotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Venda>()
            .Property(x => x.DescontoTotal)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Venda>()
            .Property(x => x.Acrescimo)
            .HasPrecision(18, 2);

        modelBuilder.Entity<Venda>()
            .Property(x => x.Total)
            .HasPrecision(18, 2);

        modelBuilder.Entity<VendaItem>()
            .Property(x => x.PrecoUnitario)
            .HasPrecision(18, 2);

        modelBuilder.Entity<VendaItem>()
            .Property(x => x.DescontoValor)
            .HasPrecision(18, 2);

        modelBuilder.Entity<VendaItem>()
            .Property(x => x.DescontoPercentual)
            .HasPrecision(18, 2);

        modelBuilder.Entity<VendaItem>()
            .Property(x => x.TotalItem)
            .HasPrecision(18, 2);

        modelBuilder.Entity<VendaItem>()
            .HasOne(x => x.Venda)
            .WithMany(x => x.Itens)
            .HasForeignKey(x => x.VendaId);

        modelBuilder.Entity<VendaItem>()
            .HasOne(x => x.Produto)
            .WithMany()
            .HasForeignKey(x => x.ProdutoId);
    }
}