using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class dailyReports : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "DailyReport");

            migrationBuilder.CreateTable(
                name: "ArticleOrderDaily",
                schema: "DailyReport",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Post = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNumber = table.Column<long>(type: "bigint", nullable: false),
                    OrderLineNumber = table.Column<int>(type: "int", nullable: false),
                    MaxOrderLineNumber = table.Column<int>(type: "int", nullable: false),
                    Article = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompletedCount = table.Column<int>(type: "int", nullable: false),
                    OrderCount = table.Column<int>(type: "int", nullable: false),
                    CompletedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalCompletedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompletedWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RemainsWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ArticleOrderDaily", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "LineDaily",
                schema: "DailyReport",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemsDone = table.Column<long>(type: "bigint", nullable: false),
                    CostCompleted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalCostCompleted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemsDoneCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ItemsReceived = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LineDaily", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PostDaily",
                schema: "DailyReport",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Line = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Post = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ItemsDone = table.Column<int>(type: "int", nullable: false),
                    CostCompleted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AdditionalCostCompleted = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DailyBudget = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CompletedWeight = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostDaily", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ArticleOrderDaily",
                schema: "DailyReport");

            migrationBuilder.DropTable(
                name: "LineDaily",
                schema: "DailyReport");

            migrationBuilder.DropTable(
                name: "PostDaily",
                schema: "DailyReport");
        }
    }
}
