using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Box.Security.Services;
using Box.Common.Services;
using SmartGeoIot.Data;
using SmartGeoIot.Services;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace SmartGeoIot.Controllers
{
    public class SGISigfoxController : Controller
    {
        private readonly SecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly SmartGeoIot.SgiSettings _sgiSettings;
        private readonly SmartGeoIotContext _context;
        private LogService _log { get; set; }
        private readonly IEmailSender _emailSender;
        RadiodadosService _sgiService;
        private string[] _sigfoxLogins;
        private string[] _sigfoxPasswords;
        

        public SGISigfoxController(
            SecurityService securityService,
            IConfiguration configuration,
            SmartGeoIotContext context,
            LogService log,
            IOptions<SmartGeoIot.SgiSettings> sgiSettings,
            IEmailSender emailSender,
            RadiodadosService sgiService)
        {
            _securityService = securityService;
            _configuration = configuration;
            _context = context;
            _log = log;
            _securityService = securityService;
            _emailSender = emailSender;
            _sgiService = sgiService;
            _sgiSettings = sgiSettings.Value;

            _sigfoxLogins = _sgiSettings.SIG_FOX_LOGIN.Split(";");
            _sigfoxPasswords = _sgiSettings.SIG_FOX_PASSWORD.Split(";");
        }

        #region MESSAGES
        public async Task DownloadMessagesSigfox()
        {
            try
            {
                _log.Log("Iniciando download dos dados dos dispositivos.");

                var devices = _sgiService.GetAllDevices();
                if (devices != null)
                {
                    foreach (var device in devices)
                    {
                        try
                        {    
                            _log.Log($"Baixando dados do dispositivo {device.Id} - {device.Name}.");

                            for (int i = 0; i < _sigfoxLogins.Length; i++)
                            {
                                var messages = await _sgiService.SigfoxGetMessagesByDevice(device.Id, _sigfoxLogins[i], _sigfoxPasswords[i]);

                                if (messages == null)
                                    continue;

                                if (messages.data.Length > 0)
                                    _sgiService.SigfoxSaveMessages(messages);
                            }
                        }
                        catch (System.Exception ex)
                        {
                            _log.Log($"Erro download dos dados do dispositivo {device.Id}. Error: {ex.Message.ToString().Substring(1, 300)}");
                            continue;
                        }
                    }
                }

                // try
                // {
                //     // _log.Log("Iniciando atualização da data de operação.");
                //     _sgiService.UpdateOperationDateFromMessages();
                //     // _log.Log("Finalizando atualização da data de operação.");
                // }
                // catch (System.Exception error)
                // {
                //     _log.Log("Erro na atualização da data de operação.", error.Message);
                // }

                _log.Log("Finalizado download dos dados dos dispositivos.");
            }
            catch (System.Exception ex)
            {
                _log.Log($"Erro download dos dados dos dispositivos. Error: {ex.Message.ToString().Substring(1, 300)}");
            }
        }

        public async Task DownloadMessagesSigfoxFromDevice(string id)
        {
            try
            {
                _log.Log($"Baixando dados do dispositivo {id}.");
                Models.SigfoxMessage messages = null;

                for (int i = 0; i < _sigfoxLogins.Length; i++)
                {
                    messages = await _sgiService.SigfoxGetMessagesByDevice(id, _sigfoxLogins[i], _sigfoxPasswords[i]);

                    if (messages != null)
                    {
                        if (messages.data.Length > 0)
                            _sgiService.SigfoxSaveMessages(messages);
                    }
                }


                // try
                // {
                //     // _log.Log("Iniciando atualização da data de operação.");
                //     _sgiService.UpdateOperationDateFromMessages();
                //     // _log.Log("Finalizando atualização da data de operação.");
                // }
                // catch (System.Exception error)
                // {
                //     _log.Log("Erro na atualização da data de operação.", error.Message);
                // }

                _log.Log($"Finalizado download de dados do dispositivo {id}.");
            }
            catch (System.Exception ex)
            {
                _log.Log($"Erro download de dados do dispositivo {id}. Error: {ex.Message.ToString().Substring(1, 300)}");
            }
        }

        public async Task DownloadOLDMessagesSigfox()
        {
            try
            {
                var devices = _sgiService.GetAllDevices();
                if (devices != null)
                {
                    foreach (var device in devices)
                    {
                        _log.Log($"Baixando dados do dispositivo {device.Id} - {device.Name}.");
                        Models.SigfoxMessage messages = await GetMessagesByDevice(device.Id);

                        bool continueUpdatingMessages = messages != null;
                        while (continueUpdatingMessages)
                        {
                            if (messages.data.Length > 0)
                                _sgiService.SigfoxSaveMessages(messages);

                            continueUpdatingMessages = !string.IsNullOrWhiteSpace(messages.paging.next);
                            if (continueUpdatingMessages)
                                messages = await GetMessagesByDevice(device.Id, messages.paging.next);
                        }

                        // var messages = _sgiService.SigfoxGetMessagesByDevice(device.Id);

                        // if (messages.data.Length > 0)
                        //     _sgiService.SigfoxSaveMessages(messages);
                        _log.Log($"Finalizado download de dados do dispositivo {device.Id}.");
                    }
                }
            }
            catch (System.Exception ex)
            {
                _log.Log($"Erro DownloadOLDMessagesSigfox. Error: {ex.Message.ToString().Substring(1, 300)}");
            }
        }
        #endregion

        #region DEVICES
        public async Task DownloadDevicesSigfox()
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
                Models.SigfoxDevice devices = null;

                for (int i = 0; i < _sigfoxLogins.Length; i++)
                {
                    devices = await _sgiService.SigfoxGetDevices(_sigfoxLogins[i], _sigfoxPasswords[i]);

                    if (devices.data.Length > 0)
                        _sgiService.SigfoxSaveDevices(devices);
                }


                _log.Log("Finalizado download dos dispositivos.");
            }
            catch (System.Exception ex)
            {
                _log.Log($"Erro download dos dispositivos. Error: {ex.Message.ToString().Substring(1, 300)}");
            }
        }
        #endregion

        public async Task<Models.SigfoxMessage> GetMessagesByDevice(string deviceId, string next = null)
        {
            Models.SigfoxMessage messageReturn = null;

            for (int i = 0; i < _sigfoxLogins.Length; i++)
            {
                messageReturn = await _sgiService.SigfoxGetMessagesByDevice(deviceId, _sigfoxLogins[i], _sigfoxPasswords[i], "0", "100", next);
            }

            return messageReturn;
        }

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