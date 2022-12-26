using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class AlterB972_Add_Pack86_Pack87 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Pack86Time",
                table: "B972s",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "Pack87Time",
                table: "B972s",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Pack86Time",
                table: "B972s");

            migrationBuilder.DropColumn(
                name: "Pack87Time",
                table: "B972s");
        }
    }
}
