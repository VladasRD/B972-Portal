using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Box.Security.Services;
using SmartGeoIot.Data;
using SmartGeoIot.Services;
using System;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace SmartGeoIot.Controllers
{
    [Route("/[controller]")]
    public class SigfoxCallbackController : Controller
    {
        private readonly SecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly SmartGeoIotContext _context;
        private LogService _log { get; set; }
        SmartGeoIotService _sgiService;
        private readonly SmartGeoIot.SgiSettings _sgiSettings;

        public SigfoxCallbackController(
            SecurityService securityService,
            IConfiguration configuration,
            SmartGeoIotContext context,
            LogService log,
            IOptions<SmartGeoIot.SgiSettings> sgiSettings,
            SmartGeoIotService sgiService)
        {
            _securityService = securityService;
            _configuration = configuration;
            _context = context;
            _log = log;
            _sgiSettings = sgiSettings.Value;
            _securityService = securityService;
            _sgiService = sgiService;
        }

        [HttpPost]
        public void Post(string deviceTypeId, [FromBody] Models.CallBackDataAdvanced callBackDataAdvanced)
        {
            string callBackDataAdvancedJSON = JsonConvert.SerializeObject(callBackDataAdvanced);

            Models.CallBackMessageValue computedLocation = null;
            if (callBackDataAdvanced.messages != null && callBackDataAdvanced.messages.Length > 1)
                computedLocation = JsonConvert.DeserializeObject<Models.CallBackMessageValue>(callBackDataAdvanced.messages[1].value.ToString());

            if (computedLocation != null)
            {
                Models.DeviceLocation deviceLocation = new Models.DeviceLocation
                {
                    DeviceLocationUId = Guid.NewGuid().ToString(),
                    DeviceId = callBackDataAdvanced?.device,
                    Data = callBackDataAdvanced?.data,
                    Time = (long)callBackDataAdvanced?.timestamp,
                    Latitude = double.Parse(computedLocation?.lat),
                    Longitude = double.Parse(computedLocation?.lng),
                    Radius = computedLocation?.radius
                };
                // Salvar o registro de localização do dispositivo
                _sgiService.SaveDeviceLocation(deviceLocation);

                // Verifica e notifica se houve bloqueio no dispositivo
                _sgiService.VerifyIsLockOnDeviceDJRF(deviceLocation.Data, deviceLocation.DeviceId, deviceLocation.Time);
            }

            // atualizar dados deste dispositivo
            try
            {
                _sgiService.DownloadMessageByDevice(callBackDataAdvanced?.device);
            }
            catch (System.Exception ex)
            {
                _log.Log($"Erro ao atualizar dados do disposito {callBackDataAdvanced?.device} quando detectado bloqueio. Erro: {ex.Message}.");
            }

            _log.Log("Método POST de call-back da sigfox de localização do dispositivo retornou " +
            $"id:{callBackDataAdvanced?.device}, time:{callBackDataAdvanced?.timestamp}, data: {callBackDataAdvanced?.data}, deviceTypeId:{deviceTypeId?.ToString()}, " +
            $"latitude: {computedLocation?.lat}, longitude: {computedLocation?.lng}, radius: {computedLocation?.radius}.");
        }

    }

}