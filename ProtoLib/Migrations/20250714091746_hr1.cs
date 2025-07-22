using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class hr1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "ProductWorkers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductWorkers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProductRoles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ProductWorkerId = table.Column<long>(type: "bigint", nullable: false),
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductAvailableRoles");

            migrationBuilder.DropTable(
                name: "ProductRoles");

            migrationBuilder.DropTable(
                name: "ProductWorkers");
        }
    }
}
