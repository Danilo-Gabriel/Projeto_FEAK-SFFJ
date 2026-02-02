using System.ComponentModel.DataAnnotations;

namespace DomainService.DTOs;

public class LoginDTO
{
    
    [Required(ErrorMessage = "Campo obrigatório")]
    public string NomeLogin { get; set; } = string.Empty;
    [Required(ErrorMessage = "Campo obrigatório")]
    public string Senha { get; set; } = string.Empty;
}