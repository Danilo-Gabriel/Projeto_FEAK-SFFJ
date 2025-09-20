using FeakApi.models.entity;
using System.ComponentModel.DataAnnotations.Schema;

namespace FeakApi.models.dtos
{
    public class ProdutoDTO
    {
        public int? Id { get; set; }
        public string Descricao { get; set; }

        public string EstoqueAtual { get; set; }

        public decimal PrecoVenda { get; set; }

        public decimal PrecoCusto { get; set; }
        public bool Ativo{ get; set; }



        public Produto ToEntity()
        {
            return new Produto
            {
                Descricao = this.Descricao,
                EstoqueAtual = this.EstoqueAtual,
                PrecoVenda = this.PrecoVenda,
                PrecoCusto = this.PrecoCusto,
                Ativo = true 
            };
        }
    }
}
