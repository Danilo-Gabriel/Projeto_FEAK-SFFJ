using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feak.SistemaFinanceiro.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class adicionar_sequence_numero_venda : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "feak_sf");

            migrationBuilder.CreateSequence(
                name: "numero_venda_seq",
                schema: "feak_sf");

            migrationBuilder.Sql(
                """
                SELECT setval(
                    'feak_sf.numero_venda_seq',
                    COALESCE((
                        SELECT MAX(CAST(SUBSTRING(numero_venda FROM 3) AS bigint))
                        FROM feak_sf.vendas
                        WHERE numero_venda ~ '^VD[0-9]+$'
                    ), 0) + 1,
                    false
                );
                """);

            migrationBuilder.CreateIndex(
                name: "IX_vendas_numero_venda",
                table: "vendas",
                column: "numero_venda",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_vendas_numero_venda",
                table: "vendas");

            migrationBuilder.DropSequence(
                name: "numero_venda_seq",
                schema: "feak_sf");
        }
    }
}
