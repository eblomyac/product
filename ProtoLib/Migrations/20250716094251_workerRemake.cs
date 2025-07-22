using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class workerRemake : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAvailableRoles");

            migrationBuilder.DropTable(
                name: "ProductRoles");

            migrationBuilder.AddColumn<string>(
                name: "PostString",
                table: "ProductWorkers",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Title",
                table: "ProductWorkers",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostString",
                table: "ProductWorkers");

            migrationBuilder.DropColumn(
                name: "Title",
                table: "ProductWorkers");

            migrationBuilder.CreateTable(
                name: "ProductAvailableRoles",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductAvailableRoles", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "ProductRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductWorkerId = table.Column<long>(type: "bigint", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductRoles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductRoles_ProductWorkers_ProductWorkerId",
                        column: x => x.ProductWorkerId,
                        principalTable: "ProductWorkers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductRoles_ProductWorkerId",
                table: "ProductRoles",
                column: "ProductWorkerId");
        }
    }
}
