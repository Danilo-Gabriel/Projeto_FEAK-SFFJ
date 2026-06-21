namespace DomainService.DTOs;

public class ConsumidorFinalDTO
{
    public Guid Id { get; set; }
    public string Nome { get; set; } = string.Empty;
    public DateTime DhInclusao { get; set; }
}
