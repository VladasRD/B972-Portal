using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Box.Security.Data.Migrations
{
    public partial class Log : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Logs",
                columns: table => new
                {
                    LogUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    ActionDescription = table.Column<string>(nullable: true),
                    ErrorDescription = table.Column<string>(nullable: true),
                    LogType = table.Column<short>(nullable: false),
                    Parameters = table.Column<string>(nullable: true),
                    SignedUser = table.Column<string>(maxLength: 255, nullable: true),
                    Url = table.Column<string>(nullable: true),
                    UserIp = table.Column<string>(maxLength: 20, nullable: true),
                    When = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.LogUId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs");
        }
    }
}
