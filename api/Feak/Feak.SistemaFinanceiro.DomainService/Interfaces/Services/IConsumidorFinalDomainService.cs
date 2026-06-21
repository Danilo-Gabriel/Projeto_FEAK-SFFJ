using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;

namespace DomainService.Interfaces.Services;

public interface IConsumidorFinalDomainService : IBaseDomainService<ConsumidorFinal>
{
    Task<ServiceResponse<List<ConsumidorFinalDTO>>> ListarAtivos();
    Task<ServiceResponse<ConsumidorFinalDTO>> Cadastrar(ConsumidorFinalRequest request);
}
