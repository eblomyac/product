using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class logupgradeMove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MovedFrom",
                table: "WorkStatusLogs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MovedTo",
                table: "WorkStatusLogs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovedFrom",
                table: "WorkStatusLogs");

            migrationBuilder.DropColumn(
                name: "MovedTo",
                table: "WorkStatusLogs");
        }
    }
}
