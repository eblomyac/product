using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class prodLine : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductLineValueId",
                table: "Works",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductLine",
                table: "DailySources",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProdutLineItemId",
                table: "DailySources",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ProductionLine",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionLine", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Works_ProductLineValueId",
                table: "Works",
                column: "ProductLineValueId");

            migrationBuilder.CreateIndex(
                name: "IX_DailySources_ProdutLineItemId",
                table: "DailySources",
                column: "ProdutLineItemId");

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DailySources_ProductionLine_ProdutLineItemId",
                table: "DailySources");

            migrationBuilder.DropForeignKey(
                name: "FK_Works_ProductionLine_ProductLineValueId",
                table: "Works");

            migrationBuilder.DropTable(
                name: "ProductionLine");

            migrationBuilder.DropIndex(
                name: "IX_Works_ProductLineValueId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_DailySources_ProdutLineItemId",
                table: "DailySources");

            migrationBuilder.DropColumn(
                name: "ProductLineValueId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "ProductLine",
                table: "DailySources");

            migrationBuilder.DropColumn(
                name: "ProdutLineItemId",
                table: "DailySources");
        }
    }
}
