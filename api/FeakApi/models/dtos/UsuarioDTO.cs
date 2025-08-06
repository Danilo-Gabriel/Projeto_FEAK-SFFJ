using FeakApi.models.entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeakApi.models.dtos
{
    public class UsuarioDTO
    {
        public string NomeCompleto { get; set; }

        public string NomeLogin { get; set; }

        public string Senha { get; set; }
        public bool Ativo{ get; set; }



        public Usuario ToEntity(string Senha)
        {
            return new Usuario
            {
                NomeCompleto = this.NomeCompleto,
                NomeLogin = this.NomeLogin,
                Senha = Senha,
                Ativo = this.Ativo
            };
        }
    }
}
