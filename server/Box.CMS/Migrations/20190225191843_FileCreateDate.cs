using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Box.CMS.Migrations
{
    public partial class FileCreateDate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "_CreateDate",
                table: "Files",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "_CreateDate",
                table: "Files");
        }
    }
}
