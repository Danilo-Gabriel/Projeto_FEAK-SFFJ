using System.Data;
using System.Security.Claims;
using DomainService.Interfaces;
using DomainService.Interfaces.Services;
using DomainService.services;
using Feak.SistemaFinanceiro.DomainService.Helpers.Config;
using Feak.SistemaFinanceiro.DomainService.Security;
using Feak.SistemaFinanceiro.Persistencia.Context;
using Feak.SistemaFinanceiro.Persistencia.services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Npgsql;


namespace Application.services;

public static class Configuration
{
    
    public static void AddDapperContexts(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("feak");

        services.AddScoped<IDbConnection>(sp =>
        {
            return new NpgsqlConnection(connectionString);
        });
    }
    
    public static void AddContextsServices(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("feak");

            services.AddDbContext<ApplicationDbcontext>(options =>
            options.UseNpgsql(connectionString));
        
    }

    public static void AddKeyclokServices(this IServiceCollection services, IConfiguration configuration)
    {
        var keycloakConfig = configuration.GetSection("KeycloakConfig").Get<KeycloakConfig>();
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = $"{keycloakConfig.KeycloakBaseUrl}";
                options.Audience = $"{keycloakConfig.KeycloakClientId}"; 
                options.RequireHttpsMetadata = false;
                options.MapInboundClaims = false;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateAudience = false,
                    ValidateIssuer = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    NameClaimType = "preferred_username",
                    RoleClaimType = ClaimTypes.Role
                };
            });
    }

    public static void AddServices(this IServiceCollection services)
    {
        
        // SERVIÇOS 
        services.AddHttpContextAccessor();
        services.AddScoped<ObjectCompareService>();
        services.AddScoped<ISecurityContext, SecurityContext>();
        
        // Domain services
        services.AddTransient(typeof(IBaseDomainService<>),  typeof(BaseDomainService<>));
        services.AddTransient<IUsuarioDomainService, UsuarioDomainService>();
        services.AddTransient<IProdutoDomainService, ProdutoDomainService>();
        services.AddTransient<IVendaDomainService, VendaDomainService>();
        
        // Repository
        services.AddTransient(typeof(IBaseRepository<>), typeof(BaseRepository<>));
        services.AddTransient<IUsuarioRepository, UsuarioRepository>();
        services.AddTransient<IProdutoRepository, ProdutoRepository>();
        services.AddTransient<IVendaRepository, VendaRepository>();
    }
    
    
    
}