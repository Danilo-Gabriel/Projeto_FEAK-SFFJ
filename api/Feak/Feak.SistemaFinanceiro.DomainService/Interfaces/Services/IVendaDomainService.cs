using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;

namespace DomainService.Interfaces.Services;

public interface IVendaDomainService : IBaseDomainService<Venda>
{
    Task<byte[]> ExportarRelatorioExcel();
    Task<ServiceResponse<List<VendaDTO>>> ListarVendas();
    Task<ServiceResponse<VendaDTO>> RegistrarVenda(RegistrarVendaRequest request);
    Task<ServiceResponse<VendaDTO>> CancelarVenda(Guid vendaId);
}