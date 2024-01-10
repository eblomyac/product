using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class postStatRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InnerCost",
                table: "PostStatistics",
                newName: "IncomeCost");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IncomeCost",
                table: "PostStatistics",
                newName: "InnerCost");
        }
    }
}
