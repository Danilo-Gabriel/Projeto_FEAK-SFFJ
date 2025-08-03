using FeakApi.models.dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeakApi.models.entity
{
    public class Usuario
    {
        [Key]
        public int Id { get; set; }

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
                NomeCompleto = this.NomeCompleto,
                NomeLogin = this.NomeLogin,
                Senha = this.Senha
            };
        }
    }
}
