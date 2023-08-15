using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class ClientApiKey : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ApiKey",
                table: "Clients",
                maxLength: 36,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApiKey",
                table: "Clients");
        }
    }
}
