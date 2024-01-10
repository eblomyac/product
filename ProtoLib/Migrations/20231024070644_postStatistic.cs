using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class postStatistic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PostStatistics",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNumber = table.Column<long>(type: "bigint", nullable: false),
                    PredictCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InnerCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WaitingCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    RunningCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    SendedCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ActualEvents = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostStatistics", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostStatistics");
        }
    }
}
