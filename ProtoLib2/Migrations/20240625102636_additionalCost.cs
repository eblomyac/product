using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class additionalCost : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AdditionalCostTemplates",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalCostTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AdditionalCosts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Comment = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    AdditionalCostTemplateId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdditionalCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AdditionalCosts_AdditionalCostTemplates_AdditionalCostTemplateId",
                        column: x => x.AdditionalCostTemplateId,
                        principalTable: "AdditionalCostTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdditionalCosts_Works_WorkId",
                        column: x => x.WorkId,
                        principalTable: "Works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalCosts_AdditionalCostTemplateId",
                table: "AdditionalCosts",
                column: "AdditionalCostTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_AdditionalCosts_WorkId",
                table: "AdditionalCosts",
                column: "WorkId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AdditionalCosts");

            migrationBuilder.DropTable(
                name: "AdditionalCostTemplates");
        }
    }
}
