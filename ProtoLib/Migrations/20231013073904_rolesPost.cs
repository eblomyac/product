using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class rolesPost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Roles_Posts_PostName",
                table: "Roles");

            migrationBuilder.DropIndex(
                name: "IX_Roles_PostName",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "PostName",
                table: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "Roles",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Roles");

            migrationBuilder.AddColumn<string>(
                name: "PostName",
                table: "Roles",
                type: "nvarchar(32)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_PostName",
                table: "Roles",
                column: "PostName");

            migrationBuilder.AddForeignKey(
                name: "FK_Roles_Posts_PostName",
                table: "Roles",
                column: "PostName",
                principalTable: "Posts",
                principalColumn: "Name");
        }
    }
}
