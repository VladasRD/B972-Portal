using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class ServiceDeskUpdateV1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PackageTimestamp",
                table: "ServiceDeskRecords",
                nullable: true,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "PackageTimestamp",
                table: "ServiceDeskRecords",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }
    }
}
