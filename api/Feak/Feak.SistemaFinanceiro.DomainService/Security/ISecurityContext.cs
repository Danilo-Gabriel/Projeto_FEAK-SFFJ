

using Feak.SistemaFinanceiro.DomainService.DTOs;

namespace Feak.SistemaFinanceiro.DomainService.Security;

public interface ISecurityContext
{
    string GetUserId();
    string GetUserName();
    string GetEmail();
    DomainDTO GetUnidade();
   // IEnumerable<DomainDTO> GetPerfis();

}