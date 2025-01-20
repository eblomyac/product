using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class costRepRename : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurentUncompleteWork",
                table: "CostReportRecords",
                newName: "CurrentUncompleteWork");

            migrationBuilder.RenameColumn(
                name: "CurentUncompleteWait",
                table: "CostReportRecords",
                newName: "CurrentUncompleteWait");

            migrationBuilder.RenameColumn(
                name: "CurentUncompleteEnd",
                table: "CostReportRecords",
                newName: "CurrentUncompleteEnd");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CurrentUncompleteWork",
                table: "CostReportRecords",
                newName: "CurentUncompleteWork");

            migrationBuilder.RenameColumn(
                name: "CurrentUncompleteWait",
                table: "CostReportRecords",
                newName: "CurentUncompleteWait");

            migrationBuilder.RenameColumn(
                name: "CurrentUncompleteEnd",
                table: "CostReportRecords",
                newName: "CurentUncompleteEnd");
        }
    }
}
