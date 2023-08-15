using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class B975_locationCity : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LocationCity",
                table: "B975s",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LocationCity",
                table: "B975s");
        }
    }
}
