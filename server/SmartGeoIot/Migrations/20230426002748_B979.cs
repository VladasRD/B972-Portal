using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class B979 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "B979s",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DeviceId = table.Column<string>(maxLength: 20, nullable: true),
                    Data = table.Column<DateTime>(nullable: false),
                    Acel = table.Column<decimal>(nullable: true),
                    Desacel = table.Column<decimal>(nullable: true),
                    EncoderPMA = table.Column<decimal>(nullable: true),
                    EncoderPMF = table.Column<decimal>(nullable: true),
                    TimerFreioOn = table.Column<decimal>(nullable: true),
                    TimerFreioOff = table.Column<decimal>(nullable: true),
                    Timer = table.Column<decimal>(nullable: true),
                    TimerP2 = table.Column<decimal>(nullable: true),
                    TOVelBaixa = table.Column<decimal>(nullable: true),
                    TempoPMA = table.Column<decimal>(nullable: true),
                    TempoPMF = table.Column<decimal>(nullable: true),
                    VelBaixa = table.Column<decimal>(nullable: true),
                    VelAltaAbrir = table.Column<decimal>(nullable: true),
                    VelAltaFechar = table.Column<decimal>(nullable: true),
                    Ciclos = table.Column<decimal>(nullable: true),
                    Horimetro = table.Column<decimal>(nullable: true),
                    Inversor = table.Column<decimal>(nullable: true),
                    Estado = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_B979s", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "B979s");
        }
    }
}
