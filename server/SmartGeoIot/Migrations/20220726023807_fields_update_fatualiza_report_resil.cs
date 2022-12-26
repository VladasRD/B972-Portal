using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class fields_update_fatualiza_report_resil : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "FAtualizaDia",
                table: "ReportResil",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FAtualizaHora",
                table: "ReportResil",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FAtualizaMes",
                table: "ReportResil",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "FAtualizaSem",
                table: "ReportResil",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FAtualizaDia",
                table: "ReportResil");

            migrationBuilder.DropColumn(
                name: "FAtualizaHora",
                table: "ReportResil");

            migrationBuilder.DropColumn(
                name: "FAtualizaMes",
                table: "ReportResil");

            migrationBuilder.DropColumn(
                name: "FAtualizaSem",
                table: "ReportResil");
        }
    }
}
