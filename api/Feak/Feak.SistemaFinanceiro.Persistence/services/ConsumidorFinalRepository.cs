using DomainService.Entities;
using DomainService.Interfaces;
using Feak.SistemaFinanceiro.Persistencia.Context;
using Microsoft.EntityFrameworkCore;

namespace Feak.SistemaFinanceiro.Persistencia.services;

public class ConsumidorFinalRepository : BaseRepository<ConsumidorFinal>, IConsumidorFinalRepository
{
    public ConsumidorFinalRepository(ApplicationDbcontext context) : base(context) { }

    public Task<List<ConsumidorFinal>> ListarAtivosAsync() => _context.ConsumidoresFinais
        .AsNoTracking()
        .Where(x => x.DhExclusao == null)
        .OrderBy(x => x.Nome)
        .ToListAsync();

    public Task<ConsumidorFinal?> ObterPorNomeAsync(string nome) => _context.ConsumidoresFinais
        .FirstOrDefaultAsync(x => x.DhExclusao == null && x.Nome.ToLower() == nome.ToLower());

    public async Task<ConsumidorFinal> CadastrarAsync(ConsumidorFinal consumidor)
    {
        await _context.ConsumidoresFinais.AddAsync(consumidor);
        await _context.SaveChangesAsync();
        return consumidor;
    }
}
