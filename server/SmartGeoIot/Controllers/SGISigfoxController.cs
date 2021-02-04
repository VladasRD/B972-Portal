using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Box.Security.Services;
using Box.Common.Services;
using SmartGeoIot.Data;
using SmartGeoIot.Services;

namespace SmartGeoIot.Controllers
{
    public class SGISigfoxController : Controller
    {
        private readonly SecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly SmartGeoIotContext _context;
        private LogService _log { get; set; }
        private readonly IEmailSender _emailSender;
        SmartGeoIotService _sgiService;

        public SGISigfoxController(
            SecurityService securityService,
            IConfiguration configuration,
            SmartGeoIotContext context,
            LogService log,
            IEmailSender emailSender,
            SmartGeoIotService sgiService)
        {
            _securityService = securityService;
            _configuration = configuration;
            _context = context;
            _log = log;
            _securityService = securityService;
            _emailSender = emailSender;
            _sgiService = sgiService;
        }

        #region MESSAGES
        public void DownloadMessagesSigfox()
        {
            try
            {
                _log.Log("Iniciando download dos dados dos dispositivos.");

                var devices = _sgiService.GetAllDevices();
                if (devices != null)
                {
                    foreach (var device in devices)
                    {
                        _log.Log($"Baixando dados do dispositivo {device.Id} - {device.Name}.");
                        var messages = _sgiService.SigfoxGetMessagesByDevice(device.Id);
                        if (messages.data.Length > 0)
                            _sgiService.SigfoxSaveMessages(messages);
                    }
                }

                _log.Log("Finalizado download dos dados dos dispositivos.");
            }
            catch (System.Exception ex)
            {
                _log.Log($"Erro download dos dados dos dispositivos. Error: {ex.Message}");
            }
        }

        public void DownloadMessagesSigfoxFromDevice(string deviceId)
        {
            try
            {
                _log.Log($"Baixando dados do dispositivo {deviceId}.");

                var messages = _sgiService.SigfoxGetMessagesByDevice(deviceId);
                if (messages.data.Length > 0)
                    _sgiService.SigfoxSaveMessages(messages);

                _log.Log($"Finalizado download de dados do dispositivo {deviceId}.");
            }
            catch (System.Exception ex)
            {
                _log.Log($"Erro download de dados do dispositivo {deviceId}. Error: {ex.Message}");
            }
        }
        #endregion

        #region DEVICES
        public void DownloadDevicesSigfox()
        {
            try
            {
                if (_sgiService.IsLastDayOfMonth())
                {
                    _log.Log("Iniciando verificação de clientes e licensas ativas.");
                    _sgiService.CalcClientsAndLicensesActived();
                    _log.Log("Finalizando verificação de clientes e licensas ativas.");
                }
            }
            catch (System.Exception) { }

            try
            {
                _log.Log("Iniciando download dos dispositivos.");

                var devices = _sgiService.SigfoxGetDevices();
                if (devices.data.Length > 0)
                    _sgiService.SigfoxSaveDevices(devices);

                _log.Log("Finalizado download dos dispositivos.");
            }
            catch (System.Exception ex)
            {
                _log.Log($"Erro download dos dispositivos. Error: {ex.Message}");
            }
        }
        #endregion

        public void SaveLogs(string action, string error = null)
        {
            try
            {
                _log.Log(action, error, true);
            }
            catch (System.Exception) { }
        }
    }
}