using DomainService.Entities;

namespace DomainService.Interfaces;

public interface IVendaRepository : IBaseRepository<Venda>
{
    Task<string> GerarNumeroVendaAsync();
    Task<Venda> RegistrarVendaAsync(Venda venda, List<VendaItem> itens);
}