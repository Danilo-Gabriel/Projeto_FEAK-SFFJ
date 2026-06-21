using DomainService.Entities;

namespace DomainService.Interfaces;

public interface IConsumidorFinalRepository : IBaseRepository<ConsumidorFinal>
{
    Task<List<ConsumidorFinal>> ListarAtivosAsync();
    Task<ConsumidorFinal?> ObterPorNomeAsync(string nome);
    Task<ConsumidorFinal> CadastrarAsync(ConsumidorFinal consumidor);
}
