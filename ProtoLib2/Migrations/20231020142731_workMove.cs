using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class workMove : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndReason",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "StartReason",
                table: "Works");

            migrationBuilder.AddColumn<string>(
                name: "MovedFrom",
                table: "Works",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MovedTo",
                table: "Works",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MovedFrom",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "MovedTo",
                table: "Works");

            migrationBuilder.AddColumn<string>(
                name: "EndReason",
                table: "Works",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "StartReason",
                table: "Works",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }
    }
}
