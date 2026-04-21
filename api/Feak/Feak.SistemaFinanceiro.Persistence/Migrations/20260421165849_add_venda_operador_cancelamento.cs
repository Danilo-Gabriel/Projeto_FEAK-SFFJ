using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feak.SistemaFinanceiro.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class add_venda_operador_cancelamento : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "cancelada",
                table: "Vendas",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "dh_cancelamento",
                table: "Vendas",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "operador",
                table: "Vendas",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "cancelada",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "dh_cancelamento",
                table: "Vendas");

            migrationBuilder.DropColumn(
                name: "operador",
                table: "Vendas");
        }
    }
}
