using DomainService.DTOs;
using DomainService.DTOs.Request;
using DomainService.Entities;
using DomainService.Interfaces;
using DomainService.Interfaces.Services;

namespace DomainService.services;

public class ConsumidorFinalDomainService : BaseDomainService<ConsumidorFinal>, IConsumidorFinalDomainService
{
    private readonly IConsumidorFinalRepository _consumidorRepository;

    public ConsumidorFinalDomainService(IConsumidorFinalRepository repository) : base(repository)
    {
        _consumidorRepository = repository;
    }

    public async Task<ServiceResponse<List<ConsumidorFinalDTO>>> ListarAtivos()
    {
        var response = new ServiceResponse<List<ConsumidorFinalDTO>>();
        try
        {
            response.Dados = (await _consumidorRepository.ListarAtivosAsync()).Select(x => x.ToDTO()).ToList();
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Mensagem = $"Erro ao listar consumidores: {ex.GetBaseException().Message}";
        }

        return response;
    }

    public async Task<ServiceResponse<ConsumidorFinalDTO>> Cadastrar(ConsumidorFinalRequest request)
    {
        var response = new ServiceResponse<ConsumidorFinalDTO>();
        var nome = string.Join(' ', request.Nome.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .ToUpperInvariant();

        try
        {
            if (await _consumidorRepository.ObterPorNomeAsync(nome) is not null)
            {
                response.Success = false;
                response.Mensagem = "Já existe um consumidor cadastrado com esse nome.";
                return response;
            }

            var consumidor = await _consumidorRepository.CadastrarAsync(new ConsumidorFinal
            {
                Id = Guid.NewGuid(),
                Nome = nome,
                DhInclusao = DateTime.UtcNow
            });

            response.Dados = consumidor.ToDTO();
            response.Mensagem = "Consumidor cadastrado com sucesso.";
        }
        catch (Exception ex)
        {
            response.Success = false;
            response.Mensagem = $"Erro ao cadastrar consumidor: {ex.GetBaseException().Message}";
        }

        return response;
    }
}
