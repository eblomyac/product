using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class otkCheckList : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OTKAvailableOperations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ShortName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    ProductLine = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTKAvailableOperations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OTKChecks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Stamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductLine = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Article = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    OrderNumber = table.Column<long>(type: "bigint", nullable: false),
                    OrderLineNumber = table.Column<int>(type: "int", nullable: false),
                    Iteration = table.Column<int>(type: "int", nullable: false),
                    ProductCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CheckedCount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Worker = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTKChecks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OTKCheckLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OTKCheckId = table.Column<long>(type: "bigint", nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    FullName = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: false),
                    Value = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OTKCheckLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OTKCheckLines_OTKChecks_OTKCheckId",
                        column: x => x.OTKCheckId,
                        principalTable: "OTKChecks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OTKCheckLines_OTKCheckId",
                table: "OTKCheckLines",
                column: "OTKCheckId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OTKAvailableOperations");

            migrationBuilder.DropTable(
                name: "OTKCheckLines");

            migrationBuilder.DropTable(
                name: "OTKChecks");
        }
    }
}
