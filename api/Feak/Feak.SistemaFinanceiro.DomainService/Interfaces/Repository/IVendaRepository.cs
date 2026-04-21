using DomainService.Entities;

namespace DomainService.Interfaces;

public interface IVendaRepository : IBaseRepository<Venda>
{
    Task<string> GerarNumeroVendaAsync();
    Task<List<Venda>> ListarVendasAsync();
    Task<Venda> RegistrarVendaAsync(Venda venda, List<VendaItem> itens);
    Task<Venda?> ObterVendaComItensAsync(Guid vendaId);
    Task<Venda> CancelarVendaAsync(Venda venda);
}