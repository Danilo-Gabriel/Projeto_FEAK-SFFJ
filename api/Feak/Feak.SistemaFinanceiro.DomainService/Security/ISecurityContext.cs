

using Feak.SistemaFinanceiro.DomainService.DTOs;

namespace Feak.SistemaFinanceiro.DomainService.Security;

public interface ISecurityContext
{
    bool IsAuthenticated();
    string GetUserId();
    string GetUserName();
    string GetFullName();
    string GetEmail();
    IEnumerable<string> GetRoles();
    DomainDTO GetUnidade();
   // IEnumerable<DomainDTO> GetPerfis();

}