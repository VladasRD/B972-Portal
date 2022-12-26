using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Box.Security.Services;
using SmartGeoIot.Data;
using SmartGeoIot.Services;
using System;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using SmartGeoIot.Models;
using SmartGeoIot.Extensions;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using SmartGeoIot.Migrations;

namespace SmartGeoIot.Controllers
{
    [Route("/[controller]")]
    public class SigfoxCallbackController : Controller
    {
        private readonly SecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly SmartGeoIotContext _context;
        private LogService _log { get; set; }
        RadiodadosService _sgiService;
        private readonly SmartGeoIot.SgiSettings _sgiSettings;

        public SigfoxCallbackController(
            SecurityService securityService,
            IConfiguration configuration,
            SmartGeoIotContext context,
            LogService log,
            IOptions<SmartGeoIot.SgiSettings> sgiSettings,
            RadiodadosService sgiService)
        {
            _securityService = securityService;
            _configuration = configuration;
            _context = context;
            _log = log;
            _sgiSettings = sgiSettings.Value;
            _securityService = securityService;
            _sgiService = sgiService;
        }

        [HttpPut]
        public async Task Put(string deviceId, string subject, string phone, string text, int type)
        {
            await _sgiService.SendWhatsappMessage(deviceId, subject, phone);
        }

