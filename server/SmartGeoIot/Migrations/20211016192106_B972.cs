using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class B972 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "B972s",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<string>(maxLength: 50, nullable: true),
                    Time = table.Column<long>(nullable: false),
                    Flow = table.Column<decimal>(nullable: true),
                    Velocity = table.Column<decimal>(nullable: true),
                    Total = table.Column<decimal>(nullable: true),
                    Partial = table.Column<decimal>(nullable: true),
                    Temperature = table.Column<decimal>(nullable: true),
                    Flags = table.Column<string>(maxLength: 8, nullable: true),
                    Quality = table.Column<decimal>(nullable: true),
                    RSSI = table.Column<string>(maxLength: 100, nullable: true),
                    Source = table.Column<string>(maxLength: 100, nullable: true),
                    Lqi = table.Column<int>(nullable: false),
                    Iq = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B972s", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "B972s");
        }
    }
}
