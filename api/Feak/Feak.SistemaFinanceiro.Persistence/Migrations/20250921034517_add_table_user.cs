using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feak.SistemaFinanceiro.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class add_table_user : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    nome_completo = table.Column<string>(type: "text", nullable: false),
                    nome_login = table.Column<string>(type: "text", nullable: false),
                    senha = table.Column<string>(type: "text", nullable: false),
                    DhInclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    DhExclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Usuarios");
        }
    }
}
