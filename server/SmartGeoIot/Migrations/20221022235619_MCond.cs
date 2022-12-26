using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class MCond : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MConds",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<string>(maxLength: 20, nullable: true),
                    Time = table.Column<long>(nullable: false),
                    PackInf = table.Column<string>(maxLength: 40, nullable: true),
                    InfStateBomb = table.Column<bool>(nullable: false),
                    InfAlarmLevelMin = table.Column<bool>(nullable: false),
                    InfAlarmLevelMax = table.Column<bool>(nullable: false),
                    InfLevel = table.Column<decimal>(nullable: false),
                    PackSup = table.Column<string>(maxLength: 40, nullable: true),
                    SupAlarmLevelMin = table.Column<bool>(nullable: false),
                    SupAlarmLevelMax = table.Column<bool>(nullable: false),
                    SupLevel = table.Column<decimal>(nullable: false),
                    PackPort = table.Column<string>(maxLength: 40, nullable: true),
                    PortFireAlarm = table.Column<bool>(nullable: false),
                    PortIvaAlarm = table.Column<bool>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MConds", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MConds");
        }
    }
}
