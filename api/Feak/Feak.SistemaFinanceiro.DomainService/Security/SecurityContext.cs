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

    public bool IsAuthenticated()
    {
        return User?.Identity?.IsAuthenticated == true;
    }

    public string GetUserId()
    {
        return GetClaimValue("sub", ClaimTypes.NameIdentifier);
    }

    public string GetUserName()
    {
        return GetClaimValue("preferred_username", ClaimTypes.Name, "name", "given_name");
    }

    public string GetFullName()
    {
        var fullName = GetClaimValue("name");
        if (!string.IsNullOrWhiteSpace(fullName))
        {
            return fullName;
        }

        var givenName = GetClaimValue("given_name", ClaimTypes.GivenName);
        var familyName = GetClaimValue("family_name", ClaimTypes.Surname);
        var nomeCompleto = string.Join(" ", new[] { givenName, familyName }.Where(x => !string.IsNullOrWhiteSpace(x)));

        return !string.IsNullOrWhiteSpace(nomeCompleto) ? nomeCompleto : GetUserName();
    }

    public string GetEmail()
    {
        return GetClaimValue("email", ClaimTypes.Email);
    }

    public IEnumerable<string> GetRoles()
    {
        var roles = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var claim in User?.FindAll(ClaimTypes.Role) ?? Enumerable.Empty<Claim>())
        {
            if (!string.IsNullOrWhiteSpace(claim.Value))
            {
                roles.Add(claim.Value);
            }
        }

        foreach (var claim in User?.FindAll("role") ?? Enumerable.Empty<Claim>())
        {
            if (!string.IsNullOrWhiteSpace(claim.Value))
            {
                roles.Add(claim.Value);
            }
        }

        var realmAccess = GetClaimValue("realm_access");
        if (!string.IsNullOrWhiteSpace(realmAccess))
        {
            try
            {
                using var document = JsonDocument.Parse(realmAccess);
                if (document.RootElement.TryGetProperty("roles", out var rolesElement)
                    && rolesElement.ValueKind == JsonValueKind.Array)
                {
                    foreach (var role in rolesElement.EnumerateArray())
                    {
                        var roleValue = role.GetString();
                        if (!string.IsNullOrWhiteSpace(roleValue))
                        {
                            roles.Add(roleValue);
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Não foi possível interpretar a claim realm_access.");
            }
        }

        var resourceAccess = GetClaimValue("resource_access");
        if (!string.IsNullOrWhiteSpace(resourceAccess))
        {
            try
            {
                using var document = JsonDocument.Parse(resourceAccess);
                foreach (var recurso in document.RootElement.EnumerateObject())
                {
                    if (recurso.Value.TryGetProperty("roles", out var rolesElement)
                        && rolesElement.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var role in rolesElement.EnumerateArray())
                        {
                            var roleValue = role.GetString();
                            if (!string.IsNullOrWhiteSpace(roleValue))
                            {
                                roles.Add(roleValue);
                            }
                        }
                    }
                }
            }
            catch (JsonException ex)
            {
                _logger.LogWarning(ex, "Não foi possível interpretar a claim resource_access.");
            }
        }

        return roles;
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

    private string GetClaimValue(params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var claimValue = User?.FindFirst(claimType)?.Value;
            if (!string.IsNullOrWhiteSpace(claimValue))
            {
                return claimValue;
            }
        }

        return null;
    }
}