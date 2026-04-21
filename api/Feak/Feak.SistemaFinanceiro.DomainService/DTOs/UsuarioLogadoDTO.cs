namespace DomainService.DTOs;

public class UsuarioLogadoDTO
{
    public string? Id { get; set; }
    public string? NomeLogin { get; set; }
    public string? NomeCompleto { get; set; }
    public string? Email { get; set; }
    public bool Autenticado { get; set; }
    public IEnumerable<string> Perfis { get; set; } = Enumerable.Empty<string>();
}