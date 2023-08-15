using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class ServiceDesk : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServiceDesks",
                columns: table => new
                {
                    ServiceDeskId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<string>(maxLength: 20, nullable: true),
                    CreateDate = table.Column<DateTime>(nullable: false),
                    FinishDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDesks", x => x.ServiceDeskId);
                });

            migrationBuilder.CreateTable(
                name: "ServiceDeskRecords",
                columns: table => new
                {
                    ServiceDeskRecordId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ServiceDeskId = table.Column<int>(nullable: false),
                    Package = table.Column<string>(maxLength: 100, nullable: true),
                    PackageTimestamp = table.Column<long>(nullable: false),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceDeskRecords", x => x.ServiceDeskRecordId);
                    table.ForeignKey(
                        name: "FK_ServiceDeskRecords_ServiceDesks_ServiceDeskId",
                        column: x => x.ServiceDeskId,
                        principalTable: "ServiceDesks",
                        principalColumn: "ServiceDeskId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceDeskRecords_ServiceDeskId",
                table: "ServiceDeskRecords",
                column: "ServiceDeskId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceDeskRecords");

            migrationBuilder.DropTable(
                name: "ServiceDesks");
        }
    }
}
