using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class B975_timeForPack : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Time",
                table: "B975s",
                newName: "TimeC");

            migrationBuilder.AddColumn<long>(
                name: "TimeA",
                table: "B975s",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "TimeB",
                table: "B975s",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TimeA",
                table: "B975s");

            migrationBuilder.DropColumn(
                name: "TimeB",
                table: "B975s");

            migrationBuilder.RenameColumn(
                name: "TimeC",
                table: "B975s",
                newName: "Time");
        }
    }
}
