using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class issues : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_IssueTemplates_TemplalteId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_TemplalteId",
                table: "Issues");

            migrationBuilder.DropColumn(
                name: "TemplalteId",
                table: "Issues");

            migrationBuilder.CreateIndex(
                name: "IX_Issues_TemplateId",
                table: "Issues",
                column: "TemplateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_IssueTemplates_TemplateId",
                table: "Issues",
                column: "TemplateId",
                principalTable: "IssueTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Issues_IssueTemplates_TemplateId",
                table: "Issues");

            migrationBuilder.DropIndex(
                name: "IX_Issues_TemplateId",
                table: "Issues");

            migrationBuilder.AddColumn<long>(
                name: "TemplalteId",
                table: "Issues",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Issues_TemplalteId",
                table: "Issues",
                column: "TemplalteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Issues_IssueTemplates_TemplalteId",
                table: "Issues",
                column: "TemplalteId",
                principalTable: "IssueTemplates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
