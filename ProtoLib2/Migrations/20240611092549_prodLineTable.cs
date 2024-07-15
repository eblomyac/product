using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class prodLineTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailySources_ProductionLine_ProdutLineItemId",
                table: "DailySources");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_ProductionLine_ProductLineValueId",
                table: "Works");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionLine",
                table: "ProductionLine");

            migrationBuilder.RenameTable(
                name: "ProductionLine",
                newName: "ProductionLines");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionLines",
                table: "ProductionLines",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailySources_ProductionLines_ProdutLineItemId",
                table: "DailySources",
                column: "ProdutLineItemId",
                principalTable: "ProductionLines",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_ProductionLines_ProductLineValueId",
                table: "Works",
                column: "ProductLineValueId",
                principalTable: "ProductionLines",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailySources_ProductionLines_ProdutLineItemId",
                table: "DailySources");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_ProductionLines_ProductLineValueId",
                table: "Works");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ProductionLines",
                table: "ProductionLines");

            migrationBuilder.RenameTable(
                name: "ProductionLines",
                newName: "ProductionLine");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ProductionLine",
                table: "ProductionLine",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_DailySources_ProductionLine_ProdutLineItemId",
                table: "DailySources",
                column: "ProdutLineItemId",
                principalTable: "ProductionLine",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_ProductionLine_ProductLineValueId",
                table: "Works",
                column: "ProductLineValueId",
                principalTable: "ProductionLine",
                principalColumn: "Id");
        }
    }
}
