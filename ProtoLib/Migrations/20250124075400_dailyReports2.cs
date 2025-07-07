using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class dailyReports2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CompletedCost",
                schema: "DailyReport",
                table: "ArticleOrderDaily",
                newName: "CostCompleted");

            migrationBuilder.AlterColumn<decimal>(
                name: "ItemsDone",
                schema: "DailyReport",
                table: "PostDaily",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<decimal>(
                name: "ItemsDone",
                schema: "DailyReport",
                table: "LineDaily",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<decimal>(
                name: "CompletedCount",
                schema: "DailyReport",
                table: "ArticleOrderDaily",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CostCompleted",
                schema: "DailyReport",
                table: "ArticleOrderDaily",
                newName: "CompletedCost");

            migrationBuilder.AlterColumn<int>(
                name: "ItemsDone",
                schema: "DailyReport",
                table: "PostDaily",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<long>(
                name: "ItemsDone",
                schema: "DailyReport",
                table: "LineDaily",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<int>(
                name: "CompletedCount",
                schema: "DailyReport",
                table: "ArticleOrderDaily",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");
        }
    }
}
