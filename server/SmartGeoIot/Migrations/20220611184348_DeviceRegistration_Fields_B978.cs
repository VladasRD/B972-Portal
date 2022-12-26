using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class DeviceRegistration_Fields_B978 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ea10",
                table: "DevicesRegistration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ed1",
                table: "DevicesRegistration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ed2",
                table: "DevicesRegistration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ed3",
                table: "DevicesRegistration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Ed4",
                table: "DevicesRegistration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sd1",
                table: "DevicesRegistration",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Sd2",
                table: "DevicesRegistration",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ea10",
                table: "DevicesRegistration");

            migrationBuilder.DropColumn(
                name: "Ed1",
                table: "DevicesRegistration");

            migrationBuilder.DropColumn(
                name: "Ed2",
                table: "DevicesRegistration");

            migrationBuilder.DropColumn(
                name: "Ed3",
                table: "DevicesRegistration");

            migrationBuilder.DropColumn(
                name: "Ed4",
                table: "DevicesRegistration");

            migrationBuilder.DropColumn(
                name: "Sd1",
                table: "DevicesRegistration");

            migrationBuilder.DropColumn(
                name: "Sd2",
                table: "DevicesRegistration");
        }
    }
}
