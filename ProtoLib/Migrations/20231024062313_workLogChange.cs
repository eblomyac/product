using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class workLogChange : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.CreateTable(
                name: "WorkStatusLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    PrevStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    Stamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedByAccName = table.Column<string>(type: "nvarchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkStatusLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkStatusLogs_Users_EditedByAccName",
                        column: x => x.EditedByAccName,
                        principalTable: "Users",
                        principalColumn: "AccName");
                    table.ForeignKey(
                        name: "FK_WorkStatusLogs_Works_WorkId",
                        column: x => x.WorkId,
                        principalTable: "Works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkStatusLogs_EditedByAccName",
                table: "WorkStatusLogs",
                column: "EditedByAccName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkStatusLogs_WorkId",
                table: "WorkStatusLogs",
                column: "WorkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkStatusLogs");

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EditedByAccName = table.Column<string>(type: "nvarchar(32)", nullable: true),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    NewCount = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    PrevCount = table.Column<int>(type: "int", nullable: false),
                    PrevStatus = table.Column<int>(type: "int", nullable: false),
                    Stamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogs_Users_EditedByAccName",
                        column: x => x.EditedByAccName,
                        principalTable: "Users",
                        principalColumn: "AccName");
                    table.ForeignKey(
                        name: "FK_WorkLogs_Works_WorkId",
                        column: x => x.WorkId,
                        principalTable: "Works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_EditedByAccName",
                table: "WorkLogs",
                column: "EditedByAccName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_WorkId",
                table: "WorkLogs",
                column: "WorkId");
        }
    }
}
