using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class editImages : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TechCardPosts_ImageSet_ImageSetId",
                table: "TechCardPosts");

            migrationBuilder.DropIndex(
                name: "IX_TechCardPosts_ImageSetId",
                table: "TechCardPosts");

            migrationBuilder.DropColumn(
                name: "ImageSetId",
                table: "TechCardPosts");

            migrationBuilder.DropColumn(
                name: "ImageSetId",
                table: "TechCardLines");

            migrationBuilder.AddColumn<string>(
                name: "PostId",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "UploadedBy",
                table: "Images",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PostId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "UploadedBy",
                table: "Images");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageSetId",
                table: "TechCardPosts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ImageSetId",
                table: "TechCardLines",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_TechCardPosts_ImageSetId",
                table: "TechCardPosts",
                column: "ImageSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_TechCardPosts_ImageSet_ImageSetId",
                table: "TechCardPosts",
                column: "ImageSetId",
                principalTable: "ImageSet",
                principalColumn: "Id");
        }
    }
}
