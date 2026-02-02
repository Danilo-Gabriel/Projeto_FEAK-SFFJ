using System.Security.Claims;
using System.Text.Json;
using Feak.SistemaFinanceiro.DomainService.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Feak.SistemaFinanceiro.DomainService.Security;

public class SecurityContext : ISecurityContext
{

    #region Construtor

    private readonly IHttpContextAccessor _httpContextAccessor;
    private ClaimsPrincipal User => _httpContextAccessor.HttpContext?.User;
    private readonly ILogger<SecurityContext> _logger;
    public SecurityContext(IHttpContextAccessor httpContextAccessor, ILogger<SecurityContext> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;

        if (User?.Identity?.IsAuthenticated == true)
        {
            _logger.LogInformation("Usuário autenticado: {Name}", User.Identity.Name);
            foreach (var claim in User.Claims)
            {
                _logger.LogInformation("Claim: {Type} = {Value}", claim.Type, claim.Value);
            }
        }
        else
        {
            _logger.LogWarning("Nenhum usuário autenticado encontrado no contexto HTTP.");
        }
    }

    #endregion

    public string GetUserId()
    {
        var id = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(id))
            return null;

        return id.Split(':').Skip(2).FirstOrDefault();
    }

    public string GetUserName()
    {
        return User?.FindFirst("preferred_username")?.Value ?? User?.Identity?.Name;
    }

    public string GetEmail()
    {
        return User?.FindFirst(ClaimTypes.Email)?.Value;
    }

  // public IEnumerable<DomainDTO> GetPerfis()
  // {
  //     var claims = User?.FindAll("perfis") ?? Enumerable.Empty<Claim>();

  //     foreach (var claim in claims)
  //     {
  //         if (!string.IsNullOrEmpty(claim.Value))
  //         {
  //             DomainDTO empresa = JsonSerializer.Deserialize<DomainDTO>(claim.Value);
  //             if (empresa != null)
  //                 yield return empresa;
  //         }
  //     }
  // }

    public DomainDTO GetUnidade()
    {
        var claim = User?.FindFirst("unidade")?.Value;

        if (!string.IsNullOrEmpty(claim))
        {
            return JsonSerializer.Deserialize<DomainDTO>(claim);
        }

        return null;
    }
}