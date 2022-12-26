using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class ReportResil : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ReportResil",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DeviceId = table.Column<string>(nullable: true),
                    Time = table.Column<long>(nullable: false),
                    Day = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    Year = table.Column<int>(nullable: false),
                    Hour = table.Column<int>(nullable: false),
                    Minute = table.Column<int>(nullable: false),
                    ConsumoHora = table.Column<decimal>(nullable: true),
                    ConsumoDia = table.Column<decimal>(nullable: true),
                    ConsumoSemana = table.Column<decimal>(nullable: true),
                    ConsumoMes = table.Column<decimal>(nullable: true),
                    Fluxo = table.Column<decimal>(nullable: true),
                    Modo = table.Column<string>(nullable: true),
                    Estado = table.Column<string>(nullable: true),
                    Valvula = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReportResil", x => x.Id);
                });

            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReportResil");
        }
    }
}
