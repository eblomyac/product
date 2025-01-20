using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class CostRepSave : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CostReportRecords",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentMyWait = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentMyWork = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurrentMyEnd = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurentUncompleteWait = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurentUncompleteWork = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CurentUncompleteEnd = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CostReportRecords", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CostReportRecords");
        }
    }
}
