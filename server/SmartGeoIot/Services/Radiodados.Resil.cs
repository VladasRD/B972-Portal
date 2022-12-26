using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        internal void ProcessReportResil(List<Message> messages)
        {
            _log.Log("Processando dados do relatório resil.", "Report.Resil", true);

            List<ReportResil> listReports = new List<ReportResil>();
            messages = messages.Where(c => c.TypePackage.Equals("23") || c.TypePackage.Equals("24")).ToList();
            Message currentMessage23 = null;
            Message currentMessage24 = null;
            // int finalHour = 23;

            // DateTime dayInitial = messages.Min(m => m.OperationDate.Value);
            // DateTime dayFinal = messages.Max(m => m.OperationDate.Value);
            // for (DateTime x = dayInitial; x <= dayFinal.AddDays(1); x = x.AddDays(1))
            foreach (var item in messages)
            {
                // var tmpMessages = messages.Where(c => c.OperationDate.Value.Day == x.Day && c.OperationDate.Value.Month == x.Month && c.OperationDate.Value.Year == x.Year).ToList();

                // for (int i = 0; i <= finalHour; i++)
                // {
                try
                {
                    // var currentMessages = tmpMessages.Where(c => c.OperationDate.Value.Hour == i).ToList();
                    // if (currentMessages.Count == 0)
                    //     continue;

                    if (currentMessage23 == null)
                    {
                        if (item.TypePackage.Equals("23"))
                        {
                            if (item.Bits.FAtualizaHora || item.Bits.FAtualizaDia || item.Bits.FAtualizaSem || item.Bits.FAtualizaMes)
                            {
                                currentMessage23 = item;
                            }
                        }
                    }

                    if (currentMessage24 == null)
                    {
                        if (item.TypePackage.Equals("24"))
                            currentMessage24 = item;
                    }

                    if (currentMessage23 == null || currentMessage24 == null)
                        continue;

                    var currentDashboard = CreateDashboard_Pack23_24ViewModel(currentMessage23, currentMessage24);
                    if (currentDashboard != null)
                    {
                        currentMessage23 = null;
                        currentMessage24 = null;
                    }

                    listReports.Add(new ReportResil()
                    {
                        Id = currentDashboard.Time.ToString(),
                        DeviceId = currentDashboard.DeviceId,
                        Time = currentDashboard.Time,
                        Day = currentDashboard.Date.Day,
                        Month = currentDashboard.Date.Month,
                        Year = currentDashboard.Date.Year,
                        Hour = currentDashboard.Date.Hour,
                        Minute = currentDashboard.Date.Minute,
                        ConsumoHora = decimal.Parse(currentDashboard.ConsumoAgua),
                        ConsumoDia = decimal.Parse(currentDashboard.ConsumoDia),
                        ConsumoSemana = decimal.Parse(currentDashboard.ConsumoSemana),
                        ConsumoMes = decimal.Parse(currentDashboard.ConsumoMes),
                        Fluxo = decimal.Parse(currentDashboard.FluxoAgua),
                        Modo = currentDashboard.Modo,
                        Estado = currentDashboard.Estado,
                        Valvula = currentDashboard.Valvula,
                        Date = currentDashboard.Date,
                        FAtualizaDia = currentDashboard.Bits.FAtualizaDia,
                        FAtualizaHora = currentDashboard.Bits.FAtualizaHora,
                        FAtualizaMes = currentDashboard.Bits.FAtualizaMes,
                        FAtualizaSem = currentDashboard.Bits.FAtualizaSem
                    });
                }
                catch (System.Exception ex)
                {
                    _log.Log("Erro ao processar report resil.", ex.Message, true);
                    continue;
                }
                // }

                // x = x.AddDays(1);
            }







            SaveReportResil(listReports);
        }

        internal void SaveReportResil(List<ReportResil> reportsResil)
        {
            if (reportsResil.Count > 0)
            {
                foreach (var item in reportsResil)
                {
                    bool oldReport = GetReportResil(item);
                    if (!oldReport)
                    {
                        _context.ReportResil.Add(item);
                        _context.SaveChanges();
                        _log.Log("Report Resil criado.");
                    }
                }
            }
        }

        internal bool GetReportResil(ReportResil reportsResil)
        {
            return _context.ReportResil
            .AsNoTracking()
            .Any(c =>
                c.DeviceId == reportsResil.DeviceId &&
                c.Day == reportsResil.Day &&
                c.Month == reportsResil.Month &&
                c.Year == reportsResil.Year &&
                c.Hour == reportsResil.Hour &&
                (
                    c.FAtualizaDia ||
                    c.FAtualizaHora ||
                    c.FAtualizaMes ||
                    c.FAtualizaSem
                )
            );
        }


    }
}