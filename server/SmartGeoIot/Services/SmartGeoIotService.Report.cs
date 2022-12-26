using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Box.Common.Web;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        public IEnumerable<DashboardViewModels> GetReports(string id, ClaimsPrincipal user, string typePackage, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null, string orderBy = null, bool blocked = false, bool isCallByGraphic = false, ReportResilType reportType = ReportResilType.Analitico)
        {
            // Projeto Aguamon
            if (typePackage.Equals("10"))
                return GetReportDataAguamon(id, skip, top, totalCount);

            // Projeto Aguamon-2
            if (int.Parse(typePackage) == (int)PackagesEnum.TQA)
                return GetReportDataPQA(id, skip, top, de, ate, totalCount, isCallByGraphic);

            // Projeto Hidroponia
            if (typePackage.Equals("22"))
                return GetReportDataHidroponia(id, skip, top, de, ate, totalCount, orderBy);

            // Projeto DJRF
            if (typePackage.Equals("12"))
                return GetReportDataDJRF(id, skip, top, de, ate, blocked, totalCount, isCallByGraphic);

            // Projeto TRM
            // if (typePackage.Equals("21"))
            //     return GetReportDataTRM(id, skip, top, de, ate, totalCount);

            // Projeto TRM-10
            if (typePackage.Equals("23"))
                return GetReportDataTRM10_23_24(id, skip, top, de, ate, totalCount, isCallByGraphic, reportType);

            // Projeto TSP
            if (typePackage.Equals("83"))
                return GetReportDataTSP_83_24(id, skip, top, de, ate, totalCount, isCallByGraphic);

            // Projeto TRM-11
            if (int.Parse(typePackage) == (int)PackagesEnum.B978)
                return GetReportDataB978(id, skip, top, de, ate, totalCount);

            return null;
        }

        public IEnumerable<DashboardViewModels> GetReportDataDJRF(string id, int skip = 0, int top = 0, string de = null, string ate = null, bool blocked = false, OptionalOutTotalCount totalCount = null, bool isCallByGraphic = false)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();
            IQueryable<Message> messages = _context.Messages
            .AsNoTracking()
            .Include(i => i.Device)
            .Where(w => w.DeviceId == id && (w.TypePackage.Equals("12") || w.TypePackage.Equals("13")))
            .OrderByDescending(o => o.Time);

            try
            {
                // verificar status de bloqueado ou has-out
                if (blocked)
                    messages = messages.Where(c => c.Bits.EstadoBloqueio || c.Bits.EstadoSaidaRastreador);

                if (!de.Equals("null"))
                {
                    var firstDate = Convert.ToDateTime(de).ToUniversalTime().AddHours(-3);
                    messages = messages.Where(c => c.Date >= firstDate);
                }
                if (!ate.Equals("null"))
                {
                    var lastDate = Convert.ToDateTime(ate).ToUniversalTime().AddHours(-3).AddDays(1);
                    messages = messages.Where(c => c.Date <= lastDate);
                }

                if (messages == null)
                    return null;

                List<Message> tmpMessages = messages.ToList();
                if (totalCount != null)
                    totalCount.Value = tmpMessages.Count();

                if (isCallByGraphic)
                    top = totalCount.Value;

                while (tmpMessages.Count() > 0 && newData.Count() <= top)
                {
                    DashboardViewModels newItem = new DashboardViewModels();
                    var message12 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals("12"));
                    var message13 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals("13"));

                    // Convertendo os dados para pacote 12
                    if (message12 != null)
                    {
                        newItem.DeviceId = message12.DeviceId;
                        newItem.Name = message12.Device.Name;
                        newItem.Package = message12.Data;
                        newItem.TypePackage = message12.TypePackage;
                        newItem.Date = message12.Date;
                        newItem.Country = message12.Country;
                        newItem.Lqi = message12.Lqi;
                        newItem.Bits = message12.Bits;
                        newItem.EstadoDetector = message12.EstadoDetector;
                        newItem.PeriodoTransmissao = message12.PeriodoTransmissao;
                        newItem.ContadorCarencias = message12.ContadorCarencias;
                        newItem.ContadorBloqueios = message12.ContadorBloqueios;
                    }

                    // Convertendo os dados para pacote 13
                    if (message13 != null)
                    {
                        var firstCaracter = message13.Temperature.Substring(0, message13.Temperature.Length - 1);
                        var lastCaracter = message13.Temperature.Substring(message13.Temperature.Length - 1, 1);
                        newItem.Temperature = $"{firstCaracter},{lastCaracter}";
                        newItem.Alimentacao = message13.Alimentacao;
                    }

                    // set location on dashboard of device
                    // DeviceLocation deviceLocation = GetDeviceLocationByDeviceId(id);
                    // if (deviceLocation != null)
                    // {
                    //     newItem.Latitude = deviceLocation.Latitude.ToString();
                    //     newItem.Longitude = deviceLocation.Longitude.ToString();
                    //     newItem.Radius = deviceLocation.Radius;

                    //     newItem.LatitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Latitude, "S");
                    //     newItem.LongitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Longitude, "W");
                    //     newItem.RadiusConverted = RadiusFormated(deviceLocation.Radius);
                    // }

                    newData.Add(newItem);

                    tmpMessages.Remove(message12);
                    tmpMessages.Remove(message13);
                }

                if (skip != 0 && top != 0)
                {
                    if (isCallByGraphic)
                        return newData.Skip(skip).Take(top).OrderBy(o => o.Date).ToArray();

                    return newData.Skip(skip).Take(top).OrderByDescending(o => o.Date).ToArray();
                }

                if (skip != 0 && top == 0)
                {
                    if (isCallByGraphic)
                        return newData.Skip(skip).OrderBy(o => o.Date).ToArray();

                    return newData.Skip(skip).OrderByDescending(o => o.Date).ToArray();
                }

                if (skip == 0 && top != 0)
                {
                    if (isCallByGraphic)
                        return newData.Take(top).OrderBy(o => o.Date).ToArray();

                    return newData.Take(top).OrderByDescending(o => o.Date).ToArray();
                }

                if (isCallByGraphic)
                    return newData.OrderBy(o => o.Date).ToArray();

                return newData.OrderByDescending(o => o.Date).ToArray();
            }
            catch (System.Exception)
            {
                return newData;
            }
        }

        public IEnumerable<DashboardViewModels> GetReportDataHidroponia(string id, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null, string orderBy = null)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();
            IQueryable<Message> reportsQuery = _context.Messages.AsNoTracking().Include(i => i.Device).Where(w => w.DeviceId == id).OrderByDescending(o => o.Id);

            try
            {
                if (totalCount != null)
                    totalCount.Value = reportsQuery.Count();

                DateTime? lastDate = reportsQuery.Max(m => m.Date);
                DateTime? fisrtDate = reportsQuery.Min(m => m.Date);

                if (top != 0)
                    fisrtDate = lastDate?.AddDays(-top);

                if (!de.Equals("null"))
                    fisrtDate = Convert.ToDateTime(de).ToUniversalTime().AddHours(-3);

                if (!ate.Equals("null"))
                    lastDate = Convert.ToDateTime(ate).ToUniversalTime().AddHours(-3);

                for (DateTime i = fisrtDate.Value; i <= lastDate; i = i.AddDays(1))
                {
                    var deviceMessageType22 = reportsQuery
                        .Where(w => w.DeviceId == id
                        && w.TypePackage.Equals("22")
                        && w.OperationDate.Value.Year == i.Year
                        && w.OperationDate.Value.Month == i.Month
                        && w.OperationDate.Value.Day == i.Day)
                        .OrderByDescending(o => o.Id).FirstOrDefault();

                    var deviceMessageType23 = reportsQuery
                          .Where(w => w.DeviceId == id
                          && w.TypePackage.Equals("23")
                          && w.OperationDate.Value.Year == i.Year
                          && w.OperationDate.Value.Month == i.Month
                          && w.OperationDate.Value.Day == i.Day)
                          .OrderByDescending(o => o.Id).FirstOrDefault();


                    if (deviceMessageType22 != null && deviceMessageType23 != null)
                    {
                        DashboardViewModels newItem = new DashboardViewModels();
                        newItem.DeviceId = deviceMessageType22.DeviceId;
                        newItem.Name = deviceMessageType22.Device.Name;
                        newItem.Package = $"{deviceMessageType22.Data} | {deviceMessageType23.Data}";
                        newItem.TypePackage = $"{deviceMessageType22.TypePackage} e {deviceMessageType23.TypePackage}";
                        newItem.Date = deviceMessageType22.Date;
                        newItem.Country = deviceMessageType22.Country;
                        newItem.Lqi = deviceMessageType22.Lqi;
                        newItem.Bits = deviceMessageType22.Bits;
                        newItem.Level = deviceMessageType22.Level;
                        newItem.Light = deviceMessageType22.Light;
                        newItem.Temperature = deviceMessageType22.Temperature;
                        newItem.Moisture = deviceMessageType22.Moisture;
                        newItem.OxigenioDissolvido = deviceMessageType22.OxigenioDissolvido;
                        newItem.Ph = deviceMessageType23.Ph;
                        newItem.Condutividade = deviceMessageType23.Condutividade;
                        newItem.PeriodoTransmissao = deviceMessageType23.PeriodoTransmissao;
                        newItem.BaseT = deviceMessageType23.BaseT;

                        newData.Add(newItem);
                    }
                }
                // if (skip != 0)
                //     reportsQuery = reportsQuery.Skip(skip);
                // if (top != 0)
                //     newData = newData.Take(top);
                return newData.OrderByDescending(o => o.Date).ToArray();
            }
            catch (System.Exception)
            {
                return newData;
            }
        }

        public IEnumerable<DashboardViewModels> GetReportDataPQA(string id, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null, bool isCallByGraphic = false)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();
            IQueryable<Message> messages = _context.Messages
            .AsNoTracking()
            .Include(i => i.Device)
            .Where(w => w.DeviceId == id && (w.TypePackage.Equals(((int)PackagesEnum.TQA).ToString()) || w.TypePackage.Equals(((int)PackagesEnum.TQA_S).ToString())))
            .OrderByDescending(o => o.Time);

            try
            {
                if (!de.Equals("null"))
                {
                    DateTime firstDate = Convert.ToDateTime(de).ToUniversalTime();
                    messages = messages.Where(c => c.OperationDate.Value.AddHours(-3).Year >= firstDate.Year && c.OperationDate.Value.AddHours(-3).Month >= firstDate.Month && c.OperationDate.Value.AddHours(-3).Day >= firstDate.Day);
                }
                if (!ate.Equals("null"))
                {
                    var lastDate = Convert.ToDateTime(ate).ToUniversalTime();
                    messages = messages.Where(c => c.OperationDate.Value.AddHours(-3).Year <= lastDate.Year && c.OperationDate.Value.AddHours(-3).Month <= lastDate.Month && c.OperationDate.Value.AddHours(-3).Day <= lastDate.Day);
                }

                if (messages == null)
                    return null;

                List<Message> tmpMessages = messages.ToList();
                if (totalCount != null)
                    totalCount.Value = tmpMessages.Count();

                if (isCallByGraphic)
                    top = totalCount.Value;

                // while (tmpMessages.Count() > 0 && newData.Count() <= top)
                while (tmpMessages.Count() > 0)
                {
                    DashboardViewModels newItem = new DashboardViewModels();
                    var message81 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals(((int)PackagesEnum.TQA).ToString()));
                    var message82 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals(((int)PackagesEnum.TQA_S).ToString()));

                    // Convertendo os dados para pacote 81
                    if (message81 != null)
                    {
                        newItem.DeviceId = message81.DeviceId;
                        newItem.Name = message81.Device.Name;
                        newItem.Package = message81.Data;
                        newItem.TypePackage = message81.TypePackage;
                        newItem.Date = message81.Date;
                        newItem.Country = message81.Country;
                        newItem.Lqi = message81.Lqi;

                        newItem.Temperature = message81.Temperature == "0" ? "0,00" : message81.Temperature;
                        newItem.Ph = message81.Ph == "0" ? "0,00" : message81.Ph;

                        newItem.Fluor = message81.Fluor;
                        newItem.Cloro = message81.Cloro == ",00" ? "0,00" : message81.Cloro;
                        newItem.Turbidez = message81.Turbidez;

                        if (message82 != null)
                        {
                            newItem.ReleBoolean = message82.ReleBoolean;
                        }

                        // if (isCallByGraphic)
                        // {
                        //     newItem.Temperature = message81.Temperature.Length > 2 ? $"{message81.Temperature.Substring(0, 2)},{message81.Temperature.Substring(2, message81.Temperature.Length - 2)}" : message81.Temperature;
                        // }
                        // else
                        // {
                        //     newItem.Temperature = message81.Temperature.Length > 2 ? $"{message81.Temperature.Substring(0, 2)},{message81.Temperature.Substring(2, message81.Temperature.Length - 2)}" : message81.Temperature;
                        //     newItem.Ph = message81.Ph;
                        //     newItem.Fluor = message81.Fluor;
                        //     newItem.Cloro = message81.Cloro;
                        //     newItem.Turbidez = message81.Turbidez;
                        // }
                    }

                    // Convertendo os dados para pacote 82
                    // if (message82 != null && !isCallByGraphic)
                    // {
                    //     newItem.Rele = new Rele()
                    //     {
                    //         Rele1 = Utils.HexaToDecimal(message82.Package.Substring(0, 2)).ToString(),
                    //         Rele2 = Utils.HexaToDecimal(message82.Package.Substring(2, 2)).ToString(),
                    //         Rele3 = Utils.HexaToDecimal(message82.Package.Substring(4, 2)).ToString(),
                    //         Rele4 = Utils.HexaToDecimal(message82.Package.Substring(6, 2)).ToString(),
                    //         Rele5 = Utils.HexaToDecimal(message82.Package.Substring(8, 2)).ToString()
                    //     };
                    // }

                    if (!isCallByGraphic)
                    {
                        if (newItem.Date != default(DateTime) && newItem.Temperature != null)
                            newData.Add(newItem);
                    }
                    else
                    {
                        if (newItem.Date != default(DateTime) && newItem.Temperature != null)
                            newData.Add(newItem);
                        // if (!message81.TemperatureIsZero)
                        //     newData.Add(newItem);
                    }

                    tmpMessages.Remove(message81);
                    tmpMessages.Remove(message82);
                }

                if (skip != 0 && top != 0)
                {
                    if (isCallByGraphic)
                        return newData.Skip(skip).Take(top).OrderBy(o => o.Date).ToArray();

                    return newData.Skip(skip).Take(top).OrderByDescending(o => o.Date).ToArray();
                }

                if (skip != 0 && top == 0)
                {
                    if (isCallByGraphic)
                        return newData.Skip(skip).OrderBy(o => o.Date).ToArray();

                    return newData.Skip(skip).OrderByDescending(o => o.Date).ToArray();
                }

                if (skip == 0 && top != 0)
                {
                    if (isCallByGraphic)
                        return newData.Take(top).OrderBy(o => o.Date).ToArray();

                    return newData.Take(top).OrderByDescending(o => o.Date).ToArray();
                }

                if (isCallByGraphic)
                    return newData.OrderBy(o => o.Date).ToArray();

                return newData.OrderByDescending(o => o.Date).ToArray();
            }
            catch (System.Exception)
            {
                return newData;
            }
        }

        public IEnumerable<DashboardViewModels> GetReportDataAguamon(string id, int skip = 0, int top = 0, OptionalOutTotalCount totalCount = null)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();
            IQueryable<Message> reportsQuery = _context.Messages
            .AsNoTracking().Include(i => i.Device)
            .Where(w => w.DeviceId == id && (w.TypePackage.Equals("10"))).OrderByDescending(o => o.Id);

            try
            {
                if (totalCount != null)
                    totalCount.Value = reportsQuery.Count();

                if (skip != 0)
                    reportsQuery = reportsQuery.Skip(skip);

                if (top != 0)
                    reportsQuery = reportsQuery.Take(top);

                foreach (var report in reportsQuery)
                {
                    DashboardViewModels newItem = new DashboardViewModels();
                    newItem.DeviceId = report.DeviceId;
                    newItem.Name = report.Device.Name;
                    newItem.Package = report.Data;
                    newItem.TypePackage = report.TypePackage;
                    newItem.Date = report.Date;
                    newItem.Country = report.Country;
                    newItem.Lqi = report.Lqi;
                    newItem.Bits = report.Bits;

                    newItem.Temperature = (decimal.Parse(report.Temperature) * 100).ToString();
                    newItem.Temperature = report.Temperature.ToString().Substring(0, report.Temperature.Length - 2);
                    newItem.Envio = report.Envio;
                    newItem.PeriodoTransmissao = report.PeriodoTransmissao;
                    newItem.Alimentacao = $"{report.Alimentacao},0";
                    newItem.AlimentacaoMinima = $"{report.AlimentacaoMinima},0";

                    newData.Add(newItem);
                }

                return newData.OrderByDescending(o => o.Date).ToArray();
            }
            catch (System.Exception)
            {
                return newData;
            }
        }

        public IEnumerable<DashboardViewModels> GetReportDataB978(string id, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();
            IQueryable<Message> reportsQuery = _context.Messages.AsNoTracking().Include(i => i.Device).Where(w => w.DeviceId == id && (w.TypePackage.Equals("21"))).OrderByDescending(o => o.Id);
            DeviceRegistration deviceRegistration = _context.DevicesRegistration.Include(i => i.Package).Include(i => i.Project).SingleOrDefault(r => r.DeviceId == id);

            try
            {
                if (!de.Equals("null"))
                {
                    DateTime firstDate = Convert.ToDateTime(de).ToUniversalTime();
                    reportsQuery = reportsQuery.Where(c => c.OperationDate.Value.Year >= firstDate.Year && c.OperationDate.Value.Month >= firstDate.Month && c.OperationDate.Value.Day >= firstDate.Day);
                }
                if (!ate.Equals("null"))
                {
                    DateTime lastDate = Convert.ToDateTime(ate).ToUniversalTime();
                    reportsQuery = reportsQuery.Where(c => c.OperationDate.Value.Year <= lastDate.Year && c.OperationDate.Value.Month <= lastDate.Month && c.OperationDate.Value.Day <= lastDate.Day);
                }

                if (totalCount != null)
                    totalCount.Value = reportsQuery.Count();

                if (skip != 0)
                    reportsQuery = reportsQuery.Skip(skip);

                if (top != 0)
                    reportsQuery = reportsQuery.Take(top);

                foreach (var report in reportsQuery)
                {
                    DashboardViewModels newItem = new DashboardViewModels();
                    newItem.DeviceId = report.DeviceId;
                    newItem.Name = report.Device.Name;
                    newItem.Package = report.Data;
                    newItem.TypePackage = report.TypePackage;
                    newItem.Date = report.Date;
                    newItem.Country = report.Country;
                    newItem.Lqi = report.Lqi;
                    newItem.Bits = report.Bits;
                    newItem.Lqi = report.Lqi;
                    newItem.SeqNumber = report.SeqNumber;
                    newItem.Time = report.Time;

                    if (deviceRegistration != null)
                    {
                        newItem.SerialNumber = deviceRegistration.SerialNumber;
                        newItem.Model = deviceRegistration.Model;
                        newItem.Notes = deviceRegistration.Notes;
                        newItem.NotesCreateDate = deviceRegistration.NotesCreateDate.HasValue ? deviceRegistration.NotesCreateDate.Value.ToShortDateString() : null;
                        newItem.Ed1 = deviceRegistration.Ed1;
                        newItem.Ed2 = deviceRegistration.Ed2;
                        newItem.Ed3 = deviceRegistration.Ed3;
                        newItem.Ed4 = deviceRegistration.Ed4;
                        newItem.Sd1 = deviceRegistration.Sd1;
                        newItem.Sd2 = deviceRegistration.Sd2;
                        newItem.Ea10 = deviceRegistration.Ea10;
                        newItem.Sa3 = deviceRegistration.Sa3;
                    }

                    var _entradaAnalogica = Utils.FromFloatSafe(report.EntradaAnalogica);
                    var _saidaAnalogica = Utils.FromFloatSafe(report.SaidaAnalogica);

                    newItem.EntradaAnalogica = String.Format("{0:0.0}", _entradaAnalogica);
                    newItem.SaidaAnalogica = String.Format("{0:0.0}", _saidaAnalogica);

                    newData.Add(newItem);
                }

                return newData.OrderByDescending(o => o.Date).ToArray();
            }
            catch (System.Exception ex)
            {
                _log.Log("Erro GetReportDataTRM.", ex.Message, true);
                return newData;
            }
        }

        public IEnumerable<DashboardViewModels> GetReportDataTRM(string id, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();
            IQueryable<Message> reportsQuery = _context.Messages.AsNoTracking().Include(i => i.Device).Where(w => w.DeviceId == id && (w.TypePackage.Equals("21"))).OrderByDescending(o => o.Id);

            try
            {
                if (!de.Equals("null"))
                {
                    DateTime firstDate = Convert.ToDateTime(de).ToUniversalTime();
                    reportsQuery = reportsQuery.Where(c => c.OperationDate.Value.Year >= firstDate.Year && c.OperationDate.Value.Month >= firstDate.Month && c.OperationDate.Value.Day >= firstDate.Day);
                }
                if (!ate.Equals("null"))
                {
                    DateTime lastDate = Convert.ToDateTime(ate).ToUniversalTime();
                    reportsQuery = reportsQuery.Where(c => c.OperationDate.Value.Year <= lastDate.Year && c.OperationDate.Value.Month <= lastDate.Month && c.OperationDate.Value.Day <= lastDate.Day);
                }

                if (totalCount != null)
                    totalCount.Value = reportsQuery.Count();

                if (skip != 0)
                    reportsQuery = reportsQuery.Skip(skip);

                if (top != 0)
                    reportsQuery = reportsQuery.Take(top);

                foreach (var report in reportsQuery)
                {
                    DashboardViewModels newItem = new DashboardViewModels();
                    newItem.DeviceId = report.DeviceId;
                    newItem.Name = report.Device.Name;
                    newItem.Package = report.Data;
                    newItem.TypePackage = report.TypePackage;
                    newItem.Date = report.Date;
                    newItem.Country = report.Country;
                    newItem.Lqi = report.Lqi;
                    newItem.Bits = report.Bits;

                    var _entradaAnalogica = Utils.FromFloatSafe(report.EntradaAnalogica);
                    var _saidaAnalogica = Utils.FromFloatSafe(report.SaidaAnalogica);

                    newItem.EntradaAnalogica = String.Format("{0:0.0}", _entradaAnalogica);
                    newItem.SaidaAnalogica = String.Format("{0:0.0}", _saidaAnalogica);

                    newData.Add(newItem);
                }

                return newData.OrderByDescending(o => o.Date).ToArray();
            }
            catch (System.Exception ex)
            {
                _log.Log("Erro GetReportDataTRM.", ex.Message, true);
                return newData;
            }
        }

        public IEnumerable<DashboardViewModels> GetReportDataTRM10_23_24(string id, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null, bool isCallByGraphic = false, ReportResilType reportType = ReportResilType.Analitico)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();
            List<ReportResil> reports = null;

            if (reportType == ReportResilType.Hora)
                reports = _context.ReportResil.Where(c => c.DeviceId == id && c.FAtualizaHora).ToList();
            
            if (reportType == ReportResilType.Dia)
                reports = _context.ReportResil.Where(c => c.DeviceId == id && c.FAtualizaDia).ToList();

            if (reportType == ReportResilType.Mes)
                reports = _context.ReportResil.Where(c => c.DeviceId == id && c.FAtualizaMes).ToList();

            if (reportType == ReportResilType.Semana)
                reports = _context.ReportResil.Where(c => c.DeviceId == id && c.FAtualizaSem).ToList();

            if (!de.Equals("null"))
            {
                var firstDate = Convert.ToDateTime(de).ToUniversalTime().AddHours(-3);
                reports = reports.Where(c => c.Date >= firstDate).ToList();
            }
            if (!ate.Equals("null"))
            {
                var lastDate = Convert.ToDateTime(ate).ToUniversalTime().AddDays(1).AddHours(-3).AddMinutes(-1);
                reports = reports.Where(c => c.Date <= lastDate).ToList();
            }
            
            foreach (var dados in reports)
            {
                newData.Add(new DashboardViewModels()
                {   
                    DeviceId = dados.DeviceId,
                    Date = dados.Date.AddHours(-3),//.AddDays(-1),
                    FluxoAgua = String.Format("{0:0.0}", dados.Fluxo),
                    ConsumoAgua = String.Format("{0:0.0}", dados.ConsumoHora),
                    ConsumoDia = String.Format("{0:0.0}", dados.ConsumoDia),
                    ConsumoMes = String.Format("{0:0.0}", dados.ConsumoMes),
                    ConsumoSemana = String.Format("{0:0.0}", dados.ConsumoSemana),
                    Estado = dados.Estado,
                    Valvula = dados.Valvula,
                    Modo = dados.Modo,
                    DateWeekName = ""//$"Semana de {start.AddDays(1).ToShortDateString()} à {end.ToShortDateString()}"
                });
            }

            if (totalCount != null)
                totalCount.Value = newData.Count();

            newData = newData.OrderByDescending(o => o.Date).ToList();

            if (isCallByGraphic)
                newData = newData.Take(top).OrderBy(o => o.Date).ToList();

            if (skip != 0)
                newData = newData.Skip(skip).ToList();

            if (top != 0)
                newData = newData.Take(top).ToList();

            return newData.ToArray();
        }

        public IEnumerable<DashboardViewModels> GetReportDataTRM10_23_24_OLD(string id, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null, bool isCallByGraphic = false, ReportResilType reportType = ReportResilType.Analitico)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();

            List<ReportResil> reports = null;
            if (reportType != ReportResilType.Analitico)
            {
                if (reportType != ReportResilType.Mes)
                {
                    reports = _context.ReportResil.Where(c => c.DeviceId == id).ToList();

                    if (!de.Equals("null"))
                    {
                        var firstDate = Convert.ToDateTime(de).ToUniversalTime().AddHours(-3);
                        reports = reports.Where(c => c.Date >= firstDate).ToList();
                    }
                    if (!ate.Equals("null"))
                    {
                        var lastDate = Convert.ToDateTime(ate).ToUniversalTime().AddHours(-3).AddDays(1);
                        reports = reports.Where(c => c.Date <= lastDate).ToList();
                    }
                }
            }

            // SEMANA
            if (reportType == ReportResilType.Semana)
            {
                List<string> mensagem = new List<string>();
                DateTime dayInitial = reports.Min(m => m.Date);
                DateTime dayFinal = reports.Max(m => m.Date);

                DateTime start = dayInitial.Date.AddDays(-(int)dayInitial.DayOfWeek);
                start = start.AddDays(1);
                DateTime end = start.AddDays(6);

                for (DateTime i = start; i <= dayFinal; i.AddDays(1))
                {
                    ReportResil dados = null;
                    dados = reports.OrderByDescending(o => o.Date).FirstOrDefault(c => c.Month == end.Month && c.Year == end.Year && c.Day == end.Day);
                    if (dados == null)
                    {
                        DateTime tmpStart = start;
                        DateTime tmpEnd = end;
                        while (dados == null && tmpStart < tmpEnd)
                        {
                            tmpEnd = tmpEnd.AddDays(-1);
                            dados = reports.OrderByDescending(o => o.Date).FirstOrDefault(c => c.Month == tmpEnd.Month && c.Year == tmpEnd.Year && c.Day == tmpEnd.Day);
                        }
                    }

                    if (dados == null)
                    {
                        i = start = end;
                        end = end.AddDays(7);
                        continue;
                    }

                    newData.Add(new DashboardViewModels()
                    {   
                        DeviceId = dados.DeviceId,
                        Date = dados.Date.AddHours(-3),
                        FluxoAgua = String.Format("{0:0.0}", dados.Fluxo),
                        ConsumoAgua = String.Format("{0:0.0}", dados.ConsumoHora),
                        ConsumoDia = String.Format("{0:0.0}", dados.ConsumoDia),
                        ConsumoMes = String.Format("{0:0.0}", dados.ConsumoMes),
                        ConsumoSemana = String.Format("{0:0.0}", dados.ConsumoSemana),
                        Estado = dados.Estado,
                        Valvula = dados.Valvula,
                        Modo = dados.Modo,
                        DateWeekName = $"Semana de {start.AddDays(1).ToShortDateString()} à {end.ToShortDateString()}"
                    });

                    i = start = end;
                    end = end.AddDays(7);
                }

                if (totalCount != null)
                    totalCount.Value = newData.Count();

                newData = newData.OrderByDescending(o => o.Date).ToList();

                if (skip != 0)
                    newData = newData.Skip(skip).ToList();

                if (top != 0)
                    newData = newData.Take(top).ToList();

                return newData.ToArray();
            }

            // MÊS
            if (reportType == ReportResilType.Mes)
            {
                reports = _context.ReportResil.Where(c => c.DeviceId == id && c.Date >= c.Date.AddDays(-365)).ToList();
                DateTime dayInitial = reports.Min(m => m.Date);
                DateTime dayFinal = reports.Max(m => m.Date);
                for (DateTime x = dayInitial; x <= dayFinal.AddMonths(1); x = x.AddMonths(1))
                {
                    var dados = reports.OrderByDescending(o => o.Date).FirstOrDefault(c => c.Month == x.Month && c.Year == x.Year);
                    if (dados == null)
                        continue;

                    newData.Add(new DashboardViewModels()
                    {   
                        DeviceId = dados.DeviceId,
                        Date = dados.Date.AddHours(-3),
                        FluxoAgua = String.Format("{0:0.0}", dados.Fluxo),
                        ConsumoAgua = String.Format("{0:0.0}", dados.ConsumoHora),
                        ConsumoDia = String.Format("{0:0.0}", dados.ConsumoDia),
                        ConsumoMes = String.Format("{0:0.0}", dados.ConsumoMes),
                        ConsumoSemana = String.Format("{0:0.0}", dados.ConsumoSemana),
                        Estado = dados.Estado,
                        Valvula = dados.Valvula,
                        Modo = dados.Modo
                    });
                }

                if (totalCount != null)
                    totalCount.Value = newData.Count();

                newData = newData.OrderByDescending(o => o.Date).ToList();

                if (skip != 0)
                    newData = newData.Skip(skip).ToList();

                if (top != 0)
                    newData = newData.Take(top).ToList();

                return newData.ToArray();
            }

            // DIA
            if (reportType == ReportResilType.Dia)
            {
                DateTime dayInitial = reports.Min(m => m.Date);
                DateTime dayFinal = reports.Max(m => m.Date);
                for (DateTime x = dayInitial; x <= dayFinal.AddDays(1); x = x.AddDays(1))
                {
                    var dados = reports.OrderByDescending(o => o.Date).FirstOrDefault(c => c.Day == x.Day && c.Month == x.Month && c.Year == x.Year);
                    if (dados == null)
                        continue;
                    newData.Add(new DashboardViewModels()
                    {   
                        DeviceId = dados.DeviceId,
                        Date = dados.Date.AddHours(-3),
                        FluxoAgua = String.Format("{0:0.0}", dados.Fluxo),
                        ConsumoAgua = String.Format("{0:0.0}", dados.ConsumoHora),
                        ConsumoDia = String.Format("{0:0.0}", dados.ConsumoDia),
                        ConsumoMes = String.Format("{0:0.0}", dados.ConsumoMes),
                        ConsumoSemana = String.Format("{0:0.0}", dados.ConsumoSemana),
                        Estado = dados.Estado,
                        Valvula = dados.Valvula,
                        Modo = dados.Modo
                    });
                }

                if (totalCount != null)
                    totalCount.Value = newData.Count();

                newData = newData.OrderByDescending(o => o.Date).ToList();

                if (skip != 0)
                    newData = newData.Skip(skip).ToList();

                if (top != 0)
                    newData = newData.Take(top).ToList();

                return newData.ToArray();
            }

            // HORA
            if (reportType != ReportResilType.Analitico)
            {
                if (totalCount != null)
                    totalCount.Value = reports.Count();

                reports = reports.OrderByDescending(o => o.Date).ToList();

                if (skip != 0)
                    reports = reports.Skip(skip).ToList();

                if (top != 0)
                    reports = reports.Take(top).ToList();

                return reports.Select(dados => new DashboardViewModels()
                {   
                    DeviceId = dados.DeviceId,
                    Date = dados.Date.AddHours(-3),
                    FluxoAgua = String.Format("{0:0.0}", dados.Fluxo),
                    ConsumoAgua = String.Format("{0:0.0}", dados.ConsumoHora),
                    ConsumoDia = String.Format("{0:0.0}", dados.ConsumoDia),
                    ConsumoMes = String.Format("{0:0.0}", dados.ConsumoMes),
                    ConsumoSemana = String.Format("{0:0.0}", dados.ConsumoSemana),
                    Estado = dados.Estado,
                    Valvula = dados.Valvula,
                    Modo = dados.Modo
                });
            }


            IQueryable<Message> messages = _context.Messages.AsNoTracking()
            .Include(i => i.Device)
            .Where(w => w.DeviceId == id && (w.TypePackage.Equals("23")) || (w.TypePackage.Equals("24")))
            .OrderByDescending(o => o.Id);

            try
            {
                if (!de.Equals("null"))
                {
                    var firstDate = Convert.ToDateTime(de).ToUniversalTime().AddHours(-3);
                    messages = messages.Where(c => c.Date >= firstDate);
                }
                if (!ate.Equals("null"))
                {
                    var lastDate = Convert.ToDateTime(ate).ToUniversalTime().AddHours(-3).AddDays(1);
                    messages = messages.Where(c => c.Date <= lastDate);
                }

                if (messages == null)
                    return null;
    
                List<Message> tmpMessages = messages.ToList();
                if (totalCount != null)
                    totalCount.Value = tmpMessages.Count();

                if (isCallByGraphic)
                    top = totalCount.Value;

                while (tmpMessages.Count() > 0 && newData.Count() <= top)
                {
                    DashboardViewModels newItem = null;
                    var message23 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals("23"));
                    var message24 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals("24"));

                    // Convertendo os dados para pacote 23
                    if (message23 != null)
                    {
                        newItem = new DashboardViewModels();
                        newItem.DeviceId = message23.DeviceId;
                        newItem.Name = message23.Device.Name;
                        newItem.Package = message23.Data;
                        newItem.TypePackage = message23.TypePackage;
                        newItem.Date = message23.Date;
                        newItem.Country = message23.Country;
                        newItem.Lqi = message23.Lqi;
                        newItem.Bits = message23.Bits;
                        newItem.SeqNumber = message23.SeqNumber;

                        var _fluxoAgua = Utils.FromFloatSafe(message23.FluxoAgua);
                        var _consumoAgua = Utils.FromFloatSafe(message23.ConsumoAgua);

                        newItem.FluxoAgua = String.Format("{0:0.0}", _fluxoAgua);
                        newItem.ConsumoAgua = String.Format("{0:0.0}", _consumoAgua);

                        var _display = Consts.GetDisplayTRM10(newItem.Bits.BAlertaMax, newItem.Bits.ModoFechado, newItem.Bits.ModoAberto);
                        newItem.Modo = _display.DisplayModo; // modo
                        newItem.Estado = _display.DisplayEstado; // alerta
                        newItem.EstadoImage = _display.EstadoImage;
                        newItem.ModoImage = _display.ModoImage;
                        newItem.Valvula = _display.DisplayValvula;
                        newItem.EstadoColor = _display.EstadoColor;

                        if (message24 != null)
                        {
                            var _consumoDia = Utils.FromFloatSafe(message24.ConsumoDia);
                            var _consumoSemana = Utils.FromFloatSafe(message24.ConsumoSemana);

                            newItem.ConsumoDia = String.Format("{0:0,0}", _consumoDia);
                            newItem.ConsumoSemana = String.Format("{0:0,0}", _consumoSemana);
                            newItem.ConsumoMes = String.Format("{0:0,0}", message24.ConsumoMes);
                        }
                    }

                    // Convertendo os dados para pacote 24

                    if (newItem != null)
                        newData.Add(newItem);

                    tmpMessages.Remove(message23);
                    tmpMessages.Remove(message24);
                }

                totalCount.Value = newData.Count();

                if (skip != 0 && top != 0)
                {
                    if (isCallByGraphic)
                        return newData.Skip(skip).Take(top).OrderBy(o => o.Date).ToArray();

                    return newData.Skip(skip).Take(top).OrderByDescending(o => o.Date).ToArray();
                }

                if (skip != 0 && top == 0)
                {
                    if (isCallByGraphic)
                        return newData.Skip(skip).OrderBy(o => o.Date).ToArray();

                    return newData.Skip(skip).OrderByDescending(o => o.Date).ToArray();
                }

                if (skip == 0 && top != 0)
                {
                    if (isCallByGraphic)
                        return newData.Take(top).OrderBy(o => o.Date).ToArray();

                    return newData.Take(top).OrderByDescending(o => o.Date).ToArray();
                }

                if (isCallByGraphic)
                    return newData.OrderBy(o => o.Date).ToArray();

                return newData.OrderByDescending(o => o.Date).ToArray();
            }
            catch (System.Exception ex)
            {
                _log.Log("Erro GetReportDataTRM.", ex.Message, true);
                return newData;
            }
        }

        public IEnumerable<DashboardViewModels> GetReportDataTSP_83_24(string id, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null, bool isCallByGraphic = false)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();
            IQueryable<Message> messages = _context.Messages.AsNoTracking()
            .Include(i => i.Device)
            .Where(w => w.DeviceId == id && w.TypePackage.Equals("83"))
            .OrderByDescending(o => o.Id);

            try
            {
                if (!de.Equals("null"))
                {
                    var firstDate = Convert.ToDateTime(de).ToUniversalTime().AddHours(-3);
                    messages = messages.Where(c => c.Date >= firstDate);
                }
                if (!ate.Equals("null"))
                {
                    var lastDate = Convert.ToDateTime(ate).ToUniversalTime().AddHours(-3).AddDays(1);
                    messages = messages.Where(c => c.Date <= lastDate);
                }

                if (messages == null)
                    return null;

                List<Message> tmpMessages = messages.ToList();
                if (totalCount != null)
                    totalCount.Value = tmpMessages.Count();

                if (isCallByGraphic)
                    top = totalCount.Value;

                while (tmpMessages.Count() > 0 && newData.Count() <= top)
                {
                    DashboardViewModels newItem = new DashboardViewModels();
                    var message83 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals("83"));
                    // var message24 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals("24"));

                    // Convertendo os dados para pacote 23
                    if (message83 != null)
                    {
                        newItem.DeviceId = message83.DeviceId;
                        newItem.Name = message83.Device.Name;
                        newItem.Package = message83.Data;
                        newItem.TypePackage = message83.TypePackage;
                        newItem.Date = message83.Date;
                        newItem.Country = message83.Country;
                        newItem.Lqi = message83.Lqi;
                        newItem.Bits = message83.Bits;
                        newItem.SeqNumber = message83.SeqNumber;

                        // var _vazao = Utils.FromFloatSafe(message83.Vazao);
                        // var _totalizacao = Utils.FromFloatSafe(message83.Totalizacao);
                        // newItem.Vazao = String.Format("{0:0.000}", _vazao);
                        double _vazao = 0;
                        if (message83.Operator != null)
                        {
                            // double currentOperator = Convert.ToDouble(message83.Operator);
                            // newItem.Vazao = String.Format("{0:0.000}", currentOperator);
                            newItem.Vazao = message83.Operator.Replace(".", ",");
                            newItem.Date = Utils.Timestamp_Milisecodns_ToDateTime_UTC(message83.Time);
                        }
                        else
                        {
                            _vazao = Utils.FromFloatSafe(message83.Vazao);
                            newItem.Vazao = String.Format("{0:0.000}", _vazao);
                        }

                        // newItem.Totalizacao = String.Format("{0:0.0}", _totalizacao);
                        var _totalizacao = Utils.FromFloatSafe(message83.Totalizacao); // total
                        newItem.Totalizacao = String.Format("{0:0}", _totalizacao);

                        newItem.Calha = message83.Calha;
                        newItem.CalhaAlerta = message83.CalhaAlerta;
                    }

                    // Convertendo os dados para pacote 24
                    // if (message24 != null)
                    // {
                    //     // var _consumoDia = Utils.FromFloatSafe(message24.ConsumoDia);
                    //     var _consumoSemana = Utils.FromFloatSafe(message24.ConsumoSemana);

                    //     // newItem.ConsumoDia = String.Format("{0:0,0}", _consumoDia);
                    //     newItem.ConsumoSemana = String.Format("{0:0,0}", _consumoSemana);
                    //     // newItem.ConsumoMes = String.Format("{0:0,0}", message24.ConsumoMes);
                    // }

                    newData.Add(newItem);

                    tmpMessages.Remove(message83);
                    // tmpMessages.Remove(message24);
                }

                if (skip != 0 && top != 0)
                {
                    if (isCallByGraphic)
                        return newData.Skip(skip).Take(top).OrderBy(o => o.Date).ToArray();

                    return newData.Skip(skip).Take(top).OrderByDescending(o => o.Date).ToArray();
                }

                if (skip != 0 && top == 0)
                {
                    if (isCallByGraphic)
                        return newData.Skip(skip).OrderBy(o => o.Date).ToArray();

                    return newData.Skip(skip).OrderByDescending(o => o.Date).ToArray();
                }

                if (skip == 0 && top != 0)
                {
                    if (isCallByGraphic)
                        return newData.Take(top).OrderBy(o => o.Date).ToArray();

                    return newData.Take(top).OrderByDescending(o => o.Date).ToArray();
                }

                if (isCallByGraphic)
                    return newData.OrderBy(o => o.Date).ToArray();

                return newData.OrderByDescending(o => o.Date).ToArray();
            }
            catch (System.Exception ex)
            {
                _log.Log("Erro GetReportDataTSP.", ex.Message, true);
                return newData;
            }
        }




    }
}