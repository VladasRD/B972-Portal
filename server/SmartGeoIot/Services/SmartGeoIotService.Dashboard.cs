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

                dashboards.Add(CreateDashboard_Pack22_23ViewModel(deviceMessageType22, deviceMessageType23));
            }

            totalCount.Value = dashboards.Count();
            return dashboards.ToArray();
        }

        public DashboardViewModels GetDashboard(string id)
        {
            var deviceRegistration = _context.DevicesRegistration.Include(i => i.Package).SingleOrDefault(r => r.DeviceId == id);

            if (deviceRegistration.Package.Type.Equals("22") || deviceRegistration.Package.Type.Equals("23")) // PROJETO SENSOR DE HIDROPONIA
            {
                Models.Message deviceMessageType22 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "22");

                Models.Message deviceMessageType23 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                    .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "23");

                if (deviceMessageType22 == null || deviceMessageType23 == null)
                    return null;

                return CreateDashboard_Pack22_23ViewModel(deviceMessageType22, deviceMessageType23);
            }
            else if (deviceRegistration.Package.Type.Equals("10")) // PROJETO AGUAMON
            {
                Models.Message deviceMessage = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "10");

                if (deviceMessage == null)
                    return null;

                return CreateDashboard_Pack10ViewModel(deviceMessage);
            }
            else if (deviceRegistration.Package.Type.Equals("12") || deviceRegistration.Package.Type.Equals("13")) // PROJETO DJRF
            {
                Models.Message deviceMessage12 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "12");

                Models.Message deviceMessage13 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "13");

                if (deviceMessage12 == null || deviceMessage13 == null)
                    return null;

                return CreateDashboard_Pack12ViewModel(deviceMessage12, deviceMessage13, deviceRegistration);
            }
            else if (deviceRegistration.Package.Type.Equals("81") || deviceRegistration.Package.Type.Equals("82")) // PQA
            {
                Models.Message deviceMessage81 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "81");

                Models.Message deviceMessage82 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "82");

                if (deviceMessage81 == null)
                    return null;

                return CreateDashboard_Pack81_82ViewModel(deviceMessage81, deviceMessage82);
            }
            else if (deviceRegistration.Package.Type.Equals("83")) // Uplink (antigo)
            {
                Models.Message deviceMessage83 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "83");

                if (deviceMessage83 == null)
                    return null;

                return CreateDashboard_Pack83ViewModel(deviceMessage83);
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

            // set location on dashboard of device
            DeviceLocation deviceLocation = GetDeviceLocationByDeviceId(dashboard.DeviceId);
            if (deviceLocation != null)
            {
                dashboard.Latitude = deviceLocation.Latitude.ToString();
                dashboard.Longitude = deviceLocation.Longitude.ToString();
                dashboard.Radius = deviceLocation.Radius;

                dashboard.LatitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Latitude, "S");
                dashboard.LongitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Longitude, "W");
                dashboard.RadiusConverted = RadiusFormated(deviceLocation.Radius);
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

        private DashboardViewModels CreateDashboard_Pack83ViewModel(Models.Message deviceMessage)
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

            dashboard.Vazao = deviceMessage.Vazao;
            dashboard.TotalizacaoParcial = deviceMessage.TotalizacaoParcial;
            dashboard.Totalizacao = deviceMessage.Totalizacao;
            dashboard.TempoParcial = deviceMessage.TempoParcial;

            return dashboard;
        }

        private DashboardViewModels CreateDashboard_Pack10ViewModel(Models.Message deviceMessage)
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

            // set location on dashboard of device
            DeviceLocation deviceLocation = GetDeviceLocationByDeviceId(dashboard.DeviceId);
            if (deviceLocation != null)
            {
                dashboard.Latitude = deviceLocation.Latitude.ToString();
                dashboard.Longitude = deviceLocation.Longitude.ToString();
                dashboard.Radius = deviceLocation.Radius;

                dashboard.LatitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Latitude, "S");
                dashboard.LongitudeConverted = LocationDecimalToDegrees((decimal)deviceLocation.Longitude, "W");
                dashboard.RadiusConverted = RadiusFormated(deviceLocation.Radius);
            }

            dashboard.Temperature = (decimal.Parse(deviceMessage.Temperature) * 100).ToString();
            dashboard.Temperature = dashboard.Temperature.ToString().Substring(0, dashboard.Temperature.Length - 2);
            dashboard.Envio = deviceMessage.Envio;
            dashboard.PeriodoTransmissao = deviceMessage.PeriodoTransmissao;
            dashboard.Alimentacao = $"{deviceMessage.Alimentacao},0";
            dashboard.AlimentacaoMinima = $"{deviceMessage.AlimentacaoMinima},0";

            return dashboard;
        }

        private DashboardViewModels CreateDashboard_Pack22_23ViewModel(Models.Message deviceMessageType22, Models.Message deviceMessageType23)
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

            // set location on dashboard of device
            DeviceLocation deviceLocation = GetDeviceLocationByDeviceId(dashboard.DeviceId);
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

        private DashboardViewModels CreateDashboard_Pack81_82ViewModel(Models.Message deviceMessageType81, Models.Message deviceMessageType82)
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

            dashboard.Date = deviceMessageType81.Date;
            dashboard.Country = deviceMessageType81.Country;
            dashboard.Lqi = deviceMessageType81.Lqi;

            dashboard.Temperature = deviceMessageType81.Temperature;
            dashboard.Ph = deviceMessageType81.Ph;

            dashboard.Fluor = deviceMessageType81.Fluor;
            dashboard.Cloro = deviceMessageType81.Cloro;
            dashboard.Turbidez = deviceMessageType81.Turbidez;

            if (deviceMessageType82 != null)
            {
                dashboard.Rele = new Rele()
                {
                    Rele1 = Utils.HexaToDecimal(deviceMessageType82.Package.Substring(0, 2)).ToString(),
                    Rele2 = Utils.HexaToDecimal(deviceMessageType82.Package.Substring(2, 2)).ToString(),
                    Rele3 = Utils.HexaToDecimal(deviceMessageType82.Package.Substring(4, 2)).ToString(),
                    Rele4 = Utils.HexaToDecimal(deviceMessageType82.Package.Substring(6, 2)).ToString(),
                    Rele5 = Utils.HexaToDecimal(deviceMessageType82.Package.Substring(8, 2)).ToString()
                };
            }

            return dashboard;
        }
    }
}