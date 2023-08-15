﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SmartGeoIot.Data;

namespace SmartGeoIot.Migrations
{
    [DbContext(typeof(SmartGeoIotContext))]
    [Migration("20230426002748_B979")]
    partial class B979
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.8-servicing-32085")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("SmartGeoIot.Models.B972", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("DeviceId")
                        .HasMaxLength(20);

                    b.Property<string>("Flags")
                        .HasMaxLength(8);

                    b.Property<decimal?>("Flow");

                    b.Property<int?>("Iq");

                    b.Property<int>("Lqi");

                    b.Property<long?>("Pack86Time");

                    b.Property<long?>("Pack87Time");

                    b.Property<decimal?>("Partial");

                    b.Property<int>("Position");

                    b.Property<decimal?>("Quality");

                    b.Property<string>("RSSI")
                        .HasMaxLength(100);

                    b.Property<string>("Source")
                        .HasMaxLength(100);

                    b.Property<decimal?>("Temperature");

                    b.Property<long>("Time");

                    b.Property<decimal?>("Total");

                    b.Property<decimal?>("Velocity");

                    b.Property<long?>("VelocityTime");

                    b.HasKey("Id");

                    b.ToTable("B972s");
                });

            modelBuilder.Entity("SmartGeoIot.Models.B979", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<decimal?>("Acel");

                    b.Property<decimal?>("Ciclos");

                    b.Property<DateTime>("Data");

                    b.Property<decimal?>("Desacel");

                    b.Property<string>("DeviceId")
                        .HasMaxLength(20);

                    b.Property<decimal?>("EncoderPMA");

                    b.Property<decimal?>("EncoderPMF");

                    b.Property<int?>("Estado");

                    b.Property<decimal?>("Horimetro");

                    b.Property<decimal?>("Inversor");

                    b.Property<decimal?>("TOVelBaixa");

                    b.Property<decimal?>("TempoPMA");

                    b.Property<decimal?>("TempoPMF");

                    b.Property<decimal?>("Timer");

                    b.Property<decimal?>("TimerFreioOff");

                    b.Property<decimal?>("TimerFreioOn");

                    b.Property<decimal?>("TimerP2");

                    b.Property<decimal?>("VelAltaAbrir");

                    b.Property<decimal?>("VelAltaFechar");

                    b.Property<decimal?>("VelBaixa");

                    b.HasKey("Id");

                    b.ToTable("B979s");
                });

            modelBuilder.Entity("SmartGeoIot.Models.B982_S", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<string>("Calha")
                        .HasMaxLength(100);

                    b.Property<string>("CalhaAlerta")
                        .HasMaxLength(200);

                    b.Property<DateTime>("Date");

                    b.Property<string>("DeviceId")
                        .HasMaxLength(20);

                    b.Property<decimal>("Flow");

                    b.Property<int?>("Iq");

                    b.Property<int>("Lqi");

                    b.Property<string>("OriginPack")
                        .HasMaxLength(30);

                    b.Property<decimal>("Partial");

                    b.Property<string>("RSSI")
                        .HasMaxLength(100);

                    b.Property<string>("Source")
                        .HasMaxLength(100);

                    b.Property<long>("Time");

                    b.Property<decimal>("Total");

                    b.HasKey("Id");

                    b.ToTable("B982_S");
                });

            modelBuilder.Entity("SmartGeoIot.Models.Client", b =>
                {
                    b.Property<string>("ClientUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<bool>("Active");

                    b.Property<string>("Address");

                    b.Property<string>("AddressNumber")
                        .HasMaxLength(10);

                    b.Property<string>("ApiKey")
                        .HasMaxLength(36);

                    b.Property<DateTime?>("Birth");

                    b.Property<string>("City");

                    b.Property<string>("ClientFatherUId");

                    b.Property<string>("Cpf")
                        .HasMaxLength(11);

                    b.Property<DateTime>("Created")
                        .ValueGeneratedOnAdd()
                        .HasDefaultValueSql("getdate()");

                    b.Property<string>("Document")
                        .HasMaxLength(14);

                    b.Property<int>("DocumentType");

                    b.Property<int?>("DueDay");

                    b.Property<string>("Email")
                        .HasMaxLength(255);

                    b.Property<bool>("EmailNotification");

                    b.Property<string>("Item");

                    b.Property<string>("Name")
                        .HasMaxLength(500);

                    b.Property<string>("Neighborhood");

                    b.Property<string>("Phone")
                        .HasMaxLength(14);

                    b.Property<string>("PostalCode")
                        .HasMaxLength(8);

                    b.Property<bool>("PushNotification");

                    b.Property<bool>("SMSNotification");

                    b.Property<DateTime?>("StartBilling");

                    b.Property<string>("State");

                    b.Property<int?>("Type");

                    b.Property<decimal?>("Value")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<bool>("WhatsAppNotification");

                    b.HasKey("ClientUId");

                    b.ToTable("Clients");
                });

            modelBuilder.Entity("SmartGeoIot.Models.ClientBilling", b =>
                {
                    b.Property<string>("ClientBillingUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("BarCode");

                    b.Property<string>("ClientUId");

                    b.Property<DateTime>("Create");

                    b.Property<int>("ExternalId");

                    b.Property<string>("LinkPdf");

                    b.Property<DateTime?>("PaymentDate");

                    b.Property<DateTime?>("PaymentDueDate");

                    b.Property<bool>("Sended");

                    b.Property<string>("Status");

                    b.HasKey("ClientBillingUId");

                    b.HasIndex("ClientUId");

                    b.ToTable("ClientsBillings");
                });

            modelBuilder.Entity("SmartGeoIot.Models.ClientDevice", b =>
                {
                    b.Property<string>("ClientDeviceUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<bool>("Active");

                    b.Property<string>("ClientUId");

                    b.Property<string>("Id");

                    b.HasKey("ClientDeviceUId");

                    b.HasIndex("ClientUId");

                    b.HasIndex("Id");

                    b.ToTable("ClientsDevices");
                });

            modelBuilder.Entity("SmartGeoIot.Models.ClientUser", b =>
                {
                    b.Property<string>("ClientUserUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("ApplicationUserId");

                    b.Property<string>("ClientUId");

                    b.HasKey("ClientUserUId");

                    b.HasIndex("ClientUId");

                    b.ToTable("ClientsUsers");
                });

            modelBuilder.Entity("SmartGeoIot.Models.Device", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<bool>("Activable");

                    b.Property<long>("ActivationTime");

                    b.Property<bool>("AutomaticRenewal");

                    b.Property<int>("AutomaticRenewalStatus");

                    b.Property<int>("ComState");

                    b.Property<string>("ContractId");

                    b.Property<string>("CreatedBy");

                    b.Property<long>("CreationTime");

                    b.Property<string>("DeviceTypeId");

                    b.Property<string>("GroupId");

                    b.Property<long>("LastCom");

                    b.Property<string>("LastEditedBy");

                    b.Property<long>("LastEditionTime");

                    b.Property<string>("LocationLat");

                    b.Property<string>("LocationLng");

                    b.Property<int>("Lqi");

                    b.Property<string>("ModemCertificateId");

                    b.Property<string>("Name");

                    b.Property<string>("Pac");

                    b.Property<bool>("Prototype");

                    b.Property<int>("SequenceNumber");

                    b.Property<int>("State");

                    b.Property<string>("TokenDetailMessage");

                    b.Property<long>("TokenEnd");

                    b.Property<int>("TokenState");

                    b.HasKey("Id");

                    b.ToTable("Devices");
                });

            modelBuilder.Entity("SmartGeoIot.Models.DeviceLocation", b =>
                {
                    b.Property<string>("DeviceLocationUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("Data")
                        .HasMaxLength(50);

                    b.Property<string>("DeviceId");

                    b.Property<double>("Latitude")
                        .HasMaxLength(100);

                    b.Property<double>("Longitude")
                        .HasMaxLength(100);

                    b.Property<string>("Radius")
                        .HasMaxLength(50);

                    b.Property<long>("Time");

                    b.HasKey("DeviceLocationUId");

                    b.ToTable("DevicesLocations");
                });

            modelBuilder.Entity("SmartGeoIot.Models.DeviceRegistration", b =>
                {
                    b.Property<string>("DeviceCustomUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("DataDownloadLink")
                        .HasMaxLength(50);

                    b.Property<string>("DeviceId");

                    b.Property<string>("Ea10");

                    b.Property<string>("Ed1");

                    b.Property<string>("Ed2");

                    b.Property<string>("Ed3");

                    b.Property<string>("Ed4");

                    b.Property<string>("Model")
                        .HasMaxLength(50);

                    b.Property<string>("Name")
                        .HasMaxLength(250);

                    b.Property<string>("NickName")
                        .HasMaxLength(100);

                    b.Property<string>("Notes");

                    b.Property<DateTime?>("NotesCreateDate");

                    b.Property<string>("PackageUId");

                    b.Property<string>("ProjectUId");

                    b.Property<string>("Sa3");

                    b.Property<string>("Sd1");

                    b.Property<string>("Sd2");

                    b.Property<string>("SerialNumber")
                        .HasMaxLength(80);

                    b.HasKey("DeviceCustomUId");

                    b.HasIndex("DeviceId");

                    b.HasIndex("PackageUId");

                    b.HasIndex("ProjectUId");

                    b.ToTable("DevicesRegistration");
                });

            modelBuilder.Entity("SmartGeoIot.Models.MCond", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<DateTime>("Date");

                    b.Property<string>("DeviceId")
                        .HasMaxLength(20);

                    b.Property<bool>("InfAlarmLevelMax");

                    b.Property<bool>("InfAlarmLevelMin");

                    b.Property<double>("InfLevel");

                    b.Property<string>("PackInf")
                        .HasMaxLength(40);

                    b.Property<string>("PackPort")
                        .HasMaxLength(40);

                    b.Property<string>("PackSup")
                        .HasMaxLength(40);

                    b.Property<bool>("PortFireAlarm");

                    b.Property<bool>("PortFireState");

                    b.Property<bool>("PortIvaAlarm");

                    b.Property<bool>("PortIvaState");

                    b.Property<bool>("SupAlarmLevelMax");

                    b.Property<bool>("SupAlarmLevelMin");

                    b.Property<double>("SupLevel");

                    b.Property<bool>("SupStateBomb");

                    b.Property<long>("Time");

                    b.HasKey("Id");

                    b.ToTable("MConds");
                });

            modelBuilder.Entity("SmartGeoIot.Models.Message", b =>
                {
                    b.Property<string>("Id");

                    b.Property<string>("DeviceId");

                    b.Property<string>("Country");

                    b.Property<string>("Data");

                    b.Property<int>("Lqi");

                    b.Property<int>("NbFrames");

                    b.Property<DateTime?>("OperationDate");

                    b.Property<string>("Operator");

                    b.Property<int>("RolloverCounter");

                    b.Property<int>("SeqNumber");

                    b.Property<long>("Time");

                    b.Property<bool>("WasProcessed");

                    b.HasKey("Id", "DeviceId");

                    b.HasIndex("DeviceId");

                    b.ToTable("Messages");
                });

            modelBuilder.Entity("SmartGeoIot.Models.Outgoing", b =>
                {
                    b.Property<string>("OutgoingUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<int>("ClientsActive");

                    b.Property<decimal>("DataCenterValue")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<string>("Description");

                    b.Property<decimal>("DevelopmentValue")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<int>("LicensesActive");

                    b.Property<int>("Month");

                    b.Property<decimal>("OperationValue")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("OperationsWNDValue")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<int>("Year");

                    b.HasKey("OutgoingUId");

                    b.ToTable("Outgoings");
                });

            modelBuilder.Entity("SmartGeoIot.Models.Package", b =>
                {
                    b.Property<string>("PackageUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<int>("Bit");

                    b.Property<int>("Byte");

                    b.Property<string>("Description")
                        .HasMaxLength(200);

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("Type")
                        .HasMaxLength(50);

                    b.HasKey("PackageUId");

                    b.ToTable("Packages");
                });

            modelBuilder.Entity("SmartGeoIot.Models.Project", b =>
                {
                    b.Property<string>("ProjectUId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("char(36)")
                        .HasMaxLength(36);

                    b.Property<string>("Code")
                        .HasMaxLength(50);

                    b.Property<string>("Description")
                        .HasMaxLength(400);

                    b.Property<string>("Name")
                        .HasMaxLength(200);

                    b.HasKey("ProjectUId");

                    b.ToTable("Projects");
                });

            modelBuilder.Entity("SmartGeoIot.Models.ProjectDevice", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code")
                        .HasMaxLength(50);

                    b.Property<string>("DeviceId");

                    b.HasKey("Id");

                    b.ToTable("VW_DevicesByProjectCode");
                });

            modelBuilder.Entity("SmartGeoIot.Models.ReportResil", b =>
                {
                    b.Property<string>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal?>("ConsumoDia");

                    b.Property<decimal?>("ConsumoHora");

                    b.Property<decimal?>("ConsumoMes");

                    b.Property<decimal?>("ConsumoSemana");

                    b.Property<DateTime>("Date");

                    b.Property<int>("Day");

                    b.Property<string>("DeviceId");

                    b.Property<string>("Estado");

                    b.Property<bool>("FAtualizaDia");

                    b.Property<bool>("FAtualizaHora");

                    b.Property<bool>("FAtualizaMes");

                    b.Property<bool>("FAtualizaSem");

                    b.Property<decimal?>("Fluxo");

                    b.Property<int>("Hour");

                    b.Property<int>("Minute");

                    b.Property<string>("Modo");

                    b.Property<int>("Month");

                    b.Property<long>("Time");

                    b.Property<string>("Valvula");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("ReportResil");
                });

            modelBuilder.Entity("SmartGeoIot.Models.ResetTotalPartial", b =>
                {
                    b.Property<string>("DeviceId")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(50);

                    b.Property<DateTime>("Date");

                    b.Property<string>("EmailUser")
                        .HasMaxLength(100);

                    b.Property<decimal>("LastValue");

                    b.HasKey("DeviceId");

                    b.ToTable("ResetTotalPartials");
                });

            modelBuilder.Entity("SmartGeoIot.Models.VW_Outgoing", b =>
                {
                    b.Property<string>("OutgoingUId")
                        .ValueGeneratedOnAdd();

                    b.Property<decimal>("AverageForClient")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("AverageForLicenseClient")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("AverageLicensesActived")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<int>("ClientsActive");

                    b.Property<decimal>("DataCenterValue")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("DataCenterValueYear")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<string>("Description");

                    b.Property<decimal>("DevelopmentValue")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("DevelopmentValueYear")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<int>("LicensesActive");

                    b.Property<int>("LicensesActiveYear");

                    b.Property<int>("Month");

                    b.Property<decimal>("OperationValue")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("OperationValueYear")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("OperationsWNDValue")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("OperationsWNDValueYear")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("TotalBilling")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<decimal>("TotalBillingYear")
                        .HasColumnType("decimal(38, 2)");

                    b.Property<int>("Year");

                    b.HasKey("OutgoingUId");

                    b.ToTable("VW_Outgoings");
                });

            modelBuilder.Entity("SmartGeoIot.Models.ClientBilling", b =>
                {
                    b.HasOne("SmartGeoIot.Models.Client")
                        .WithMany("Billings")
                        .HasForeignKey("ClientUId");
                });

            modelBuilder.Entity("SmartGeoIot.Models.ClientDevice", b =>
                {
                    b.HasOne("SmartGeoIot.Models.Client")
                        .WithMany("Devices")
                        .HasForeignKey("ClientUId");

                    b.HasOne("SmartGeoIot.Models.Device", "AppDevice")
                        .WithMany()
                        .HasForeignKey("Id");
                });

            modelBuilder.Entity("SmartGeoIot.Models.ClientUser", b =>
                {
                    b.HasOne("SmartGeoIot.Models.Client")
                        .WithMany("Users")
                        .HasForeignKey("ClientUId");
                });

            modelBuilder.Entity("SmartGeoIot.Models.DeviceRegistration", b =>
                {
                    b.HasOne("SmartGeoIot.Models.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId");

                    b.HasOne("SmartGeoIot.Models.Package", "Package")
                        .WithMany()
                        .HasForeignKey("PackageUId");

                    b.HasOne("SmartGeoIot.Models.Project", "Project")
                        .WithMany()
                        .HasForeignKey("ProjectUId");
                });

            modelBuilder.Entity("SmartGeoIot.Models.Message", b =>
                {
                    b.HasOne("SmartGeoIot.Models.Device", "Device")
                        .WithMany()
                        .HasForeignKey("DeviceId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
