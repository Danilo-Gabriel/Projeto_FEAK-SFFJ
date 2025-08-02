using FeakApi.models.dtos;
using Npgsql;
using System.Data;
using Dapper;


namespace FeakApi.models.repository
{
    public class DapperGenericRepository
    {
        private readonly IDbConnection _connection;

        public DapperGenericRepository(IDbConnection connection)
        {
            _connection = connection;
        }

        public async Task<IEnumerable<DapperGenericDTO>> ObterTotaisPorUsuarioAsync()
        {
            var sql = @"
            SELECT u.nome AS NomeUsuario, SUM(p.valor) AS TotalPedidos
            FROM usuarios u
            JOIN pedidos p ON p.usuarioid = u.id
            GROUP BY u.nome;
        ";

            return await _connection.QueryAsync<DapperGenericDTO>(sql);
        }
    }
}
