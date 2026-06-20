using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Feak.SistemaFinanceiro.Persistencia.Migrations
{
    /// <inheritdoc />
    public partial class padronizar_schema_snake_case : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VendaItens_Produtos_produto_id",
                table: "VendaItens");

            migrationBuilder.DropForeignKey(
                name: "FK_VendaItens_Vendas_venda_id",
                table: "VendaItens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Vendas",
                table: "Vendas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_VendaItens",
                table: "VendaItens");

            migrationBuilder.RenameTable(
                name: "Vendas",
                newName: "vendas");

            migrationBuilder.RenameTable(
                name: "Usuarios",
                newName: "usuarios");

            migrationBuilder.RenameTable(
                name: "Produtos",
                newName: "produtos");

            migrationBuilder.RenameTable(
                name: "VendaItens",
                newName: "venda_itens");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "vendas",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DhInclusao",
                table: "vendas",
                newName: "dh_inclusao");

            migrationBuilder.RenameColumn(
                name: "DhExclusao",
                table: "vendas",
                newName: "dh_exclusao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "usuarios",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DhInclusao",
                table: "usuarios",
                newName: "dh_inclusao");

            migrationBuilder.RenameColumn(
                name: "DhExclusao",
                table: "usuarios",
                newName: "dh_exclusao");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "produtos",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DhInclusao",
                table: "produtos",
                newName: "dh_inclusao");

            migrationBuilder.RenameColumn(
                name: "DhExclusao",
                table: "produtos",
                newName: "dh_exclusao");

            migrationBuilder.RenameIndex(
                name: "IX_Produtos_codigo_barras",
                table: "produtos",
                newName: "IX_produtos_codigo_barras");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "venda_itens",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DhInclusao",
                table: "venda_itens",
                newName: "dh_inclusao");

            migrationBuilder.RenameColumn(
                name: "DhExclusao",
                table: "venda_itens",
                newName: "dh_exclusao");

            migrationBuilder.RenameIndex(
                name: "IX_VendaItens_venda_id",
                table: "venda_itens",
                newName: "IX_venda_itens_venda_id");

            migrationBuilder.RenameIndex(
                name: "IX_VendaItens_produto_id",
                table: "venda_itens",
                newName: "IX_venda_itens_produto_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_vendas",
                table: "vendas",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_produtos",
                table: "produtos",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_venda_itens",
                table: "venda_itens",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_venda_itens_produtos_produto_id",
                table: "venda_itens",
                column: "produto_id",
                principalTable: "produtos",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_venda_itens_vendas_venda_id",
                table: "venda_itens",
                column: "venda_id",
                principalTable: "vendas",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_venda_itens_produtos_produto_id",
                table: "venda_itens");

            migrationBuilder.DropForeignKey(
                name: "FK_venda_itens_vendas_venda_id",
                table: "venda_itens");

            migrationBuilder.DropPrimaryKey(
                name: "PK_vendas",
                table: "vendas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_usuarios",
                table: "usuarios");

            migrationBuilder.DropPrimaryKey(
                name: "PK_produtos",
                table: "produtos");

            migrationBuilder.DropPrimaryKey(
                name: "PK_venda_itens",
                table: "venda_itens");

            migrationBuilder.RenameTable(
                name: "vendas",
                newName: "Vendas");

            migrationBuilder.RenameTable(
                name: "usuarios",
                newName: "Usuarios");

            migrationBuilder.RenameTable(
                name: "produtos",
                newName: "Produtos");

            migrationBuilder.RenameTable(
                name: "venda_itens",
                newName: "VendaItens");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Vendas",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "dh_inclusao",
                table: "Vendas",
                newName: "DhInclusao");

            migrationBuilder.RenameColumn(
                name: "dh_exclusao",
                table: "Vendas",
                newName: "DhExclusao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Usuarios",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "dh_inclusao",
                table: "Usuarios",
                newName: "DhInclusao");

            migrationBuilder.RenameColumn(
                name: "dh_exclusao",
                table: "Usuarios",
                newName: "DhExclusao");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Produtos",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "dh_inclusao",
                table: "Produtos",
                newName: "DhInclusao");

            migrationBuilder.RenameColumn(
                name: "dh_exclusao",
                table: "Produtos",
                newName: "DhExclusao");

            migrationBuilder.RenameIndex(
                name: "IX_produtos_codigo_barras",
                table: "Produtos",
                newName: "IX_Produtos_codigo_barras");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "VendaItens",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "dh_inclusao",
                table: "VendaItens",
                newName: "DhInclusao");

            migrationBuilder.RenameColumn(
                name: "dh_exclusao",
                table: "VendaItens",
                newName: "DhExclusao");

            migrationBuilder.RenameIndex(
                name: "IX_venda_itens_venda_id",
                table: "VendaItens",
                newName: "IX_VendaItens_venda_id");

            migrationBuilder.RenameIndex(
                name: "IX_venda_itens_produto_id",
                table: "VendaItens",
                newName: "IX_VendaItens_produto_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Vendas",
                table: "Vendas",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Usuarios",
                table: "Usuarios",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Produtos",
                table: "Produtos",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_VendaItens",
                table: "VendaItens",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_VendaItens_Produtos_produto_id",
                table: "VendaItens",
                column: "produto_id",
                principalTable: "Produtos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_VendaItens_Vendas_venda_id",
                table: "VendaItens",
                column: "venda_id",
                principalTable: "Vendas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
