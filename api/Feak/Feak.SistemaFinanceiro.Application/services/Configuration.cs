using DomainService.Interfaces;
using DomainService.Interfaces.Services;
using DomainService.services;
using Feak.SistemaFinanceiro.Persistencia.Context;
using Feak.SistemaFinanceiro.Persistencia.services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Application.services;

public static class Configuration
{
    public static void AddContextsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("feak");

            services.AddDbContext<ApplicationDbcontext>(options =>
            options.UseNpgsql(connectionString));
        
    }

    public static void AddServices(this IServiceCollection services)
    {
        // Domain services
        services.AddTransient(typeof(IBaseDomainService<>),  typeof(BaseDomainService<>));
        services.AddTransient<IUsuarioDomainService, UsuarioDomainService>();
        
        // Repository
        services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddTransient<IUsuarioRepository, UsuarioRepository>();
    }
    
    
    
}