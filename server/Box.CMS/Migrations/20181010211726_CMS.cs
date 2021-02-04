using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Box.CMS.Migrations
{
    public partial class CMS : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ContentComments",
                columns: table => new
                {
                    CommentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    ContentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: true),
                    ParentCommentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: true),
                    CommentDate = table.Column<DateTime>(nullable: false),
                    Author = table.Column<string>(maxLength: 50, nullable: true),
                    Comment = table.Column<string>(nullable: true),
                    Status = table.Column<short>(nullable: false),
                    StartRank = table.Column<short>(nullable: false),
                    Position = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentComments", x => x.CommentUId);
                });

            migrationBuilder.CreateTable(
                name: "ContentHeads",
                columns: table => new
                {
                    ContentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    CanonicalName = table.Column<string>(maxLength: 250, nullable: true),
                    Kind = table.Column<string>(maxLength: 25, nullable: true),
                    Abstract = table.Column<string>(maxLength: 300, nullable: true),
                    ThumbFilePath = table.Column<string>(maxLength: 87, nullable: true),
                    Location = table.Column<string>(maxLength: 500, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    ContentDate = table.Column<DateTime>(nullable: false),
                    DisplayOrder = table.Column<short>(nullable: false),
                    PublishUntil = table.Column<DateTime>(nullable: true),
                    PublishAfter = table.Column<DateTime>(nullable: true),
                    Version = table.Column<int>(nullable: false),
                    ExternalLinkUrl = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentHeads", x => x.ContentUId);
                });

            migrationBuilder.CreateTable(
                name: "Files",
                columns: table => new
                {
                    FileUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    FileName = table.Column<string>(maxLength: 255, nullable: true),
                    Size = table.Column<int>(nullable: false),
                    Type = table.Column<string>(maxLength: 100, nullable: true),
                    Folder = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Files", x => x.FileUId);
                });

            migrationBuilder.CreateTable(
                name: "ContentCommentCounts",
                columns: table => new
                {
                    ContentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentCommentCounts", x => x.ContentUId);
                    table.ForeignKey(
                        name: "FK_ContentCommentCounts_ContentHeads_ContentUId",
                        column: x => x.ContentUId,
                        principalTable: "ContentHeads",
                        principalColumn: "ContentUId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentCustomInfos",
                columns: table => new
                {
                    ContentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Text1 = table.Column<string>(maxLength: 2000, nullable: true),
                    Text2 = table.Column<string>(maxLength: 2000, nullable: true),
                    Text3 = table.Column<string>(maxLength: 2000, nullable: true),
                    Text4 = table.Column<string>(maxLength: 2000, nullable: true),
                    Number1 = table.Column<double>(nullable: true),
                    Number2 = table.Column<double>(nullable: true),
                    Number3 = table.Column<double>(nullable: true),
                    Number4 = table.Column<double>(nullable: true),
                    Date1 = table.Column<DateTime>(nullable: true),
                    Date2 = table.Column<DateTime>(nullable: true),
                    Date3 = table.Column<DateTime>(nullable: true),
                    Date4 = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentCustomInfos", x => x.ContentUId);
                    table.ForeignKey(
                        name: "FK_ContentCustomInfos_ContentHeads_ContentUId",
                        column: x => x.ContentUId,
                        principalTable: "ContentHeads",
                        principalColumn: "ContentUId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentDatas",
                columns: table => new
                {
                    ContentUId = table.Column<string>(maxLength: 36, nullable: false),
                    JSON = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentDatas", x => x.ContentUId);
                    table.ForeignKey(
                        name: "FK_ContentDatas_ContentHeads_ContentUId",
                        column: x => x.ContentUId,
                        principalTable: "ContentHeads",
                        principalColumn: "ContentUId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentPageViewCounts",
                columns: table => new
                {
                    ContentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentPageViewCounts", x => x.ContentUId);
                    table.ForeignKey(
                        name: "FK_ContentPageViewCounts_ContentHeads_ContentUId",
                        column: x => x.ContentUId,
                        principalTable: "ContentHeads",
                        principalColumn: "ContentUId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentSharesCounts",
                columns: table => new
                {
                    ContentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Count = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentSharesCounts", x => x.ContentUId);
                    table.ForeignKey(
                        name: "FK_ContentSharesCounts_ContentHeads_ContentUId",
                        column: x => x.ContentUId,
                        principalTable: "ContentHeads",
                        principalColumn: "ContentUId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ContentTags",
                columns: table => new
                {
                    ContentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Tag = table.Column<string>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ContentTags", x => new { x.ContentUId, x.Tag });
                    table.ForeignKey(
                        name: "FK_ContentTags_ContentHeads_ContentUId",
                        column: x => x.ContentUId,
                        principalTable: "ContentHeads",
                        principalColumn: "ContentUId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrossLinks",
                columns: table => new
                {
                    ContentUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    PageArea = table.Column<string>(maxLength: 50, nullable: false),
                    DisplayOrder = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrossLinks", x => new { x.ContentUId, x.PageArea });
                    table.ForeignKey(
                        name: "FK_CrossLinks_ContentHeads_ContentUId",
                        column: x => x.ContentUId,
                        principalTable: "ContentHeads",
                        principalColumn: "ContentUId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FileData",
                columns: table => new
                {
                    FileUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    StoredData = table.Column<byte[]>(nullable: true),
                    StoredThumbData = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FileData", x => x.FileUId);
                    table.ForeignKey(
                        name: "FK_FileData_Files_FileUId",
                        column: x => x.FileUId,
                        principalTable: "Files",
                        principalColumn: "FileUId",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ContentCommentCounts");

            migrationBuilder.DropTable(
                name: "ContentComments");

            migrationBuilder.DropTable(
                name: "ContentCustomInfos");

            migrationBuilder.DropTable(
                name: "ContentDatas");

            migrationBuilder.DropTable(
                name: "ContentPageViewCounts");

            migrationBuilder.DropTable(
                name: "ContentSharesCounts");

            migrationBuilder.DropTable(
                name: "ContentTags");

            migrationBuilder.DropTable(
                name: "CrossLinks");

            migrationBuilder.DropTable(
                name: "FileData");

            migrationBuilder.DropTable(
                name: "ContentHeads");

            migrationBuilder.DropTable(
                name: "Files");
        }
    }
}
