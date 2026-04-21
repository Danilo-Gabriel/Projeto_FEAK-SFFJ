using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feak.SistemaFinanceiro.Persistencia.Migrations
{
    public partial class add_vendas_and_codigo_barras : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "codigo_barras",
                table: "Produtos",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "Vendas",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    numero_venda = table.Column<string>(type: "text", nullable: false),
                    consumidor = table.Column<string>(type: "text", nullable: false),
                    forma_pagamento = table.Column<string>(type: "text", nullable: false),
                    subtotal = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    desconto_total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    acrescimo = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DhInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DhExclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vendas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VendaItens",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    venda_id = table.Column<Guid>(type: "uuid", nullable: false),
                    produto_id = table.Column<Guid>(type: "uuid", nullable: false),
                    codigo_produto = table.Column<string>(type: "text", nullable: false),
                    descricao_produto = table.Column<string>(type: "text", nullable: false),
                    quantidade = table.Column<int>(type: "integer", nullable: false),
                    preco_unitario = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    desconto_valor = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    desconto_percentual = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    total_item = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    DhInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DhExclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendaItens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VendaItens_Produtos_produto_id",
                        column: x => x.produto_id,
                        principalTable: "Produtos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VendaItens_Vendas_venda_id",
                        column: x => x.venda_id,
                        principalTable: "Vendas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_codigo_barras",
                table: "Produtos",
                column: "codigo_barras",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VendaItens_produto_id",
                table: "VendaItens",
                column: "produto_id");

            migrationBuilder.CreateIndex(
                name: "IX_VendaItens_venda_id",
                table: "VendaItens",
                column: "venda_id");

            migrationBuilder.Sql(@"
                UPDATE ""Produtos""
                SET codigo_barras = 'PDV-' || SUBSTRING(REPLACE(CAST(""Id"" AS text), '-', ''), 1, 10)
                WHERE codigo_barras = '';
            ");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "VendaItens");
            migrationBuilder.DropTable(name: "Vendas");
            migrationBuilder.DropIndex(name: "IX_Produtos_codigo_barras", table: "Produtos");
            migrationBuilder.DropColumn(name: "codigo_barras", table: "Produtos");
        }
    }
}