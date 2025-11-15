using System.ComponentModel.DataAnnotations.Schema;
using DomainService.DTOs;

namespace DomainService.Entities;

public class Usuario : BaseEntity
{
    [Column("nome_completo")]
    public string NomeCompleto { get; set; }

    [Column("nome_login")]
    public string NomeLogin { get; set; }
    
    [Column("senha")]
    public string Senha { get; set; }
    
    
    public UsuarioDTO toDTO()
    {
        return new UsuarioDTO
        {
            Id = this.Id,
            NomeCompleto = this.NomeCompleto,
            NomeLogin = this.NomeLogin,
            Senha = this.Senha,
            DhInclusao = this.DhInclusao,
            DhExclusao = this.DhExclusao,
        };
    }
    
    public Usuario (){}
}