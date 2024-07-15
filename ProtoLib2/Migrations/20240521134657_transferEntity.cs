using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class transferEntity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transfers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PaperId = table.Column<string>(type: "nvarchar(64)", maxLength: 64, nullable: false),
                    PostFromId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    PostToId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedStamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Closed = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ClosedStamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ClosedBy = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Transfers_Posts_PostFromId",
                        column: x => x.PostFromId,
                        principalTable: "Posts",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.NoAction);
                    table.ForeignKey(
                        name: "FK_Transfers_Posts_PostToId",
                        column: x => x.PostToId,
                        principalTable: "Posts",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateTable(
                name: "TransferLines",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Article = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OrderNumber = table.Column<long>(type: "bigint", nullable: false),
                    Count = table.Column<int>(type: "int", nullable: false),
                    ProductionLine = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsTransfered = table.Column<bool>(type: "bit", nullable: false),
                    TransferedCount = table.Column<int>(type: "int", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    SourceWorkId = table.Column<long>(type: "bigint", nullable: false),
                    SourceWorkCost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TargetWorkId = table.Column<long>(type: "bigint", nullable: true),
                    TransferId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TransferLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TransferLines_Transfers_TransferId",
                        column: x => x.TransferId,
                        principalTable: "Transfers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_TransferLines_TransferId",
                table: "TransferLines",
                column: "TransferId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_PostFromId",
                table: "Transfers",
                column: "PostFromId");

            migrationBuilder.CreateIndex(
                name: "IX_Transfers_PostToId",
                table: "Transfers",
                column: "PostToId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TransferLines");

            migrationBuilder.DropTable(
                name: "Transfers");
        }
    }
}
