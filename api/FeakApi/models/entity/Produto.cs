using FeakApi.models.dtos;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeakApi.models.entity
{
    public class Produto
    {
        [Key]
        public int? Id { get; set; }

        [Column("descricao")]
        public string Descricao{ get; set; }

        [Column("estoque_atual")]
        public string EstoqueAtual{ get; set; }

        [Column("preco_venda")]
        public decimal PrecoVenda { get; set; }

        [Column("preco_custo")]
        public decimal PrecoCusto{ get; set; }

        [Column("ativo")]
        public bool Ativo { get; set; }


        public ProdutoDTO toDTO()
        {
            return new ProdutoDTO
            {

                Ativo = this.Id == 0 ? true : this.Ativo
            };
        }
    }
}
