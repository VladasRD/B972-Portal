using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class update_mconds : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "InfStateBomb",
                table: "MConds",
                newName: "SupStateBomb");

            migrationBuilder.AlterColumn<double>(
                name: "SupLevel",
                table: "MConds",
                nullable: false,
                oldClrType: typeof(decimal));

            migrationBuilder.AlterColumn<double>(
                name: "InfLevel",
                table: "MConds",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SupStateBomb",
                table: "MConds",
                newName: "InfStateBomb");

            migrationBuilder.AlterColumn<decimal>(
                name: "SupLevel",
                table: "MConds",
                nullable: false,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<decimal>(
                name: "InfLevel",
                table: "MConds",
                nullable: false,
                oldClrType: typeof(double));
        }
    }
}
