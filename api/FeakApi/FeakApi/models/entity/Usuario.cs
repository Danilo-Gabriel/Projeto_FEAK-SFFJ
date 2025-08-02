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
    }
}
