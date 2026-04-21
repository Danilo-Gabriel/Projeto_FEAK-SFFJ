using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;

namespace DomainService.Interfaces.Services;

public interface IVendaDomainService : IBaseDomainService<Venda>
{
    Task<ServiceResponse<VendaDTO>> RegistrarVenda(RegistrarVendaRequest request);
}