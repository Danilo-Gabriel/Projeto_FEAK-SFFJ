using System.ComponentModel.DataAnnotations;

namespace DomainService.DTOs.Request;

public class UsuarioAtualizacaoRequest
{
    [Required]
    public Guid Id { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "Campo obrigatório")]
    public string NomeLogin { get; set; } = string.Empty;
}