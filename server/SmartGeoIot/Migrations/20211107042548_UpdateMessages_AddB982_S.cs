using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class UpdateMessages_AddB982_S : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "WasProcessed",
                table: "Messages",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "B972s",
                maxLength: 20,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "B982_S",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<string>(maxLength: 20, nullable: true),
                    Time = table.Column<long>(nullable: false),
                    OriginPack = table.Column<string>(maxLength: 30, nullable: true),
                    Flow = table.Column<decimal>(nullable: false),
                    Total = table.Column<decimal>(nullable: false),
                    Partial = table.Column<decimal>(nullable: false),
                    Calha = table.Column<string>(maxLength: 100, nullable: true),
                    CalhaAlerta = table.Column<string>(maxLength: 200, nullable: true),
                    RSSI = table.Column<string>(maxLength: 100, nullable: true),
                    Source = table.Column<string>(maxLength: 100, nullable: true),
                    Lqi = table.Column<int>(nullable: false),
                    Iq = table.Column<int>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B982_S", x => x.Id);
                });

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "B982_S");

            migrationBuilder.DropColumn(
                name: "WasProcessed",
                table: "Messages");

            migrationBuilder.AlterColumn<string>(
                name: "DeviceId",
                table: "B972s",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 20,
                oldNullable: true);
        }
    }
}
