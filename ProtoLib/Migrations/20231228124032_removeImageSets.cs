using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class removeImageSets : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_ImageSet_ImageSetId",
                table: "Works");

            migrationBuilder.DropTable(
                name: "ImageSet");

            migrationBuilder.DropIndex(
                name: "IX_Works_ImageSetId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "ImageSetId",
                table: "Works");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ImageSetId",
                table: "Works",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ImageSet",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ImageId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImageSet", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImageSet_Images_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Images",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Works_ImageSetId",
                table: "Works",
                column: "ImageSetId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageSet_ImageId",
                table: "ImageSet",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_ImageSet_ImageSetId",
                table: "Works",
                column: "ImageSetId",
                principalTable: "ImageSet",
                principalColumn: "Id");
        }
    }
}
