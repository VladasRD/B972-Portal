using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class Add_Outgoings_Alter_Clients : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Packages",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Packages",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Packages",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Clients",
                type: "decimal(38, 2)",
                nullable: true,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 2)");

            migrationBuilder.CreateTable(
                name: "Outgoings",
                columns: table => new
                {
                    OutgoingUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Year = table.Column<int>(nullable: false),
                    Month = table.Column<int>(nullable: false),
                    LicensesActive = table.Column<int>(nullable: false),
                    ClientsActive = table.Column<int>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DevelopmentValue = table.Column<decimal>(type: "decimal(38, 2)", nullable: false),
                    OperationsWNDValue = table.Column<decimal>(type: "decimal(38, 2)", nullable: false),
                    DataCenterValue = table.Column<decimal>(type: "decimal(38, 2)", nullable: false),
                    OperationValue = table.Column<decimal>(type: "decimal(38, 2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Outgoings", x => x.OutgoingUId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Outgoings");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Projects",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Packages",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Packages",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Packages",
                nullable: true,
                oldClrType: typeof(string),
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<decimal>(
                name: "Value",
                table: "Clients",
                type: "decimal(38, 2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(38, 2)",
                oldNullable: true);
        }
    }
}
