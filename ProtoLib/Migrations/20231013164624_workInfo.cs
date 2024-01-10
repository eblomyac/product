using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class workInfo : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "Works",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ProductLine",
                table: "Works",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "ProductLine",
                table: "Works");
        }
    }
}
