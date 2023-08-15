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
        public IEnumerable<DashboardViewModels> GetDashboards(ClaimsPrincipal user, OptionalOutTotalCount totalCount = null, bool isFullAcess = false)
        {
            List<DashboardViewModels> dashboards = new List<DashboardViewModels>();

            IQueryable<Device> devices = _context.Devices.Where(w => w.Activable);
            if (!isFullAcess)
            {
                // filtrar apenas os dispositivos que tem acesso
                var userDevices = GetUserDevices(user.GetId());
                devices = devices.Where(c => userDevices.Any(a => a.Id == c.Id));
            }

            foreach (var device in devices)
            {
                Models.Message deviceMessageType22 = _context.Messages.AsNoTracking().Include(i => i.Device)
                .Where(w => w.DeviceId == device.Id && w.Data.Substring(0, 2) == "22")
                .OrderByDescending(o => o.Id).SingleOrDefault();

                Models.Message deviceMessageType23 = _context.Messages.AsNoTracking().Include(i => i.Device)
                    .Where(w => w.DeviceId == device.Id && w.Data.Substring(0, 2) == "23")
                    .OrderByDescending(o => o.Id).SingleOrDefault();

                if (deviceMessageType22 == null)
                    return null;

                dashboards.Add(CreateDashboard_Pack22_23ViewModel(deviceMessageType22, deviceMessageType23, null));
            }

            totalCount.Value = dashboards.Count();
            return dashboards.ToArray();
        }

        public DashboardViewModels GetDashboard(string id, DateTime? date = null, long time = 0, string navigation = null, long timeb = 0, string project = null)
        {
            DeviceRegistration deviceRegistration = null;

            // Projeto MCond
            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B987).ToLower())
                deviceRegistration = _context.DevicesRegistration.Include(i => i.Package).Include(i => i.Project).Include(c => c.Device).SingleOrDefault(r => r.DeviceId == id);
            else
                deviceRegistration = _context.DevicesRegistration.Include(i => i.Package).Include(i => i.Project).SingleOrDefault(r => r.DeviceId == id);

            // try
            // {

            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B972_P).ToLower()) // TRM-10 (P972U1)
            {
                Models.Message deviceMessage23 = null;
                Models.Message deviceMessage24 = null;

                if (time == 0)
                {
                    if (date == null)
                    {
                        deviceMessage23 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23");

                        deviceMessage24 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "24");
                    }
                    else
                    {
                        date = date.Value.AddHours(3);
                        deviceMessage23 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23" &&
                                    w.OperationDate.Value.Year == date.Value.Year &&
                                    w.OperationDate.Value.Month == date.Value.Month &&
                                    w.OperationDate.Value.Day == date.Value.Day &&
                                    w.OperationDate.Value.Hour == date.Value.Hour &&
                                    w.OperationDate.Value.Minute == date.Value.Minute);
                        
                        if (deviceMessage23 == null)
                        {
                            deviceMessage23 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23" &&
                                    w.OperationDate.Value < date.Value);
                        }

                        deviceMessage24 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "24" &&
                                    w.OperationDate.Value.Year == date.Value.Year &&
                                    w.OperationDate.Value.Month == date.Value.Month &&
                                    w.OperationDate.Value.Day == date.Value.Day &&
                                    w.OperationDate.Value.Hour == date.Value.Hour &&
                                    w.OperationDate.Value.Minute == date.Value.Minute);

                        if (deviceMessage24 == null)
                        {
                            deviceMessage24 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "24" &&
                                    w.OperationDate.Value == date.Value);
                        }
                    }
                }
                else
                {
                    bool nextNavigation = navigation.Equals("next");

                    // while (deviceMessage23 == null)
                    // {
                    if (nextNavigation)
                    {
                        deviceMessage23 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderBy(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23" && w.Time > time);
                    }
                    else
                    {
                        deviceMessage23 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderByDescending(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23" && w.Time < time);
                    }

                    if (timeb == 0)
                    {
                        deviceMessage24 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderByDescending(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "24");
                    }
                    else
                    {
                        if (nextNavigation)
                        {
                            deviceMessage24 = _context.Messages.AsNoTracking()
                                .Include(i => i.Device)
                                .OrderBy(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "24" && w.Time > timeb);
                        }
                        else
                        {
                            deviceMessage24 = _context.Messages.AsNoTracking()
                                .Include(i => i.Device)
                                .OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "24" && w.Time < timeb);
                        }
                    }

                        // if (navigation != null && deviceMessage23 == null)
                        //     time = navigation.Equals("next") ? time+1 : time-1;
                    // }
                }

                if (deviceMessage23 == null)
                {
                    deviceMessage23 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23");

                    deviceMessage24 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "24");
                }

                if (deviceMessage23 == null)
                    return null;

                // TESTES
                // deviceMessage23.Data = "2304EBF64742720C03420054";
                // deviceMessage24.Data = "2400002F4400803B460BB85B";

                return CreateDashboard_Pack23_24ViewModel(deviceMessage23, deviceMessage24, deviceRegistration);
            }
            
            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.Hidroleg).ToLower()) // PROJETO SENSOR DE HIDROPONIA
            {
                Models.Message deviceMessageType22 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "22");

                Models.Message deviceMessageType23 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23");

                if (deviceMessageType22 == null || deviceMessageType23 == null)
                    return null;

                return CreateDashboard_Pack22_23ViewModel(deviceMessageType22, deviceMessageType23, deviceRegistration);
            }
            
            if (deviceRegistration.Package.Type.Equals("10")) // PROJETO AGUAMON
            {
                Models.Message deviceMessage = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "10");

                if (deviceMessage == null)
                    return null;

                return CreateDashboard_Pack10ViewModel(deviceMessage, deviceRegistration);
            }
            
            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.DJRFleg).ToLower()) // PROJETO DJRF
            {
                Models.Message deviceMessage12 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "12");

                Models.Message deviceMessage13 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "13");

                if (deviceMessage12 == null || deviceMessage13 == null)
                    return null;

                return CreateDashboard_Pack12ViewModel(deviceMessage12, deviceMessage13, deviceRegistration);
            }

            // Projeto MCond
            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B987).ToLower())
            {
                bool nextNavigation = navigation.Equals("next");

                return CreateDashboardMCond(deviceRegistration, nextNavigation, (int)time, date);
            }


            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B978).ToLower()) // TRM-11
            {
                Models.Message deviceMessage21 = null;

                if (time == 0)
                {
                    if (date == null)
                    {
                        deviceMessage21 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "21");
                    }
                    else
                    {
                        date = date.Value.AddHours(3);
                        deviceMessage21 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "21" &&
                                    w.OperationDate.Value.Year == date.Value.Year &&
                                    w.OperationDate.Value.Month == date.Value.Month &&
                                    w.OperationDate.Value.Day == date.Value.Day &&
                                    w.OperationDate.Value.Hour == date.Value.Hour &&
                                    w.OperationDate.Value.Minute == date.Value.Minute);

                        if (deviceMessage21 == null)
                        {
                            deviceMessage21 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "21" &&
                                    w.OperationDate.Value < date.Value);
                        }
                    }
                }
                else
                {
                    bool nextNavigation = navigation.Equals("next");

                    if (nextNavigation)
                    {
                        deviceMessage21 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderBy(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "21" && w.Time > time);
                    }
                    else
                    {
                        deviceMessage21 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderByDescending(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "21" && w.Time < time);
                    }
                }

                if (deviceMessage21 == null)
                {
                    deviceMessage21 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "21");
                }

                if (deviceMessage21 == null)
                    return null;

                return CreateDashboard_Pack21ViewModel(deviceMessage21, deviceRegistration);
            }


            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B981).ToLower()) // PQA
            {
                Models.Message deviceMessage81 = null;
                Models.Message deviceMessage82 = null;

                if (time == 0)
                {
                    deviceMessage81 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "81");

                    deviceMessage82 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "82");
                }
                else
                {
                    bool nextNavigation = navigation.Equals("next");
                    // while (deviceMessage81  == null)
                    // {
                    if (nextNavigation)
                    {
                        deviceMessage81 = _context.Messages.AsNoTracking()
                        .Include(i => i.Device)
                        .OrderBy(o => o.Id)
                        .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "81" && w.Time > time);
                    }
                    else
                    {
                        deviceMessage81 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderByDescending(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "81" && w.Time < time);
                    }

                        // if (navigation != null && deviceMessage81 == null)
                        //     time = navigation.Equals("next") ? time+1 : time-1;
                    // }

                    // while (deviceMessage82  == null)
                    // {
                    if (nextNavigation)
                    {
                        deviceMessage82 = _context.Messages.AsNoTracking()
                        .Include(i => i.Device)
                        .OrderBy(o => o.Id)
                        .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "82" && w.Time > timeb);
                    }
                    else
                    {
                        deviceMessage82 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderByDescending(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "82" && w.Time < timeb);
                    }

                        // if (navigation != null && deviceMessage82 == null)
                        //     timeb = navigation.Equals("next") ? timeb+1 : timeb-1;
                    // }
                }

                if (deviceMessage81 == null)
                {
                    deviceMessage81 = _context.Messages.AsNoTracking()
                        .Include(i => i.Device)
                        .OrderByDescending(o => o.Id)
                        .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "81");
                }

                if (deviceMessage82 == null)
                {
                    deviceMessage82 = _context.Messages.AsNoTracking()
                        .Include(i => i.Device)
                        .OrderByDescending(o => o.Id)
                        .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "82");
                }

                if (deviceMessage81 == null)
                    return null;

                return CreateDashboard_Pack81_82ViewModel(deviceMessage81, deviceMessage82, deviceRegistration);
            }
            
            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B982_S).ToLower()) // Uplink (antigo)
            {
                Models.Message deviceMessage83 = null;

                if (time == 0)
                {
                    if (date == null)
                    {
                        deviceMessage83 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "83");
                    }
                    else
                    {
                        date = date.Value.AddHours(3);
                        deviceMessage83 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "83" &&
                                    w.OperationDate.Value.Year == date.Value.Year &&
                                    w.OperationDate.Value.Month == date.Value.Month &&
                                    w.OperationDate.Value.Day == date.Value.Day &&
                                    w.OperationDate.Value.Hour == date.Value.Hour &&
                                    w.OperationDate.Value.Minute == date.Value.Minute);

                        if (deviceMessage83 == null)
                        {
                            deviceMessage83 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "83" &&
                                    w.OperationDate.Value < date.Value);
                        }
                    }
                }
                else
                {
                    bool nextNavigation = navigation.Equals("next");

                    if (nextNavigation)
                    {
                        deviceMessage83 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderBy(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "83" && w.Time > time);
                    }
                    else
                    {
                        deviceMessage83 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderByDescending(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "83" && w.Time < time);
                    }
                }

                if (deviceMessage83 == null)
                {
                    deviceMessage83 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "83");
                }

                if (deviceMessage83 == null)
                    return null;

                // testes
                // deviceMessage83.Data = "836EFEB54078CD914201312E";

                return CreateDashboard_Pack83ViewModel(deviceMessage83, deviceRegistration);
            }
            
            if (deviceRegistration.Package.Type.Equals("21")) // TRM-10 (P965U1)
            {
                Models.Message deviceMessage21 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "21");

                if (deviceMessage21 == null)
                    return null;

                return CreateDashboard_Pack21ViewModel(deviceMessage21, deviceRegistration);
            }

            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B980).ToLower()) // TRM-10 (P965U1)
            {
                Models.Message deviceMessage31 = null;

                if (time == 0)
                {
                    if (date == null)
                    {
                        deviceMessage31 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.OperationDate)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.StartsWith("31"));
                    }
                    else
                    {
                        date = date.Value.AddHours(3);
                        deviceMessage31 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.OperationDate)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.StartsWith("31") &&
                                    w.OperationDate.Value.Year == date.Value.Year &&
                                    w.OperationDate.Value.Month == date.Value.Month &&
                                    w.OperationDate.Value.Day == date.Value.Day &&
                                    w.OperationDate.Value.Hour == date.Value.Hour &&
                                    w.OperationDate.Value.Minute == date.Value.Minute);

                        if (deviceMessage31 == null)
                        {
                            deviceMessage31 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.OperationDate)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.StartsWith("31") &&
                                    w.OperationDate.Value < date.Value);
                        }
                    }
                }
                else
                {
                    bool nextNavigation = navigation.Equals("next");

                    if (nextNavigation)
                    {
                        deviceMessage31 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderBy(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.StartsWith("31") && w.Time > time);
                    }
                    else
                    {
                        deviceMessage31 = _context.Messages.AsNoTracking()
                            .Include(i => i.Device)
                            .OrderByDescending(o => o.OperationDate)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.StartsWith("31") && w.Time < time);
                    }
                }

                if (deviceMessage31 == null)
                {
                    deviceMessage31 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.OperationDate)
                                    .FirstOrDefault(w => w.DeviceId == id && w.Data.StartsWith("31"));
                }

                if (deviceMessage31 == null)
                    return null;

                return CreateDashboard_Pack21ViewModel(deviceMessage31, deviceRegistration);
            }

            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B975).ToLower()) // TRM-10 (P965U1)
            {
                bool nextNavigation = navigation.Equals("next");

                return CreateDashboardB975(deviceRegistration, nextNavigation, (int)time, date);
            }

            if (project.ToLower() == Utils.EnumToAnnotationText(ProjectCode.B972).ToLower())
            {
                B972 b972 = null;
                if (time == 0)
                {
                    b972 = _context.B972s.AsNoTracking().OrderByDescending(o => o.Time).FirstOrDefault(c => c.DeviceId == id);
                }
                else
                {
                    bool nextNavigation = navigation.Equals("next");
                    if (nextNavigation)
                    {
                        b972 = _context.B972s.AsNoTracking()
                            .OrderBy(o => o.Time)
                            .FirstOrDefault(w => w.DeviceId == id && w.Time > time);
                    }
                    else
                    {
                        b972 = _context.B972s.AsNoTracking()
                            .OrderByDescending(o => o.Time)
                            .FirstOrDefault(w => w.DeviceId == id && w.Time < time);
                    }
                }

                if (b972 == null)
                    b972 = _context.B972s.AsNoTracking().OrderByDescending(o => o.Time).FirstOrDefault(c => c.DeviceId == id);

                if (b972 == null)
                    return null;

                return CreateDashboard_B972ViewModel(b972, deviceRegistration);
            }

            return null;
        }

        private DashboardViewModels CreateDashboard_Pack12ViewModel(Models.Message deviceMessage_12, Models.Message deviceMessage_13, DeviceRegistration deviceRegistration)
        {
            DashboardViewModels dashboard = new DashboardViewModels();
            dashboard.DeviceId = deviceMessage_12.DeviceId;
            dashboard.Name = deviceMessage_12.Device.Name;
            dashboard.Package = deviceMessage_12.Data;
            dashboard.TypePackage = deviceMessage_12.TypePackage;
            dashboard.Date = deviceMessage_12.Date;
            dashboard.Country = deviceMessage_12.Country;
            dashboard.Lqi = deviceMessage_12.Lqi;
            dashboard.Bits = deviceMessage_12.Bits;
            dashboard.Time = deviceMessage_12.Time;
            if (deviceRegistration != null)
            {
                dashboard.SerialNumber = deviceRegistration.SerialNumber;
                dashboard.Model = deviceRegistration.Model;
            }

            // set location on dashboard of device
            DeviceLocation deviceLocation = GetLastDeviceLocationByDeviceId(dashboard.DeviceId);
            if (deviceLocation != null)
            {
                dashboard.Latitude = deviceLocation.Latitude.ToString();
                dashboard.Longitude = deviceLocation.Longitude.ToString();
                dashboard.Radius = deviceLocation.Radius;

                dashboard.LatitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Latitude, "S");
                dashboard.LongitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Longitude, "W");
                dashboard.RadiusConverted = RadiusFormated(deviceLocation.Radius);

                dashboard.LocationCity = GetCityByCoordinates(deviceLocation.Latitude, deviceLocation.Longitude);
            }

            // converter os bits deste
            dashboard.EstadoDetector = deviceMessage_12.EstadoDetector;
            dashboard.PeriodoTransmissao = deviceMessage_12.PeriodoTransmissao;
            dashboard.AlertaFonteBaixa = deviceMessage_12.AlertaFonteBaixa;

            dashboard.ContadorCarencias = deviceMessage_12.ContadorCarencias;
            dashboard.ContadorBloqueios = deviceMessage_12.ContadorBloqueios;

            var firstCaracter = deviceMessage_13.Temperature.Substring(0, deviceMessage_13.Temperature.Length - 1);
            var lastCaracter = deviceMessage_13.Temperature.Substring(deviceMessage_13.Temperature.Length - 1, 1);
            dashboard.Temperature = $"{firstCaracter},{lastCaracter}";

            // dashboard.Alimentacao = deviceMessage_13.Alimentacao;
            if (deviceMessage_13.Alimentacao == "0")
            {
                if (deviceMessage_13.AlimentacaoH != "0")
                    dashboard.Alimentacao = $"{deviceMessage_13.AlimentacaoL}{deviceMessage_13.AlimentacaoH}";
                else
                    dashboard.Alimentacao = deviceMessage_13.AlimentacaoL.Contains(",") ? $"{deviceMessage_13.AlimentacaoL}" : $"{deviceMessage_13.AlimentacaoL},0";
            }
            else
                dashboard.Alimentacao = deviceMessage_13.Alimentacao.Contains(",") ? $"{deviceMessage_13.Alimentacao}" : $"{deviceMessage_13.Alimentacao},0";

            if (deviceRegistration.DataDownloadLink != null)
            {
                dashboard.Envio = deviceRegistration.Envio;
                dashboard.PeriodoTransmissao = deviceRegistration.PeriodoTransmissao;
                dashboard.Bits.BaseTempoUpLink = deviceRegistration.BaseTempoUpLink;
                dashboard.TensaoMinima = deviceRegistration.TensaoMinima;
            }

            return dashboard;
        }


        private DashboardViewModels CreateDashboard_Pack23_24ViewModel(Models.Message deviceMessage, Models.Message deviceMessage24, DeviceRegistration deviceRegistration = null)
        {
            DashboardViewModels dashboard = new DashboardViewModels();
            dashboard.DeviceId = deviceMessage.DeviceId;
            dashboard.Name = deviceMessage.Device?.Name;
            dashboard.Package = deviceMessage.Data;
            dashboard.TypePackage = deviceMessage.TypePackage;
            dashboard.Date = deviceMessage.Date;
            dashboard.Country = deviceMessage.Country;
            dashboard.Lqi = deviceMessage.Lqi;
            dashboard.Bits = deviceMessage.Bits;
            dashboard.SeqNumber = deviceMessage.SeqNumber;
            dashboard.Time = deviceMessage.Time;
            if (deviceRegistration != null)
            {
                dashboard.SerialNumber = deviceRegistration.SerialNumber;
                dashboard.Model = deviceRegistration.Model;
            }

            var _fluxoAgua = Utils.FromFloatSafe(deviceMessage.FluxoAgua);
            var _consumoAgua = Utils.FromFloatSafe(deviceMessage.ConsumoAgua);

            dashboard.FluxoAgua = String.Format("{0:0.0}", _fluxoAgua);
            dashboard.ConsumoAgua = String.Format("{0:0.0}", _consumoAgua); // consumo hora

            var _display = Consts.GetDisplayTRM10(dashboard.Bits.BAlertaMax, dashboard.Bits.ModoFechado, dashboard.Bits.ModoAberto);
            dashboard.Modo = _display.DisplayModo; // modo
            dashboard.Estado = _display.DisplayEstado; // alerta
            dashboard.EstadoImage = _display.EstadoImage;
            dashboard.ModoImage = _display.ModoImage;
            dashboard.Valvula = _display.DisplayValvula;
            dashboard.EstadoColor = _display.EstadoColor;


            if (deviceMessage24 == null)
                return dashboard;

            var _consumoDia = Utils.FromFloatSafe(deviceMessage24.ConsumoDia);
            var _consumoSemana = Utils.FromFloatSafe(deviceMessage24.ConsumoSemana);

            dashboard.ConsumoDia = String.Format("{0:0,0}", _consumoDia);
            dashboard.ConsumoSemana = String.Format("{0:0,0}", _consumoSemana);
            dashboard.ConsumoMes = String.Format("{0:0,0}", deviceMessage24.ConsumoMes);

            return dashboard;
        }

        private DashboardViewModels CreateDashboard_B972ViewModel(Models.B972 b972, DeviceRegistration deviceRegistration)
        {
            var device = _context.DevicesRegistration.SingleOrDefault(c => c.DeviceId == b972.DeviceId);
            var partial = _context.ResetTotalPartials.SingleOrDefault(c => c.DeviceId == b972.DeviceId);

            DashboardViewModels dashboard = new DashboardViewModels();
            dashboard.DeviceId = b972.DeviceId;
            dashboard.Name = device.Name;
            dashboard.Date = b972.DateGMTBrasilian;
            dashboard.Lqi = b972.Lqi;
            dashboard.Time = b972.Time;
            if (deviceRegistration != null)
            {
                dashboard.SerialNumber = deviceRegistration.SerialNumber;
                dashboard.Model = deviceRegistration.Model;
            }

            dashboard.Flow = String.Format("{0:0.0}", b972.Flow != null ? b972.Flow.Value : 0);
            dashboard.Velocity = String.Format("{0:0.0}", b972.Velocity != null ? b972.Velocity.Value : 0);

            decimal currentTotal = b972.Total != null ? b972.Total.Value : 0;
            dashboard.Total = String.Format("{0:0.0}", currentTotal);
            
            if (partial == null)
                dashboard.Partial = String.Format("{0:0.0}", b972.Partial != null ? b972.Partial.Value : 0);
            else
            {
                decimal currentPartial = currentTotal - partial.LastValue;
                dashboard.Partial = String.Format("{0:0.0}", currentPartial);
            }

            dashboard.Temperature = String.Format("{0:0.0}", b972.Temperature != null ? b972.Temperature.Value : 0);

            dashboard.Flags = b972.Flags != null ? b972.Flags : "00000000";
            dashboard.Quality = String.Format("{0:0.0}", b972.Quality != null ? b972.Quality.Value : 0);

            dashboard.Bits = new ViewModels.Bits()
            {
                AlertaQualidade = Convert.ToBoolean(int.Parse(dashboard.Flags.Substring(2, 1))),
                AlertaTotalizador = Convert.ToBoolean(int.Parse(dashboard.Flags.Substring(3, 1))),
                AlertaVazao = Convert.ToBoolean(int.Parse(dashboard.Flags.Substring(4, 1))),
                Qualidade = Convert.ToBoolean(int.Parse(dashboard.Flags.Substring(5, 1))),
                Totalizador = Convert.ToBoolean(int.Parse(dashboard.Flags.Substring(6, 1))),
                Vazao = Convert.ToBoolean(int.Parse(dashboard.Flags.Substring(7, 1)))
            };

            return dashboard;
        }

        private DashboardViewModels CreateDashboardMCond(DeviceRegistration deviceRegistration, bool nextNavigation, int time, DateTime? date)
        {
            DashboardViewModels dashboard = new DashboardViewModels();
            MCond mCond = null;

            if (time == 0)
            {
                if (!date.HasValue)
                {
                    mCond = _context.MConds.LastOrDefault(c => c.DeviceId == deviceRegistration.DeviceId);
                }
                else
                {
                    date = date.Value.AddHours(-3);

                    // mCond = _context.MConds.LastOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.PackPort != null && c.PackInf != null && c.PackSup != null);
                    mCond = _context.MConds.FirstOrDefault(c => c.DeviceId == deviceRegistration.DeviceId &&
                                    c.Date.Year == date.Value.Year &&
                                    c.Date.Month == date.Value.Month &&
                                    c.Date.Day == date.Value.Day &&
                                    c.Date.Hour == date.Value.Hour &&
                                    c.Date.Minute == date.Value.Minute);
                    
                    if (mCond == null)
                    {
                        mCond = _context.MConds.FirstOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.Date >= date.Value);
                    }

                    if (mCond == null)
                    {
                        mCond = _context.MConds.FirstOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.Date <= date.Value);
                    }
                }
            }
            else
            {
                if (nextNavigation)
                {
                    time++;
                    mCond = _context.MConds.FirstOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.Id >= time);
                }
                else
                {
                    time--;
                    mCond = _context.MConds.LastOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.Id <= time);
                }
            }

            if (mCond == null)
                mCond = _context.MConds.LastOrDefault(c => c.DeviceId == deviceRegistration.DeviceId);
            
            dashboard.DeviceId = deviceRegistration.DeviceId;
            dashboard.Name = deviceRegistration.Name;
            dashboard.Package = mCond != null ? $"{mCond.PackPort};{mCond.PackInf};{mCond.PackSup}" : string.Empty;
            dashboard.TypePackage = ((int)ProjectCode.B987).ToString();
            dashboard.Date = mCond != null ? mCond.Date : DateTime.Today;
            dashboard.Country = "BRA";
            dashboard.Lqi = deviceRegistration.Device.Lqi;
            dashboard.SeqNumber = deviceRegistration.Device.SequenceNumber;
            dashboard.Time = mCond != null ? mCond.Time : 0;
            dashboard.MCond = mCond;

            return dashboard;
        }

        private DashboardViewModels CreateDashboardB975(DeviceRegistration deviceRegistration, bool nextNavigation, int time, DateTime? date)
        {
            DashboardViewModels dashboard = new DashboardViewModels();
            B975 b975 = null;

            if (time == 0)
            {
                if (!date.HasValue)
                {
                    b975 = _context.B975s.LastOrDefault(c => c.DeviceId == deviceRegistration.DeviceId);
                }
                else
                {
                    date = date.Value.AddHours(-3);

                    // mCond = _context.MConds.LastOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.PackPort != null && c.PackInf != null && c.PackSup != null);
                    b975 = _context.B975s.FirstOrDefault(c => c.DeviceId == deviceRegistration.DeviceId &&
                                    c.Date.Year == date.Value.Year &&
                                    c.Date.Month == date.Value.Month &&
                                    c.Date.Day == date.Value.Day &&
                                    c.Date.Hour == date.Value.Hour &&
                                    c.Date.Minute == date.Value.Minute);
                    
                    if (b975 == null)
                    {
                        b975 = _context.B975s.FirstOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.Date >= date.Value);
                    }

                    if (b975 == null)
                    {
                        b975 = _context.B975s.FirstOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.Date <= date.Value);
                    }
                }
            }
            else
            {
                if (nextNavigation)
                {
                    time++;
                    b975 = _context.B975s.FirstOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.Id >= time);
                }
                else
                {
                    time--;
                    b975 = _context.B975s.LastOrDefault(c => c.DeviceId == deviceRegistration.DeviceId && c.Id <= time);
                }
            }

            if (b975 == null)
                b975 = _context.B975s.LastOrDefault(c => c.DeviceId == deviceRegistration.DeviceId);

            if (b975 == null)
                return null;
            
            dashboard.DeviceId = deviceRegistration.DeviceId;
            dashboard.Name = deviceRegistration.Name;
            dashboard.Package = $"{b975.PackA};{b975.PackB};{b975.PackC}";
            dashboard.TypePackage = ((int)ProjectCode.B987).ToString();
            dashboard.Date = b975.DateGMTBrasilian;
            dashboard.Country = "BRA";
            dashboard.Lqi = b975.Lqi;
            // dashboard.SeqNumber = deviceRegistration.Device.SequenceNumber;
            // dashboard.Time = mCond != null ? mCond.Time : 0;
            dashboard.B975 = b975;

            if (deviceRegistration != null)
            {
                dashboard.SerialNumber = deviceRegistration.SerialNumber;
                dashboard.Model = deviceRegistration.Model;
                dashboard.Notes = deviceRegistration.Notes;
                dashboard.NotesCreateDate = deviceRegistration.NotesCreateDate.HasValue ? deviceRegistration.NotesCreateDate.Value.ToShortDateString() : null;
            }

            return dashboard;
        }


        private DashboardViewModels CreateDashboard_Pack21ViewModel(Models.Message deviceMessage, DeviceRegistration deviceRegistration)
        {
            DeviceLocation deviceLocation = GetDeviceLocationByTime(deviceMessage.DeviceId, deviceMessage.Time);

            // pacotes 21 e 31 tem a mesma estrutura
            DashboardViewModels dashboard = new DashboardViewModels();
            dashboard.DeviceId = deviceMessage.DeviceId;
            dashboard.Name = deviceMessage.Device.Name;
            dashboard.Package = deviceMessage.Data;
            dashboard.TypePackage = deviceMessage.TypePackage;
            // dashboard.Date = deviceMessage.Date;
            dashboard.Country = deviceMessage.Country;
            dashboard.Lqi = deviceMessage.Lqi;
            dashboard.Bits = deviceMessage.Bits;
            dashboard.SeqNumber = deviceMessage.SeqNumber;
            dashboard.Time = deviceMessage.Time;
            dashboard.Date = deviceMessage.OperationDate.Value.AddHours(-3);
            if (deviceRegistration != null)
            {
                dashboard.SerialNumber = deviceRegistration.SerialNumber;
                dashboard.Model = deviceRegistration.Model;
                dashboard.Notes = deviceRegistration.Notes;
                dashboard.NotesCreateDate = deviceRegistration.NotesCreateDate.HasValue ? deviceRegistration.NotesCreateDate.Value.ToShortDateString() : null;
                dashboard.Ed1 = deviceRegistration.Ed1;
                dashboard.Ed2 = deviceRegistration.Ed2;
                dashboard.Ed3 = deviceRegistration.Ed3;
                dashboard.Ed4 = deviceRegistration.Ed4;
                dashboard.Sd1 = deviceRegistration.Sd1;
                dashboard.Sd2 = deviceRegistration.Sd2;
                dashboard.Ea10 = deviceRegistration.Ea10;
                dashboard.Sa3 = deviceRegistration.Sa3;
            }

            if (deviceLocation != null)
            {
                dashboard.Latitude = deviceLocation.Latitude.ToString();
                dashboard.Longitude = deviceLocation.Longitude.ToString();
                dashboard.Radius = deviceLocation.Radius;
                if (deviceLocation.City != null)
                    dashboard.LocationCity = $"{deviceLocation.Neighborhood} - {deviceLocation.City}/{deviceLocation.State}";
                else
                    dashboard.LocationCity = "Não encontrado";
            }

            var _entradaAnalogica = Utils.FromFloatSafe(deviceMessage.EntradaAnalogica);
            var _saidaAnalogica = Utils.FromFloatSafe(deviceMessage.SaidaAnalogica);

            dashboard.EntradaAnalogica = String.Format("{0:0.00}", _entradaAnalogica);
            dashboard.SaidaAnalogica = String.Format("{0:0.0}", _saidaAnalogica);

            return dashboard;
        }

        private DashboardViewModels CreateDashboard_Pack83ViewModel(Models.Message deviceMessage, DeviceRegistration deviceRegistration)
        {
            var partial = _context.ResetTotalPartials.SingleOrDefault(c => c.DeviceId == deviceMessage.DeviceId);

            DashboardViewModels dashboard = new DashboardViewModels();
            dashboard.DeviceId = deviceMessage.DeviceId;
            dashboard.Name = deviceMessage.Device.Name;
            dashboard.Time = deviceMessage.Time;
            dashboard.Package = deviceMessage.Data;
            dashboard.TypePackage = deviceMessage.TypePackage;
            dashboard.Date = deviceMessage.Date;
            dashboard.Country = deviceMessage.Country;
            dashboard.Lqi = deviceMessage.Lqi;
            dashboard.Bits = deviceMessage.Bits;
            dashboard.SeqNumber = deviceMessage.SeqNumber;
            if (deviceRegistration != null)
            {
                dashboard.SerialNumber = deviceRegistration.SerialNumber;
                dashboard.Model = deviceRegistration.Model;
            }

            // set location on dashboard of device
            // DeviceLocation deviceLocation = GetDeviceLocationByDeviceId(dashboard.DeviceId);
            // if (deviceLocation != null)
            // {
            //     dashboard.Latitude = deviceLocation.Latitude.ToString();
            //     dashboard.Longitude = deviceLocation.Longitude.ToString();
            //     dashboard.Radius = deviceLocation.Radius;

            //     dashboard.LatitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Latitude, "S");
            //     dashboard.LongitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Longitude, "W");
            //     dashboard.RadiusConverted = RadiusFormated(deviceLocation.Radius);
            // }


            double _vazao = 0;
            if (deviceMessage.Operator != null)
            {
                dashboard.Vazao = deviceMessage.Operator.Replace(".", ",");
                dashboard.Date = Utils.TimeStampToDateTime(deviceMessage.Time);
            }
            else
            {
                _vazao = Utils.FromFloatSafe(deviceMessage.Vazao);
                dashboard.Vazao = String.Format("{0:0.000}", _vazao);
            }

            var _totalizacao = Utils.FromFloatSafe(deviceMessage.Totalizacao); // total
            dashboard.Totalizacao = String.Format("{0:0}", _totalizacao);
            dashboard.Calha = deviceMessage.Calha;
            dashboard.CalhaImage = deviceMessage.CalhaImage;
            dashboard.CalhaAlerta = deviceMessage.CalhaAlerta;


            if (partial == null)
                dashboard.Partial = String.Format("{0:0}", _totalizacao);
            else
            {
                decimal currentPartial = (decimal)_totalizacao - partial.LastValue;
                dashboard.Partial = String.Format("{0:0}", currentPartial < 0 ? 0 : currentPartial);
            }

            return dashboard;
        }

        private DashboardViewModels CreateDashboard_Pack10ViewModel(Models.Message deviceMessage, DeviceRegistration deviceRegistration)
        {
            DashboardViewModels dashboard = new DashboardViewModels();
            dashboard.DeviceId = deviceMessage.DeviceId;
            dashboard.Name = deviceMessage.Device.Name;
            dashboard.Package = deviceMessage.Data;
            dashboard.TypePackage = deviceMessage.TypePackage;
            dashboard.Date = deviceMessage.Date;
            dashboard.Country = deviceMessage.Country;
            dashboard.Lqi = deviceMessage.Lqi;
            dashboard.Bits = deviceMessage.Bits;
            dashboard.SeqNumber = deviceMessage.SeqNumber;
            dashboard.Time = deviceMessage.Time;
            if (deviceRegistration != null)
            {
                dashboard.SerialNumber = deviceRegistration.SerialNumber;
                dashboard.Model = deviceRegistration.Model;
            }

            // set location on dashboard of device
            DeviceLocation deviceLocation = GetLastDeviceLocationByDeviceId(dashboard.DeviceId);
            if (deviceLocation != null)
            {
                dashboard.Latitude = deviceLocation.Latitude.ToString();
                dashboard.Longitude = deviceLocation.Longitude.ToString();
                dashboard.Radius = deviceLocation.Radius;

                dashboard.LatitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Latitude, "S");
                dashboard.LongitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Longitude, "W");
                dashboard.RadiusConverted = RadiusFormated(deviceLocation.Radius);
            }

            var pack53DownloadLink = GetPack53DownloadLink(dashboard.DeviceId);
            if (pack53DownloadLink != null)
            {
                dashboard.DownloadLink = new DownloadLink() {
                    NumeroEnvios = Convert.ToInt32(pack53DownloadLink.Envio),
                    TempoTransmissao = Convert.ToInt32(pack53DownloadLink.PeriodoTransmissao),
                    TipoEnvio = pack53DownloadLink.Bits.TipoEnvio
                };
            }

            dashboard.Temperature = (decimal.Parse(deviceMessage.Temperature) * 100).ToString();
            dashboard.Temperature = dashboard.Temperature.ToString().Substring(0, dashboard.Temperature.Length - 2);
            dashboard.Envio = deviceMessage.Envio;
            dashboard.PeriodoTransmissao = deviceMessage.PeriodoTransmissao;
            dashboard.Alimentacao = $"{deviceMessage.Alimentacao},0";
            dashboard.AlimentacaoMinima = $"{deviceMessage.AlimentacaoMinima},0";

            return dashboard;
        }

        private DashboardViewModels CreateDashboard_Pack22_23ViewModel(Models.Message deviceMessageType22, Models.Message deviceMessageType23, DeviceRegistration deviceRegistration = null)
        {
            DashboardViewModels dashboard = new DashboardViewModels();
            dashboard.DeviceId = deviceMessageType22.DeviceId;
            dashboard.Name = deviceMessageType22.Device.Name;
            dashboard.Package = $"{deviceMessageType22.Data};{deviceMessageType23.Data}";
            dashboard.TypePackage = $"{deviceMessageType22.TypePackage};{deviceMessageType23.TypePackage}";
            dashboard.Date = deviceMessageType22.Date;
            dashboard.Country = deviceMessageType22.Country;
            dashboard.Lqi = deviceMessageType22.Lqi;
            dashboard.Bits = deviceMessageType22.Bits;
            dashboard.Level = deviceMessageType22.Level;
            dashboard.Light = deviceMessageType22.Light;
            dashboard.Temperature = deviceMessageType22.Temperature;
            dashboard.Moisture = deviceMessageType22.Moisture;
            dashboard.OxigenioDissolvido = deviceMessageType22.OxigenioDissolvido;
            dashboard.Ph = deviceMessageType23.Ph;
            dashboard.Condutividade = deviceMessageType23.Condutividade;
            dashboard.PeriodoTransmissao = deviceMessageType23.PeriodoTransmissao;
            dashboard.BaseT = deviceMessageType23.BaseT;
            dashboard.Time = deviceMessageType23.Time;
            if (deviceRegistration != null)
            {
                dashboard.SerialNumber = deviceRegistration.SerialNumber;
                dashboard.Model = deviceRegistration.Model;
            }

            // set location on dashboard of device
            DeviceLocation deviceLocation = GetLastDeviceLocationByDeviceId(dashboard.DeviceId);
            if (deviceLocation != null)
            {
                dashboard.Latitude = deviceLocation.Latitude.ToString();
                dashboard.Longitude = deviceLocation.Longitude.ToString();
                dashboard.Radius = deviceLocation.Radius;

                dashboard.LatitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Latitude, "S");
                dashboard.LongitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Longitude, "W");
                dashboard.RadiusConverted = RadiusFormated(deviceLocation.Radius);
            }

            return dashboard;
        }

        private DashboardViewModels CreateDashboard_Pack81_82ViewModel(Models.Message deviceMessageType81, Models.Message deviceMessageType82, DeviceRegistration deviceRegistration)
        {
            DashboardViewModels dashboard = new DashboardViewModels();
            dashboard.DeviceId = deviceMessageType81.DeviceId;
            dashboard.Name = deviceMessageType81.Device.Name;
            if (deviceMessageType82 != null)
                dashboard.Package = $"{deviceMessageType81.Data};{deviceMessageType82.Data}";
            else
                dashboard.Package = $"{deviceMessageType81.Data}";

            if (deviceMessageType82 != null)
                dashboard.TypePackage = $"{deviceMessageType81.TypePackage};{deviceMessageType82.TypePackage}";
            else
                dashboard.TypePackage = $"{deviceMessageType81.TypePackage}";
            
            if (deviceRegistration != null)
            {
                dashboard.SerialNumber = deviceRegistration.SerialNumber;
                dashboard.Model = deviceRegistration.Model;
            }

            dashboard.Date = deviceMessageType81.Date;
            dashboard.Country = deviceMessageType81.Country;
            dashboard.Lqi = deviceMessageType81.Lqi;
            
            dashboard.SeqNumber = deviceMessageType81.SeqNumber;
            // dashboard.SeqNumberb = deviceMessageType82.SeqNumber;
            dashboard.SeqNumberb = deviceMessageType82.Time;
            
            dashboard.Time = deviceMessageType81.Time;

            dashboard.Temperature = deviceMessageType81.Temperature == "0" ? "0,00" : deviceMessageType81.Temperature;
            dashboard.Ph = deviceMessageType81.Ph == "0" ? "0,00" : deviceMessageType81.Ph;

            dashboard.Fluor = deviceMessageType81.Fluor;
            dashboard.Cloro = deviceMessageType81.Cloro == ",00" ? "0,00" : deviceMessageType81.Cloro;
            dashboard.Turbidez = deviceMessageType81.Turbidez;
            dashboard.DownloadLink = new DownloadLink();


            var pack53DownloadLink = GetPack53DownloadLink(dashboard.DeviceId);
            if (pack53DownloadLink != null)
            {
                dashboard.DownloadLink.NumeroEnvios = Convert.ToInt32(pack53DownloadLink.Envio);
                dashboard.DownloadLink.TempoTransmissao = Convert.ToInt32(pack53DownloadLink.PeriodoTransmissao);
                dashboard.DownloadLink.TipoEnvio = pack53DownloadLink.Bits.TipoEnvio;
            }

            if (deviceRegistration.DataDownloadLink != null)
            {
                dashboard.DownloadLink.NumeroEnvios = Convert.ToInt32(deviceRegistration.Envio);
                dashboard.DownloadLink.TempoTransmissao = Convert.ToInt32(deviceRegistration.PeriodoTransmissao);
                dashboard.DownloadLink.TipoEnvio = deviceRegistration.BaseTempoUpLink;
                dashboard.DownloadLink.TensaoMinima = deviceRegistration.TensaoMinima;
            }

            if (deviceMessageType82 != null)
            {
                dashboard.ReleBoolean = deviceMessageType82.ReleBoolean;
            }

            // set location on dashboard of device
            DeviceLocation deviceLocation = GetLastDeviceLocationByDeviceId(dashboard.DeviceId);
            if (deviceLocation != null)
            {
                dashboard.Latitude = deviceLocation.Latitude.ToString();
                dashboard.Longitude = deviceLocation.Longitude.ToString();
                dashboard.Radius = deviceLocation.Radius;

                dashboard.LatitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Latitude, "S");
                dashboard.LongitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Longitude, "W");
                dashboard.RadiusConverted = RadiusFormated(deviceLocation.Radius);
            }

            return dashboard;
        }

        internal Message GetPack53DownloadLink(string deviceId)
        {
            return _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                    .FirstOrDefault(w => w.DeviceId == deviceId && w.Data.Substring(0, 2) == "53");
        }

        public void CleanPartial(string deviceId, decimal partial, ClaimsPrincipal user)
        {
            var oldPartial = _context.ResetTotalPartials.SingleOrDefault(c => c.DeviceId == deviceId);
            var _user = _securityContext.Users.AsNoTracking().SingleOrDefault(w => w.Id == user.GetId());

            ResetTotalPartial resetTotalPartial = new ResetTotalPartial()
            {
                DeviceId = deviceId,
                Date = DateTime.UtcNow.AddHours(-3),
                LastValue = partial,
                EmailUser = _user.Email
            };

            if (oldPartial == null)
            {
                _context.Entry<Models.ResetTotalPartial>(resetTotalPartial).State = EntityState.Added;
            }
            else
            {
                resetTotalPartial.LastValue = partial;
                _context.ResetTotalPartials.Attach(oldPartial);
                _context.Entry<Models.ResetTotalPartial>(oldPartial).CurrentValues.SetValues(resetTotalPartial);
            }

            _context.SaveChanges(true);
            _log.Log($"Reset do total parcial do dispositivo {deviceId} efetuado pelo usuário {_user.Email}.");

        }

        

    }
}