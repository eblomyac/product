using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class productLineValueId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_ProductionLines_ProductLineValueId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "ProductLine",
                table: "Works");

            migrationBuilder.AlterColumn<string>(
                name: "ProductLineValueId",
                table: "Works",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Works_ProductionLines_ProductLineValueId",
                table: "Works",
                column: "ProductLineValueId",
                principalTable: "ProductionLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_ProductionLines_ProductLineValueId",
                table: "Works");

            migrationBuilder.AlterColumn<string>(
                name: "ProductLineValueId",
                table: "Works",
                type: "nvarchar(32)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(32)",
                oldMaxLength: 32);

            migrationBuilder.AddColumn<string>(
                name: "ProductLine",
                table: "Works",
                type: "nvarchar(32)",
                maxLength: 32,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_ProductionLines_ProductLineValueId",
                table: "Works",
                column: "ProductLineValueId",
                principalTable: "ProductionLines",
                principalColumn: "Id");
        }
    }
}
