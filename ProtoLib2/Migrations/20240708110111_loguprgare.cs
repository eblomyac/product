using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class loguprgare : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Count",
                table: "WorkStatusLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OrderLineNumber",
                table: "WorkStatusLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProductionLineId",
                table: "WorkStatusLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "SingleCost",
                table: "WorkStatusLogs",
                type: "decimal(18,2)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Count",
                table: "WorkStatusLogs");

            migrationBuilder.DropColumn(
                name: "OrderLineNumber",
                table: "WorkStatusLogs");

            migrationBuilder.DropColumn(
                name: "ProductionLineId",
                table: "WorkStatusLogs");

            migrationBuilder.DropColumn(
                name: "SingleCost",
                table: "WorkStatusLogs");
        }
    }
}
