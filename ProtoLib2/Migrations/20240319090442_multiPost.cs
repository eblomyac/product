using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class multiPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "MasterPostMap",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MasterPostMap",
                table: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