        [HttpPost]
        public async Task Post(string deviceTypeId, [FromBody] Models.CallBackDataAdvanced callBackDataAdvanced)
        {
            // if (callBackDataAdvanced == null)
            // {
            //     callBackDataAdvanced = new Models.CallBackDataAdvanced()
            //     {
            //         data = "8403d203cc03d203d203cca1",
            //         device = "440C54",
            //         timestamp = 1627774762000
            //     };
            // }
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

                // Atualizar a hora do dispositivo
                // if (int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.TRM)
                // {
                //     // verificar se o parametro está setado para trocar o horário
                //     Models.Message _message = new Models.Message()
                //     {
                //         Data = callBackDataAdvanced.data
                //     };

                //     if (_message.Bits.SincHora)
                //         _sgiService.SendPackToUpdateHourDevice(deviceLocation.DeviceId, callBackDataAdvanced.data);
                // }

                

                // VERIFICANDO O TIPO DE PACOTE SE É UM NÚMERO, SE NÃO, TEM ERRO, NÃO PROSSEGUIR
                try
                {
                    int.Parse(callBackDataAdvanced.TypePackage);
                }
                catch (System.Exception)
                {
                    return;                    
                }



                // Projeto - MCond
                if (int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.B987)
                {
                    if (_sgiService.VerifyDeviceIsProjectB987(deviceLocation.DeviceId))
                    {
                        List<Message> listMessage = new List<Message>();
                        listMessage.Add
                        (
                            new Message()
                            {
                                DeviceId = deviceLocation.DeviceId,
                                Data = deviceLocation.Data,
                                Time = deviceLocation.Time,
                                OperationDate = Utils.TimeStampSecondsToDateTime(deviceLocation.Time)
                            }
                        );

                        Models.MCond currentData = _sgiService.ProcessDataB987(listMessage);

                        // notificar alterações nos alertas
                        if (currentData != null)
                            await _sgiService.VerifyAndAlertAlarmsB987(currentData);
                    }
                }
                
                // pacote 83 projeto vazão de fluxo TSP
                if (int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.TSP)
                {
                    var projectDevicesB982_S = _sgiService.GetDevicesByProjectCode(Utils.EnumToAnnotationText(ProjectCode.B982_S));
                    var devicesB982_S = projectDevicesB982_S != null ? projectDevicesB982_S.Select(s => s.DeviceId).ToArray() : null;
                    if (devicesB982_S != null && devicesB982_S.Any(a => deviceLocation.DeviceId == a) && int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.TSP)
                    {
                        try
                        {    
                            _log.Log($"SigfoxCallbackController/Post: Pacote {(int)Models.PackagesEnum.TSP} recebido, verificando mudança de alerta.");
                            _sgiService.SendNotificationStateChangedTSP(deviceLocation);
                        }
                        catch (System.Exception ex)
                        {   
                            _log.Log($"SigfoxCallbackController/Post: Erro Pacote {(int)Models.PackagesEnum.TSP} na verificação de mudança de alerta.", ex.Message, true);
                        }
                    }
                }

                // pacote 82 projeto vazão de fluxo TQA
                if (int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.TQA_S)
                {
                    var projectDevicesB981 = _sgiService.GetDevicesByProjectCode(Utils.EnumToAnnotationText(ProjectCode.B981));
                    var devicesB981 = projectDevicesB981 != null ? projectDevicesB981.Select(s => s.DeviceId).ToArray() : null;
                    if (devicesB981 != null && devicesB981.Any(a => deviceLocation.DeviceId == a) && int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.TQA_S)
                    {
                        try
                        {    
                            _log.Log($"SigfoxCallbackController/Post: Pacote {(int)Models.PackagesEnum.TQA_S} recebido, verificando mudança de alerta.");
                            _sgiService.SendNotificationStateChangedTQA(deviceLocation);
                        }
                        catch (System.Exception ex)
                        {   
                            _log.Log($"SigfoxCallbackController/Post: Erro Pacote {(int)Models.PackagesEnum.TQA_S} na verificação de mudança de alerta.", ex.Message, true);
                        }
                    }
                }

                // pacote 23 projeto vazão de fluxo Resil
                if (int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.TRM)
                {
                    var projectDevicesB972_P = _sgiService.GetDevicesByProjectCode(Utils.EnumToAnnotationText(ProjectCode.B972_P));
                    var devicesB972_P = projectDevicesB972_P != null ? projectDevicesB972_P.Select(s => s.DeviceId).ToArray() : null;
                    if (devicesB972_P != null && devicesB972_P.Any(a => deviceLocation.DeviceId == a) && int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.TRM)
                    {
                        try
                        {    
                            _log.Log($"SigfoxCallbackController/Post: Pacote {(int)Models.PackagesEnum.TRM}, verificando mudança de estado.");
                            _sgiService.SendNotificationStateChangedTRM(deviceLocation);
                        }
                        catch (System.Exception ex)
                        {   
                            _log.Log($"SigfoxCallbackController/Post: Erro Pacote {(int)Models.PackagesEnum.TRM}, verificando mudança de estado.", ex.Message, true);
                        }
                    }
                }

                // if (int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.TSP)
                // {
                //     try
                //     {    
                //         _log.Log($"SigfoxCallbackController/Post: Pacote {(int)Models.PackagesEnum.TSP} recebido, verificando mudança de estado.");
                //         _sgiService.SendNotificationStateChanged(deviceLocation);
                //     }
                //     catch (System.Exception ex)
                //     {   
                //         _log.Log($"SigfoxCallbackController/Post: Erro Pacote {(int)Models.PackagesEnum.TSP}, verificando mudança de estado.", ex.Message, true);
                //     }
                // }

                var projectDevices = _sgiService.GetDevicesByProjectCode(Utils.EnumToAnnotationText(ProjectCode.B982_S));
                var devices = projectDevices != null ? projectDevices.Select(s => s.DeviceId).ToArray() : null;
                if (devices != null && devices.Any(a => deviceLocation.DeviceId == a) && int.Parse(callBackDataAdvanced.TypePackage) == (int)Models.PackagesEnum.B972_84)
                {
                    try
                    {
                        // if (!await _sgiService.ExistPackProcessB982_S(deviceLocation.Time, deviceLocation.DeviceId))
                        if (!await _sgiService.ExistPackProcessed(deviceLocation.Time, "83", deviceLocation.DeviceId))
                        {
                            try
                            {     
                                _log.Log("Pacote 84 recebido, convertendo.");

                                // converter 84 e gerar 83´s
                                var packs84 = _sgiService.GetVazaosPack84(deviceLocation.Data, deviceLocation.Time, deviceLocation.DeviceId);
                                foreach (var item in packs84)
                                {
                                    _sgiService.CreatePack83ByData84(item);
                                }
                                // _sgiService.CreateB982_SByData84List(packs84, deviceLocation.Data);

                                // Setando que o pacote já foi processado
                                _sgiService.SetPackProcessed(deviceLocation.DeviceId, deviceLocation.Data, deviceLocation.Time);
                            }
                            catch (System.Exception ex)
                            {
                                _log.Log("SmartGeoIotService.SetPackProcessed: Error processes pack 84.", ex.InnerException == null ? ex.Message : ex.InnerException.Message, true);
                            }


                        }
                    }
                    catch (System.Exception ex)
                    {
                        _log.Log("Pacote 84 recebido, erro na conversão.", ex.Message, true);
                    }
                }

            }

            // atualizar dados deste dispositivo
            // try
            // {
            //     _sgiService.DownloadMessageByDevice(callBackDataAdvanced?.device);
            // }
            // catch (System.Exception ex)
            // {
            //     _log.Log($"Erro ao atualizar dados do disposito {callBackDataAdvanced?.device} quando detectado bloqueio. Erro: {ex.Message}.");
            // }

            _log.Log("Método POST de call-back da sigfox de localização do dispositivo retornou " +
            $"id:{callBackDataAdvanced?.device}, time:{callBackDataAdvanced?.timestamp}, data: {callBackDataAdvanced?.data}, deviceTypeId:{deviceTypeId?.ToString()}, " +
            $"latitude: {computedLocation?.lat}, longitude: {computedLocation?.lng}, radius: {computedLocation?.radius}.");
        }

    }

}