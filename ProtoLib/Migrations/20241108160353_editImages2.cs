using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class editImages2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_ImageSet_ImageSetId",
                table: "Images");

            migrationBuilder.DropForeignKey(
                name: "FK_TechCards_ImageSet_ImageSetId",
                table: "TechCards");

            migrationBuilder.DropTable(
                name: "ImageSet");

            migrationBuilder.DropIndex(
                name: "IX_TechCards_ImageSetId",
                table: "TechCards");

            migrationBuilder.DropIndex(
                name: "IX_Images_ImageSetId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ImageSetId",
                table: "TechCards");

            migrationBuilder.DropColumn(
                name: "ImageSetId",
                table: "Images");

            migrationBuilder.AddColumn<Guid>(
                name: "TechCardId",
                table: "Images",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Images_TechCardId",
                table: "Images",
                column: "TechCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_TechCards_TechCardId",
                table: "Images",
                column: "TechCardId",
                principalTable: "TechCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_TechCards_TechCardId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_TechCardId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "TechCardId",
                table: "Images");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageSetId",
                table: "TechCards",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImageSetId",
                table: "Images",
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
                });

            migrationBuilder.CreateIndex(
                name: "IX_TechCards_ImageSetId",
                table: "TechCards",
                column: "ImageSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Images_ImageSetId",
                table: "Images",
                column: "ImageSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_ImageSet_ImageSetId",
                table: "Images",
                column: "ImageSetId",
                principalTable: "ImageSet",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TechCards_ImageSet_ImageSetId",
                table: "TechCards",
                column: "ImageSetId",
                principalTable: "ImageSet",
                principalColumn: "Id");
        }
    }
}
