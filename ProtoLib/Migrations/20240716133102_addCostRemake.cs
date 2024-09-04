using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class addCostRemake : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Disabled",
                table: "AdditionalCostTemplates",
                newName: "CanPost");

            migrationBuilder.AddColumn<bool>(
                name: "CanItem",
                table: "AdditionalCostTemplates",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanItem",
                table: "AdditionalCostTemplates");

            migrationBuilder.RenameColumn(
                name: "CanPost",
                table: "AdditionalCostTemplates",
                newName: "Disabled");
        }
    }
}
