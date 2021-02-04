alter VIEW dbo.VW_Outgoings
AS

SELECT distinct otg.*
,isnull(tb.TotalBilling, 0) as TotalBilling
,CONVERT(decimal(38, 2), tl.AverageLicensesActived / CONVERT(decimal(4,2), otg.Month)) as AverageLicensesActived
,isnull(CONVERT(decimal(38, 2), isnull(tby.TotalBillingYear, 0) / CONVERT(decimal(4,2), tc.TotalClients)), 0) as AverageForClient
,isnull(CONVERT(decimal(38, 2), isnull(tby.TotalBillingYear, 0) / CONVERT(decimal(4,2), tcd.TotalDevices)), 0) as AverageForLicenseClient
,isnull(tby.TotalBillingYear, 0) as TotalBillingYear
,sum(DevelopmentValueYear) as DevelopmentValueYear
,sum(OperationsWNDValueYear) as OperationsWNDValueYear
,sum(DataCenterValueYear) as DataCenterValueYear
,sum(OperationValueYear) as OperationValueYear
,sum(LicensesActiveYear) as LicensesActiveYear

FROM [dbo].[Outgoings] otg

left join (
    select distinct Sum(Value) as TotalBilling, MONTH(cb.PaymentDate) as Month, YEAR(cb.PaymentDate) as Year from dbo.Clients c
    inner join dbo.ClientsBillings cb on cb.ClientUId = c.ClientUId
                                  and c.Active = 1
                                  group by cb.PaymentDate
) tb on tb.Month = otg.[Month] and tb.Year = otg.[Year]

left join (
    select distinct Sum(Value) as TotalBillingYear, YEAR(cb.PaymentDate) as Year from dbo.Clients c
    inner join dbo.ClientsBillings cb on cb.ClientUId = c.ClientUId
                                  and c.Active = 1
                                  group by cb.PaymentDate
) tby on tby.Year = otg.[Year]

left join (
    select distinct sum(LicensesActive) as AverageLicensesActived, [Month], [Year] FROM [dbo].[Outgoings]
    group by [Month], [Year]
) tl on tl.Month = otg.[Month] and tl.Year = otg.[Year]

left join (
    select distinct count(*) as TotalClients, Created from dbo.Clients where Active = 1 group by Created
) tc on YEAR(tc.Created) = otg.[Year] and MONTH(tc.Created) <= otg.[Month]

left join (
    select distinct count(*) as TotalDevices, c.Created from dbo.Clients c
    inner join dbo.ClientsDevices cd on cd.ClientUId = c.ClientUId
    where c.Active = 1 and cd.Active = 1
    group by c.Created
) tcd on YEAR(tcd.Created) = otg.[Year] and MONTH(tcd.Created) <= otg.[Month]

left join (
    select distinct sum(DevelopmentValue) as DevelopmentValueYear
    ,sum(OperationsWNDValue) as OperationsWNDValueYear
    ,sum(DataCenterValue) as DataCenterValueYear
    ,sum(OperationValue) as OperationValueYear
    ,sum(LicensesActive) as LicensesActiveYear
    ,[Year], [Month] 
    FROM [dbo].[Outgoings]
    group by [Year], [Month]
) tdy on tdy.Year = otg.[Year] and tdy.[Month] <= otg.[Month]



group by OutgoingUId, otg.[Year]
      ,otg.[Month]
      ,[LicensesActive]
      ,[ClientsActive]
      ,[Description]
      ,[DevelopmentValue]
      ,[OperationsWNDValue]
      ,[DataCenterValue]
      ,[OperationValue]
      ,tb.TotalBilling
      ,tl.AverageLicensesActived
      ,tby.TotalBillingYear
      ,tc.TotalClients
      ,tcd.TotalDevices