using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class workLogChangeAnalitic : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_WorkStatusLogs_Users_EditedByAccName",
                table: "WorkStatusLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkStatusLogs_Works_WorkId",
                table: "WorkStatusLogs");

            migrationBuilder.DropIndex(
                name: "IX_WorkStatusLogs_EditedByAccName",
                table: "WorkStatusLogs");

            migrationBuilder.DropIndex(
                name: "IX_WorkStatusLogs_WorkId",
                table: "WorkStatusLogs");

            migrationBuilder.DropColumn(
                name: "EditedByAccName",
                table: "WorkStatusLogs");

            migrationBuilder.AddColumn<string>(
                name: "Article",
                table: "WorkStatusLogs",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "EditedBy",
                table: "WorkStatusLogs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "OrderNumber",
                table: "WorkStatusLogs",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "WorkStatusLogs",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Article",
                table: "WorkStatusLogs");

            migrationBuilder.DropColumn(
                name: "EditedBy",
                table: "WorkStatusLogs");

            migrationBuilder.DropColumn(
                name: "OrderNumber",
                table: "WorkStatusLogs");

            migrationBuilder.DropColumn(
                name: "PostId",
                table: "WorkStatusLogs");

            migrationBuilder.AddColumn<string>(
                name: "EditedByAccName",
                table: "WorkStatusLogs",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_WorkStatusLogs_EditedByAccName",
                table: "WorkStatusLogs",
                column: "EditedByAccName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkStatusLogs_WorkId",
                table: "WorkStatusLogs",
                column: "WorkId");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkStatusLogs_Users_EditedByAccName",
                table: "WorkStatusLogs",
                column: "EditedByAccName",
                principalTable: "Users",
                principalColumn: "AccName");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkStatusLogs_Works_WorkId",
                table: "WorkStatusLogs",
                column: "WorkId",
                principalTable: "Works",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
