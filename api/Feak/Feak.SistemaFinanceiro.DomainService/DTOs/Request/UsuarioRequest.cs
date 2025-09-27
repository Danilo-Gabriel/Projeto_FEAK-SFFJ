namespace DomainService.DTOs.Request;

public class UsuarioRequest
{
    public string NomeCompleto { get; set; }

    public string NomeLogin { get; set; }

    public string Senha { get; set; }
}