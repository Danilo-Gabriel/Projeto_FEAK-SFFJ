using System.ComponentModel.DataAnnotations;
using DomainService.Entities;

namespace DomainService.DTOs.Request;

public class UsuarioRequest
{
    [Required(ErrorMessage = "Campo obrigatório")]
    public string NomeCompleto { get; set; }

    [Required(ErrorMessage = "Campo obrigatório")]
    public string NomeLogin { get; set; }

    public string Senha { get; set; }

    public Usuario ToEntity()
    {
        return new Usuario()
        {
            Id = Guid.NewGuid(),
            NomeCompleto = this.NomeCompleto,
            NomeLogin = this.NomeLogin,
            Senha = this.Senha
        };
    }
}