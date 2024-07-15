using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class imageSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Works_WorkId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_WorkId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "WorkId",
                table: "Images");

            migrationBuilder.AddColumn<Guid>(
                name: "ImagesId",
                table: "Works",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

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
                name: "IX_Works_ImagesId",
                table: "Works",
                column: "ImagesId");

            migrationBuilder.CreateIndex(
                name: "IX_ImageSet_ImageId",
                table: "ImageSet",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_ImageSet_ImagesId",
                table: "Works",
                column: "ImagesId",
                principalTable: "ImageSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_ImageSet_ImagesId",
                table: "Works");

            migrationBuilder.DropTable(
                name: "ImageSet");

            migrationBuilder.DropIndex(
                name: "IX_Works_ImagesId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "ImagesId",
                table: "Works");

            migrationBuilder.AddColumn<long>(
                name: "WorkId",
                table: "Images",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_WorkId",
                table: "Images",
                column: "WorkId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Works_WorkId",
                table: "Images",
                column: "WorkId",
                principalTable: "Works",
                principalColumn: "Id");
        }
    }
}
