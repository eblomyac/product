using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class prodLineLast : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailySources_ProductionLines_ProdutLineItemId",
                table: "DailySources");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_ProductionLines_ProductLineValueId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Works_ProductLineValueId",
                table: "Works");

            migrationBuilder.RenameColumn(
                name: "ProductLineValueId",
                table: "Works",
                newName: "ProductLineId");

            migrationBuilder.RenameColumn(
                name: "ProdutLineItemId",
                table: "DailySources",
                newName: "ProdutLineId");

            migrationBuilder.RenameColumn(
                name: "ProductLine",
                table: "DailySources",
                newName: "ProductLineId");

            migrationBuilder.RenameIndex(
                name: "IX_DailySources_ProdutLineItemId",
                table: "DailySources",
                newName: "IX_DailySources_ProdutLineId");

            migrationBuilder.AddColumn<string>(
                name: "ProductLine",
                table: "Works",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_DailySources_ProductionLines_ProdutLineId",
                table: "DailySources",
                column: "ProdutLineId",
                principalTable: "ProductionLines",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailySources_ProductionLines_ProdutLineId",
                table: "DailySources");

            migrationBuilder.DropColumn(
                name: "ProductLine",
                table: "Works");

            migrationBuilder.RenameColumn(
                name: "ProductLineId",
                table: "Works",
                newName: "ProductLineValueId");

            migrationBuilder.RenameColumn(
                name: "ProdutLineId",
                table: "DailySources",
                newName: "ProdutLineItemId");

            migrationBuilder.RenameColumn(
                name: "ProductLineId",
                table: "DailySources",
                newName: "ProductLine");

            migrationBuilder.RenameIndex(
                name: "IX_DailySources_ProdutLineId",
                table: "DailySources",
                newName: "IX_DailySources_ProdutLineItemId");

            migrationBuilder.CreateIndex(
                name: "IX_Works_ProductLineValueId",
                table: "Works",
                column: "ProductLineValueId");

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
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
