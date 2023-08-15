using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class B975 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "B975s",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<string>(maxLength: 20, nullable: true),
                    Time = table.Column<long>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    PackA = table.Column<string>(maxLength: 100, nullable: true),
                    PcPosChave = table.Column<bool>(nullable: false),
                    Jam = table.Column<bool>(nullable: false),
                    Vio = table.Column<bool>(nullable: false),
                    RasIn = table.Column<bool>(nullable: false),
                    Bloqueio = table.Column<bool>(nullable: false),
                    RasOut = table.Column<bool>(nullable: false),
                    StatusDJ = table.Column<string>(nullable: true),
                    AlertaFonteBaixa = table.Column<bool>(nullable: false),
                    IntervaloUpLink = table.Column<decimal>(nullable: false),
                    ContadorCarencias = table.Column<int>(nullable: false),
                    ContadorBloqueios = table.Column<int>(nullable: false),
                    PackB = table.Column<string>(maxLength: 100, nullable: true),
                    TemperaturaInterna = table.Column<decimal>(nullable: false),
                    TensaoAlimentacao = table.Column<decimal>(nullable: false),
                    PackC = table.Column<string>(maxLength: 100, nullable: true),
                    MediaRFMinimo = table.Column<decimal>(nullable: false),
                    MediaRFMaximo = table.Column<decimal>(nullable: false),
                    MediaLinhaBase = table.Column<decimal>(nullable: false),
                    MediaInterferencia = table.Column<decimal>(nullable: false),
                    DeteccaoInterferencia = table.Column<decimal>(nullable: false),
                    DeteccaoJammer = table.Column<decimal>(nullable: false),
                    NumeroViolacao = table.Column<decimal>(nullable: false),
                    Source = table.Column<string>(maxLength: 100, nullable: true),
                    Radius = table.Column<string>(maxLength: 50, nullable: true),
                    Latitude = table.Column<double>(maxLength: 100, nullable: false),
                    Longitude = table.Column<double>(maxLength: 100, nullable: false),
                    Lqi = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B975s", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "B975s");
        }
    }
}
