using DomainService.Entities;

namespace DomainService.DTOs;

public class UsuarioDTO
{
    public Guid Id {get; set;}
    public string NomeCompleto { get; set; }

    public string NomeLogin { get; set; }

    public string Senha { get; set; }
    
    public DateTime DhInclusao { get; set; }
    
    public DateTime? DhExclusao {get; set;}
    

    public Usuario ToEntity()
    {
        return new Usuario
        {
            Id =  this.Id,
            NomeCompleto = this.NomeCompleto,
            NomeLogin = this.NomeLogin,
            Senha = this.Senha
        };
    }
}