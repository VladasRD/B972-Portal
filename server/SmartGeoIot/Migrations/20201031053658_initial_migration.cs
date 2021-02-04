using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class initial_migration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clients",
                columns: table => new
                {
                    ClientUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: true),
                    DocumentType = table.Column<int>(nullable: false),
                    Document = table.Column<string>(maxLength: 14, nullable: true),
                    Address = table.Column<string>(nullable: true),
                    AddressNumber = table.Column<string>(maxLength: 10, nullable: true),
                    City = table.Column<string>(nullable: true),
                    State = table.Column<string>(nullable: true),
                    Neighborhood = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    Email = table.Column<string>(maxLength: 255, nullable: true),
                    Phone = table.Column<string>(maxLength: 14, nullable: true),
                    PostalCode = table.Column<string>(maxLength: 8, nullable: true),
                    StartBilling = table.Column<DateTime>(nullable: true),
                    DueDay = table.Column<int>(nullable: true),
                    Item = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: true),
                    Value = table.Column<decimal>(type: "decimal(38, 2)", nullable: false),
                    Cpf = table.Column<string>(maxLength: 11, nullable: true),
                    Birth = table.Column<DateTime>(nullable: true),
                    EmailNotification = table.Column<bool>(nullable: false),
                    SMSNotification = table.Column<bool>(nullable: false),
                    WhatsAppNotification = table.Column<bool>(nullable: false),
                    PushNotification = table.Column<bool>(nullable: false),
                    ClientFatherUId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clients", x => x.ClientUId);
                });

            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    SequenceNumber = table.Column<int>(nullable: false),
                    LastCom = table.Column<long>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    ComState = table.Column<int>(nullable: false),
                    Pac = table.Column<string>(nullable: true),
                    LocationLat = table.Column<string>(nullable: true),
                    LocationLng = table.Column<string>(nullable: true),
                    DeviceTypeId = table.Column<string>(nullable: true),
                    GroupId = table.Column<string>(nullable: true),
                    Lqi = table.Column<int>(nullable: false),
                    ActivationTime = table.Column<long>(nullable: false),
                    TokenState = table.Column<int>(nullable: false),
                    TokenDetailMessage = table.Column<string>(nullable: true),
                    TokenEnd = table.Column<long>(nullable: false),
                    ContractId = table.Column<string>(nullable: true),
                    CreationTime = table.Column<long>(nullable: false),
                    ModemCertificateId = table.Column<string>(nullable: true),
                    Prototype = table.Column<bool>(nullable: false),
                    AutomaticRenewal = table.Column<bool>(nullable: false),
                    AutomaticRenewalStatus = table.Column<int>(nullable: false),
                    CreatedBy = table.Column<string>(nullable: true),
                    LastEditionTime = table.Column<long>(nullable: false),
                    LastEditedBy = table.Column<string>(nullable: true),
                    Activable = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DevicesLocations",
                columns: table => new
                {
                    DeviceLocationUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    DeviceId = table.Column<string>(nullable: true),
                    Time = table.Column<long>(nullable: false),
                    Data = table.Column<string>(maxLength: 50, nullable: true),
                    Radius = table.Column<string>(maxLength: 50, nullable: true),
                    Latitude = table.Column<double>(maxLength: 100, nullable: false),
                    Longitude = table.Column<double>(maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicesLocations", x => x.DeviceLocationUId);
                });

            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    PackageUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Type = table.Column<string>(nullable: true),
                    Byte = table.Column<int>(nullable: false),
                    Bit = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.PackageUId);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectUId);
                });

            migrationBuilder.CreateTable(
                name: "ClientsBillings",
                columns: table => new
                {
                    ClientBillingUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    ClientUId = table.Column<string>(nullable: true),
                    Create = table.Column<DateTime>(nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: true),
                    PaymentDueDate = table.Column<DateTime>(nullable: true),
                    Sended = table.Column<bool>(nullable: false),
                    ExternalId = table.Column<int>(nullable: false),
                    BarCode = table.Column<string>(nullable: true),
                    LinkPdf = table.Column<string>(nullable: true),
                    Status = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientsBillings", x => x.ClientBillingUId);
                    table.ForeignKey(
                        name: "FK_ClientsBillings_Clients_ClientUId",
                        column: x => x.ClientUId,
                        principalTable: "Clients",
                        principalColumn: "ClientUId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientsUsers",
                columns: table => new
                {
                    ClientUserUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    ClientUId = table.Column<string>(nullable: true),
                    ApplicationUserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientsUsers", x => x.ClientUserUId);
                    table.ForeignKey(
                        name: "FK_ClientsUsers_Clients_ClientUId",
                        column: x => x.ClientUId,
                        principalTable: "Clients",
                        principalColumn: "ClientUId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientsDevices",
                columns: table => new
                {
                    ClientDeviceUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    ClientUId = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: true),
                    Active = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientsDevices", x => x.ClientDeviceUId);
                    table.ForeignKey(
                        name: "FK_ClientsDevices_Clients_ClientUId",
                        column: x => x.ClientUId,
                        principalTable: "Clients",
                        principalColumn: "ClientUId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientsDevices_Devices_Id",
                        column: x => x.Id,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    DeviceId = table.Column<string>(nullable: true),
                    Time = table.Column<long>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    RolloverCounter = table.Column<int>(nullable: false),
                    SeqNumber = table.Column<int>(nullable: false),
                    NbFrames = table.Column<int>(nullable: false),
                    Operator = table.Column<string>(nullable: true),
                    Country = table.Column<string>(nullable: true),
                    Lqi = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Messages_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DevicesRegistration",
                columns: table => new
                {
                    DeviceCustomUId = table.Column<string>(type: "char(36)", maxLength: 36, nullable: false),
                    Name = table.Column<string>(maxLength: 250, nullable: true),
                    NickName = table.Column<string>(maxLength: 100, nullable: true),
                    DeviceId = table.Column<string>(nullable: true),
                    PackageUId = table.Column<string>(nullable: true),
                    ProjectUId = table.Column<string>(nullable: true),
                    DataDownloadLink = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DevicesRegistration", x => x.DeviceCustomUId);
                    table.ForeignKey(
                        name: "FK_DevicesRegistration_Devices_DeviceId",
                        column: x => x.DeviceId,
                        principalTable: "Devices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevicesRegistration_Packages_PackageUId",
                        column: x => x.PackageUId,
                        principalTable: "Packages",
                        principalColumn: "PackageUId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_DevicesRegistration_Projects_ProjectUId",
                        column: x => x.ProjectUId,
                        principalTable: "Projects",
                        principalColumn: "ProjectUId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClientsBillings_ClientUId",
                table: "ClientsBillings",
                column: "ClientUId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientsDevices_ClientUId",
                table: "ClientsDevices",
                column: "ClientUId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientsDevices_Id",
                table: "ClientsDevices",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ClientsUsers_ClientUId",
                table: "ClientsUsers",
                column: "ClientUId");

            migrationBuilder.CreateIndex(
                name: "IX_DevicesRegistration_DeviceId",
                table: "DevicesRegistration",
                column: "DeviceId");

            migrationBuilder.CreateIndex(
                name: "IX_DevicesRegistration_PackageUId",
                table: "DevicesRegistration",
                column: "PackageUId");

            migrationBuilder.CreateIndex(
                name: "IX_DevicesRegistration_ProjectUId",
                table: "DevicesRegistration",
                column: "ProjectUId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_DeviceId",
                table: "Messages",
                column: "DeviceId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientsBillings");

            migrationBuilder.DropTable(
                name: "ClientsDevices");

            migrationBuilder.DropTable(
                name: "ClientsUsers");

            migrationBuilder.DropTable(
                name: "DevicesLocations");

            migrationBuilder.DropTable(
                name: "DevicesRegistration");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Clients");

            migrationBuilder.DropTable(
                name: "Packages");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
