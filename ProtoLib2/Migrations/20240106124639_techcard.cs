using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ProtoLib.Migrations
{
    public partial class techcard : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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

            migrationBuilder.CreateTable(
                name: "TechCards",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Article = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Identity = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ImageSetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechCards_ImageSet_ImageSetId",
                        column: x => x.ImageSetId,
                        principalTable: "ImageSet",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TechCardPosts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechCardId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PostId = table.Column<string>(type: "nvarchar(32)", maxLength: 32, nullable: false),
                    ImageSetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechCardPosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechCardPosts_ImageSet_ImageSetId",
                        column: x => x.ImageSetId,
                        principalTable: "ImageSet",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TechCardPosts_Posts_PostId",
                        column: x => x.PostId,
                        principalTable: "Posts",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TechCardPosts_TechCards_TechCardId",
                        column: x => x.TechCardId,
                        principalTable: "TechCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TechCardLines",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TechCardPostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCustom = table.Column<bool>(type: "bit", nullable: false),
                    ProcessionOrder = table.Column<int>(type: "int", nullable: false),
                    Operation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Device = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Equipment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EquipmentInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ImageSetId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TechCardLines", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TechCardLines_TechCardPosts_TechCardPostId",
                        column: x => x.TechCardPostId,
                        principalTable: "TechCardPosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ImageSet_ImageId",
                table: "ImageSet",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_TechCardLines_TechCardPostId",
                table: "TechCardLines",
                column: "TechCardPostId");

            migrationBuilder.CreateIndex(
                name: "IX_TechCardPosts_ImageSetId",
                table: "TechCardPosts",
                column: "ImageSetId");

            migrationBuilder.CreateIndex(
                name: "IX_TechCardPosts_PostId",
                table: "TechCardPosts",
                column: "PostId");

            migrationBuilder.CreateIndex(
                name: "IX_TechCardPosts_TechCardId",
                table: "TechCardPosts",
                column: "TechCardId");

            migrationBuilder.CreateIndex(
                name: "IX_TechCards_ImageSetId",
                table: "TechCards",
                column: "ImageSetId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TechCardLines");

            migrationBuilder.DropTable(
                name: "TechCardPosts");

            migrationBuilder.DropTable(
                name: "TechCards");

            migrationBuilder.DropTable(
                name: "ImageSet");
        }
    }
}
