using System;
using System.Collections.Generic;
using System.Globalization;
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
        public FirmwareViewModels GetFirmware(string id)
        {
            Models.Message deviceMessage51 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                                .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "51");

            Models.Message deviceMessage52 = _context.Messages.AsNoTracking().Include(i => i.Device).OrderByDescending(o => o.Id)
                            .FirstOrDefault(w => w.DeviceId == id && w.Data.Substring(0, 2) == "52");

            // deviceMessage51.Data = "51090614010102030400007F";
            // deviceMessage52.Data = "5214621500170000000000F4";
            return CreateDashboard_Pack51_52ViewModel(deviceMessage51, deviceMessage52);
        }

        private FirmwareViewModels CreateDashboard_Pack51_52ViewModel(Models.Message deviceMessage_51, Models.Message deviceMessage_52)
        {
            FirmwareViewModels firmwareViewModels = new FirmwareViewModels();
            firmwareViewModels.Pacotes = $"{deviceMessage_51.Data} / {deviceMessage_52.Data}";

            firmwareViewModels.DataCompilacao = deviceMessage_51.DataCompilacao;
            firmwareViewModels.SerieDispositivo = deviceMessage_51.SerieDispositivo;
            firmwareViewModels.Hardware = $"{deviceMessage_51.VersaoHardware}.{deviceMessage_51.RevisaoHardware}";
            firmwareViewModels.Firmware = $"{deviceMessage_51.VersaoFirmware}.{deviceMessage_51.RevisaoFirmware}";

            firmwareViewModels.Proc = deviceMessage_52.Proc;
            firmwareViewModels.Placa = deviceMessage_52.Placa;
            firmwareViewModels.VPlaca = deviceMessage_52.VPlaca;
            firmwareViewModels.NAplic = deviceMessage_52.NAplic;

            return firmwareViewModels;
        }

    }
}