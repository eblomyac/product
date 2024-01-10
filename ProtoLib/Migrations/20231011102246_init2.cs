using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class init2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Posts",
                columns: table => new
                {
                    Name = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Posts", x => x.Name);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    AccName = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    Mail = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.AccName);
                });

            migrationBuilder.CreateTable(
                name: "PostKeys",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PostId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Key = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostKeys", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostKeys_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Works",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OrderNumber = table.Column<long>(type: "bigint", nullable: false),
                    Article = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    SingleCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Works", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Works_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    UserAccName = table.Column<string>(type: "nvarchar(32)", nullable: false),
                    PostName = table.Column<string>(type: "nvarchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Posts_PostName",
                        column: x => x.PostName,
                        principalTable: "Posts",
                        principalColumn: "Name");
                    table.ForeignKey(
                        name: "FK_Roles_Users_UserAccName",
                        column: x => x.UserAccName,
                        principalTable: "Users",
                        principalColumn: "AccName",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    WorkId = table.Column<long>(type: "bigint", nullable: false),
                    PrevStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    Stamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EditedByAccName = table.Column<string>(type: "nvarchar(32)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WorkLogs_Users_EditedByAccName",
                        column: x => x.EditedByAccName,
                        principalTable: "Users",
                        principalColumn: "AccName");
                    table.ForeignKey(
                        name: "FK_WorkLogs_Works_WorkId",
                        column: x => x.WorkId,
                        principalTable: "Works",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PostKeys_PostId",
                table: "PostKeys",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_PostName",
                table: "Roles",
                column: "PostName");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_UserAccName",
                table: "Roles",
                column: "UserAccName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_EditedByAccName",
                table: "WorkLogs",
                column: "EditedByAccName");

            migrationBuilder.CreateIndex(
                name: "IX_WorkLogs_WorkId",
                table: "WorkLogs",
                column: "WorkId");

            migrationBuilder.CreateIndex(
                name: "IX_Works_PostId",
                table: "Works",
                column: "PostId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PostKeys");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "WorkLogs");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Works");

            migrationBuilder.DropTable(
                name: "Posts");
        }
    }
}
