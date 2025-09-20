using FeakApi.models.dtos;
using FeakApi.models.entity;
using Microsoft.EntityFrameworkCore;

namespace FeakApi.models.repository
{
    public class ProdutoRepository
    {

        private readonly AppDbContext _context;
        
        public ProdutoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Produto> GetByIdAsync(int id)
        {
            return await _context.Produtos
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<List<Produto>> ObterUsuarios()
        {
            return await _context.Produtos.ToListAsync();
        }

        //public async Task<Produto> GetByNomeLoginAsync(string NomeLogin)
        //{
        //    return await _context.Usuarios
        //        .FirstOrDefaultAsync(u => u.NomeLogin == NomeLogin);
        //}

        public async Task<Produto> CadastrarProduto(Produto dados) 
        {

            await _context.Produtos.AddAsync(dados);
            await _context.SaveChangesAsync();

            return dados;
        }

        public async Task<Produto> AtualizarProduto(Produto dados)
        {
            _context.Produtos.Update(dados);
            await _context.SaveChangesAsync();

            return dados;
        }


        public async Task<Produto> InativaProduto(int id)
        {

            var produto = await _context.Produtos.FindAsync(id);
            if (produto == null)
            {
                throw new Exception("Usuário não encontrado");
            }

            produto.Ativo = false;

            _context.Produtos.Update(produto);
            await _context.SaveChangesAsync();

            return produto;
        }


    }
}
