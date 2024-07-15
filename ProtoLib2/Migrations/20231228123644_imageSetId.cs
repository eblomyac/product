using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class imageSetId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_ImageSet_ImagesId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Works_ImagesId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "ImagesId",
                table: "Works");

            migrationBuilder.AddColumn<Guid>(
                name: "ImageSetId",
                table: "Works",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Works_ImageSetId",
                table: "Works",
                column: "ImageSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_ImageSet_ImageSetId",
                table: "Works",
                column: "ImageSetId",
                principalTable: "ImageSet",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Works_ImageSet_ImageSetId",
                table: "Works");

            migrationBuilder.DropIndex(
                name: "IX_Works_ImageSetId",
                table: "Works");

            migrationBuilder.DropColumn(
                name: "ImageSetId",
                table: "Works");

            migrationBuilder.AddColumn<Guid>(
                name: "ImagesId",
                table: "Works",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Works_ImagesId",
                table: "Works",
                column: "ImagesId");

            migrationBuilder.AddForeignKey(
                name: "FK_Works_ImageSet_ImagesId",
                table: "Works",
                column: "ImagesId",
                principalTable: "ImageSet",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
