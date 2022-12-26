using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class ResetTotalPartial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ResetTotalPartials",
                columns: table => new
                {
                    DeviceId = table.Column<string>(maxLength: 50, nullable: false),
                    LastValue = table.Column<decimal>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    EmailUser = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ResetTotalPartials", x => x.DeviceId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ResetTotalPartials");
        }
    }
}
