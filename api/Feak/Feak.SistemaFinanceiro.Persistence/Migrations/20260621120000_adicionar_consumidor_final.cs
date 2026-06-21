using Feak.SistemaFinanceiro.Persistencia.Context;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feak.SistemaFinanceiro.Persistencia.Migrations;

[DbContext(typeof(ApplicationDbcontext))]
[Migration("20260621120000_adicionar_consumidor_final")]
public partial class adicionar_consumidor_final : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "consumidores_finais",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                nome = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                dh_inclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                dh_exclusao = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_consumidores_finais", x => x.id));

        migrationBuilder.CreateIndex(
            name: "IX_consumidores_finais_nome",
            table: "consumidores_finais",
            column: "nome",
            unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "consumidores_finais");
    }
}
