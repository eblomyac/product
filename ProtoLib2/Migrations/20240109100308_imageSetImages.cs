using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class imageSetImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ImageSet_Images_ImageId",
                table: "ImageSet");

            migrationBuilder.DropIndex(
                name: "IX_ImageSet_ImageId",
                table: "ImageSet");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageSetId",
                table: "Images",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_ImageSet_ImageSetId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_ImageSetId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "ImageSetId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "Images");

            migrationBuilder.CreateIndex(
                name: "IX_ImageSet_ImageId",
                table: "ImageSet",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_ImageSet_Images_ImageId",
                table: "ImageSet",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
