using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class prodLineLastFix : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailySources_ProductionLines_ProdutLineId",
                table: "DailySources");

            migrationBuilder.DropIndex(
                name: "IX_DailySources_ProdutLineId",
                table: "DailySources");

            migrationBuilder.DropColumn(
                name: "ProdutLineId",
                table: "DailySources");

            migrationBuilder.CreateIndex(
                name: "IX_DailySources_ProductLineId",
                table: "DailySources",
                column: "ProductLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailySources_ProductionLines_ProductLineId",
                table: "DailySources",
                column: "ProductLineId",
                principalTable: "ProductionLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailySources_ProductionLines_ProductLineId",
                table: "DailySources");

            migrationBuilder.DropIndex(
                name: "IX_DailySources_ProductLineId",
                table: "DailySources");

            migrationBuilder.AddColumn<string>(
                name: "ProdutLineId",
                table: "DailySources",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_DailySources_ProdutLineId",
                table: "DailySources",
                column: "ProdutLineId");

            migrationBuilder.AddForeignKey(
                name: "FK_DailySources_ProductionLines_ProdutLineId",
                table: "DailySources",
                column: "ProdutLineId",
                principalTable: "ProductionLines",
                principalColumn: "Id");
        }
    }
}
