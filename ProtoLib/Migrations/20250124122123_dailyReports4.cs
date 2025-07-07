using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class dailyReports4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "RemainPart",
                schema: "DailyReport",
                table: "ArticleOrderDaily",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCompletedWeight",
                schema: "DailyReport",
                table: "ArticleOrderDaily",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RemainPart",
                schema: "DailyReport",
                table: "ArticleOrderDaily");

            migrationBuilder.DropColumn(
                name: "TotalCompletedWeight",
                schema: "DailyReport",
                table: "ArticleOrderDaily");
        }
    }
}
