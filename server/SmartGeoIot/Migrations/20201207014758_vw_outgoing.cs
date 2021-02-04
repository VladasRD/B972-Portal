using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class vw_outgoing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "Created",
                table: "Clients",
                nullable: false,
                defaultValueSql: "getdate()");

            string script =
            @"
                alter VIEW dbo.VW_Outgoings
                AS

                SELECT otg.*
                ,isnull(tb.TotalBilling, 0) as TotalBilling
                ,CONVERT(decimal(38, 2), tl.AverageLicensesActived / CONVERT(decimal(4,2), otg.Month)) as AverageLicensesActived
                ,isnull(CONVERT(decimal(38, 2), isnull(tby.TotalBillingYear, 0) / CONVERT(decimal(4,2), tc.TotalClients)), 0) as AverageForClient
                ,isnull(CONVERT(decimal(38, 2), isnull(tby.TotalBillingYear, 0) / CONVERT(decimal(4,2), tcd.TotalDevices)), 0) as AverageForLicenseClient
                FROM [dbo].[Outgoings] otg

                left join (
                    select Sum(Value) as TotalBilling, MONTH(cb.PaymentDate) as Month, YEAR(cb.PaymentDate) as Year from dbo.Clients c
                    inner join dbo.ClientsBillings cb on cb.ClientUId = c.ClientUId
                                                and c.Active = 1
                                                group by cb.PaymentDate
                ) tb on tb.Month = otg.[Month] and tb.Year = otg.[Year]

                left join (
                    select Sum(Value) as TotalBillingYear, YEAR(cb.PaymentDate) as Year from dbo.Clients c
                    inner join dbo.ClientsBillings cb on cb.ClientUId = c.ClientUId
                                                and c.Active = 1
                                                group by cb.PaymentDate
                ) tby on tby.Year = otg.[Year]

                left join (
                    select sum(LicensesActive) as AverageLicensesActived, [Month], [Year] FROM [dbo].[Outgoings]
                    group by [Month], [Year]
                ) tl on tl.Month = otg.[Month] and tl.Year = otg.[Year]

                left join (
                    select count(*) TotalClients, Created from dbo.Clients where Active = 1 group by Created
                ) tc on YEAR(tc.Created) = otg.[Year] and MONTH(tc.Created) <= otg.[Month]

                left join (
                    select count(*) TotalDevices, c.Created from dbo.Clients c
                    inner join dbo.ClientsDevices cd on cd.ClientUId = c.ClientUId
                    where c.Active = 1 and cd.Active = 1
                    group by c.Created
                ) tcd on YEAR(tcd.Created) = otg.[Year] and MONTH(tcd.Created) <= otg.[Month]
            ";
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW dbo.VW_Outgoings");

            migrationBuilder.DropColumn(
                name: "Created",
                table: "Clients");
        }
    }
}
