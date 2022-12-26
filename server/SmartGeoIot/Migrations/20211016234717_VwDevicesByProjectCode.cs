using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SmartGeoIot.Migrations
{
    public partial class VwDevicesByProjectCode : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            string script = @"SET ANSI_NULLS ON
            GO
            SET QUOTED_IDENTIFIER ON
            GO
            CREATE VIEW [dbo].[VW_DevicesByProjectCode]
            AS
                select NEWID() as Id, p.Code, d.DeviceId from dbo.Projects p
                inner join dbo.DevicesRegistration d on d.ProjectUId = p.ProjectUId
                where p.Code is not null
            GO
            ";
            migrationBuilder.Sql(script);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP VIEW [dbo].[VW_DevicesByProjectCode]");
        }
    }
}
