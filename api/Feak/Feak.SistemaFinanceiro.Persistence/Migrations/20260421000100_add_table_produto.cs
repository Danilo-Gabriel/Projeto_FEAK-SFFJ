using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feak.SistemaFinanceiro.Persistencia.Migrations
{
    public partial class add_table_produto : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Produtos",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    descricao = table.Column<string>(type: "text", nullable: false),
                    preco_custo = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    preco_venda = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    estoque_atual = table.Column<int>(type: "integer", nullable: false),
                    DhInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DhExclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Produtos", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Produtos");
        }
    }
}