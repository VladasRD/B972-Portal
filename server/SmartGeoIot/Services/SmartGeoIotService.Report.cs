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
    public partial class SmartGeoIotService
    {
        public IEnumerable<DashboardViewModels> GetReports(string id, ClaimsPrincipal user, string typePackage, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null, string orderBy = null, bool blocked = false, bool isCallByGraphic = false)
        {
            // Projeto Aguamon
            if (typePackage.Equals("10"))
                return GetReportDataAguamon(id, skip, top, totalCount);

            // Projeto Aguamon-2
            if (typePackage.Equals("81"))
                return GetReportDataPQA(id, skip, top, de, ate, totalCount, isCallByGraphic);

            // Projeto Hidroponia
            if (typePackage.Equals("22"))
                return GetReportDataHidroponia(id, skip, top, de, ate, totalCount, orderBy);

            // Projeto DJRF
            if (typePackage.Equals("12"))
                return GetReportDataDJRF(id, skip, top, de, ate, blocked, totalCount, isCallByGraphic);

            // Projeto TRM
            if (typePackage.Equals("21"))
                return GetReportDataTRM(id, skip, top, de, ate, totalCount);

            // Projeto TRM-10
            if (typePackage.Equals("23"))
                return GetReportDataTRM10(id, skip, top, de, ate, totalCount, isCallByGraphic);

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
            .Where(w => w.DeviceId == id && (w.TypePackage.Equals("81") || w.TypePackage.Equals("82")))
            .OrderByDescending(o => o.Time);

            try
            {
                if (!de.Equals("null"))
                {
                    DateTime firstDate = Convert.ToDateTime(de).ToUniversalTime();
                    messages = messages.Where(c => c.OperationDate.Value.Year >= firstDate.Year && c.OperationDate.Value.Month >= firstDate.Month && c.OperationDate.Value.Day >= firstDate.Day);
                }
                if (!ate.Equals("null"))
                {
                    var lastDate = Convert.ToDateTime(ate).ToUniversalTime();
                    messages = messages.Where(c => c.OperationDate.Value.Year <= lastDate.Year && c.OperationDate.Value.Month <= lastDate.Month && c.OperationDate.Value.Day <= lastDate.Day);
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
                    var message81 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals("81"));
                    var message82 = tmpMessages.FirstOrDefault(w => w.TypePackage.Equals("82"));

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

                        if (isCallByGraphic)
                        {
                            newItem.Temperature = message81.Temperature.Length > 2 ? $"{message81.Temperature.Substring(0, 2)},{message81.Temperature.Substring(2, message81.Temperature.Length - 2)}" : message81.Temperature;
                        }
                        else
                        {
                            newItem.Temperature = message81.Temperature.Length > 2 ? $"{message81.Temperature.Substring(0, 2)},{message81.Temperature.Substring(2, message81.Temperature.Length - 2)}" : message81.Temperature;
                            newItem.Ph = message81.Ph;
                            newItem.Fluor = message81.Fluor;
                            newItem.Cloro = message81.Cloro;
                            newItem.Turbidez = message81.Turbidez;
                        }
                    }

                    // Convertendo os dados para pacote 82
                    if (message82 != null && !isCallByGraphic)
                    {
                        newItem.Rele = new Rele()
                        {
                            Rele1 = Utils.HexaToDecimal(message82.Package.Substring(0, 2)).ToString(),
                            Rele2 = Utils.HexaToDecimal(message82.Package.Substring(2, 2)).ToString(),
                            Rele3 = Utils.HexaToDecimal(message82.Package.Substring(4, 2)).ToString(),
                            Rele4 = Utils.HexaToDecimal(message82.Package.Substring(6, 2)).ToString(),
                            Rele5 = Utils.HexaToDecimal(message82.Package.Substring(8, 2)).ToString()
                        };
                    }

                    if (!isCallByGraphic)
                        newData.Add(newItem);
                    else
                    {
                        if (!message81.TemperatureIsZero)
                            newData.Add(newItem);
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

                    var _entradaAnalogica = FromFloatSafe(report.EntradaAnalogica);
                    var _saidaAnalogica = FromFloatSafe(report.SaidaAnalogica);

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

        public IEnumerable<DashboardViewModels> GetReportDataTRM10(string id, int skip = 0, int top = 0, string de = null, string ate = null, OptionalOutTotalCount totalCount = null, bool isCallByGraphic = false)
        {
            List<DashboardViewModels> newData = new List<DashboardViewModels>();
            IQueryable<Message> reportsQuery = _context.Messages.AsNoTracking().Include(i => i.Device).Where(w => w.DeviceId == id && (w.TypePackage.Equals("23"))).OrderByDescending(o => o.Id);

            try
            {
                if (!de.Equals("null"))
                {
                    DateTime firstDate = Convert.ToDateTime(de).ToUniversalTime();
                    reportsQuery = reportsQuery.Where(c => c.OperationDate.Value.Year >= firstDate.Year && c.OperationDate.Value.Month >= firstDate.Month && c.OperationDate.Value.Day >= firstDate.Day);
                }
                if (!ate.Equals("null"))
                {
                    var lastDate = Convert.ToDateTime(ate).ToUniversalTime();
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

                    var _fluxoAgua = FromFloatSafe(report.FluxoAgua);
                    var _consumoAgua = FromFloatSafe(report.ConsumoAgua);

                    newItem.FluxoAgua = String.Format("{0:0.0}", _fluxoAgua);
                    newItem.ConsumoAgua = String.Format("{0:0.0}", _consumoAgua);

                    if (!isCallByGraphic)
                    {
                        var _display = Consts.GetDisplayTRM10(newItem.Bits.BAlertaMax, newItem.Bits.ModoFechado, newItem.Bits.ModoAberto);
                        newItem.Modo = _display.DisplayModo; // modo
                        newItem.Estado = _display.DisplayEstado; // alerta
                        newItem.Valvula = _display.DisplayValvula; // válvula
                        newItem.EstadoColor = _display.EstadoColor;
                    }

                    newData.Add(newItem);
                }

                return newData.OrderBy(o => o.Date).ToArray();
            }
            catch (System.Exception ex)
            {
                _log.Log("Erro GetReportDataTRM.", ex.Message, true);
                return newData;
            }
        }



    }
}