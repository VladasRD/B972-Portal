using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class LocationB980 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "City",
                table: "DevicesLocations",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Neighborhood",
                table: "DevicesLocations",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "State",
                table: "DevicesLocations",
                maxLength: 100,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "City",
                table: "DevicesLocations");

            migrationBuilder.DropColumn(
                name: "Neighborhood",
                table: "DevicesLocations");

            migrationBuilder.DropColumn(
                name: "State",
                table: "DevicesLocations");
        }
    }
}
