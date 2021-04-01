using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartGeoIot.Models;
using SmartGeoIot.Extensions;

namespace SmartGeoIot.Services
{
    public partial class SmartGeoIotService
    {
        public SigfoxDevice SigfoxGetDevices()
        {
            try
            {
                HttpClient wc = new HttpClient();
                wc.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Utils.CreateBasicOauth(_sgiSettings.SIG_FOX_LOGIN, _sgiSettings.SIG_FOX_PASSWORD));

                var parameters = new Dictionary<string, string>();
                parameters.Add("Content-Type", "application/json");
                var req = new HttpRequestMessage(HttpMethod.Get, $"{_sgiSettings.SIG_FOX_URL}/devices/") { Content = new FormUrlEncodedContent(parameters) };
                HttpResponseMessage msg = wc.SendAsync(req).Result;

                string jsonResult = string.Empty;
                if (msg.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    jsonResult = msg.Content.ReadAsStringAsync().Result;
                }

                return JsonConvert.DeserializeObject<SigfoxDevice>(jsonResult);
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao baixar dispositivos, erro {ex.Message}.");
                throw;
            }
        }

        public SigfoxMessage SigfoxGetMessagesByDevice(string id, string before = "0", string limit = "100", string next = null)
        {
            try
            {
                HttpClient wc = new HttpClient();
                wc.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Utils.CreateBasicOauth(_sgiSettings.SIG_FOX_LOGIN, _sgiSettings.SIG_FOX_PASSWORD));

                var parameters = new Dictionary<string, string>();
                parameters.Add("Content-Type", "application/json");
                parameters.Add("limit", limit);
                parameters.Add("before", before);

                string _url = null;
                if (String.IsNullOrWhiteSpace(next))
                    _url = $"{_sgiSettings.SIG_FOX_URL}/devices/{id}/messages";
                else
                    _url = next;

                var req = new HttpRequestMessage(HttpMethod.Get, _url) { Content = new FormUrlEncodedContent(parameters) };
                HttpResponseMessage msg = wc.SendAsync(req).Result;

                string jsonResult = string.Empty;
                if (msg.StatusCode == System.Net.HttpStatusCode.OK)
                    jsonResult = msg.Content.ReadAsStringAsync().Result;

                return JsonConvert.DeserializeObject<SigfoxMessage>(jsonResult);
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao baixar informações do dispositivo {id}, erro {ex.Message}.");
                throw;
            }
        }

        public void SigfoxSendChangesDeviceTypes(Device device, int numeroEnvios, int tempoTransmissao, bool tipoEnvio, int tensaoMinima)
        {
            try
            {
                ViewModels.DashboardViewModels dashboard = GetDashboard(device.Id);
                string typePackage = (dashboard.TypePackage.ToLower().Equals("12") ? "54" : "11");
                string downlinkDataString = CreatePackDownloadLink(typePackage, numeroEnvios, tempoTransmissao, tipoEnvio, tensaoMinima, dashboard.Package.Substring(8, 2), dashboard.Bits.Downlink);

                DeviceType data = new DeviceType()
                {
                    id = device.DeviceTypeId,
                    downlinkDataString = downlinkDataString
                };

                HttpClient client = new HttpClient();
                string _url = $"{_sgiSettings.SIG_FOX_URL}/device-types/{device.DeviceTypeId}";
                string _contentType = "application/json";

                client.BaseAddress = new Uri(_url);
                client.DefaultRequestHeaders.Add("Authorization", string.Format("Basic {0}", Utils.CreateBasicOauth(_sgiSettings.SIG_FOX_LOGIN, _sgiSettings.SIG_FOX_PASSWORD)));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_contentType));

                HttpContent _Body = new StringContent(JsonConvert.SerializeObject(data));
                _Body.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
                HttpResponseMessage response = client.PutAsync(_url, _Body).Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    _log.Log($"Erro enviar alterações do dispositivo {device.Id}, erro {response.RequestMessage}, status code {response.StatusCode}.");
                    throw new Box.Common.BoxLogicException("Erro ao enviar as informações de alteração do dispositivo, consulte o log para mais informações.");
                }
                UpdateDeviceRegistration(device.Id, downlinkDataString);
                _log.Log($"Envio de DownloadLink {downlinkDataString} tipo pacote {typePackage} para o dispositivo {device.Id} com os seguintes dados: numero de envios: {numeroEnvios}, tempo de transmissão: {tempoTransmissao}.");
            }
            catch (Exception ex)
            {
                _log.Log($"Erro enviar alterações do dispositivo {device.Id}, erro {ex.Message}.");
                throw new Box.Common.BoxLogicException("Erro ao enviar as informações de alteração do dispositivo, consulte o log para mais informações.");
            }
        }

        public void UpdateDeviceRegistration(string deviceId, string dataDownloadLink)
        {
            var deviceRegistration = GetDeviceRegistrationById(deviceId);
            if (deviceRegistration == null)
                return;
            deviceRegistration.DataDownloadLink = dataDownloadLink;
            _context.Entry(deviceRegistration).State = EntityState.Modified;

            _context.SaveChanges(false);
            _log.Log($"Download link do dispositivo {deviceRegistration.Name} atualizado.");
        }

        public void SigfoxSaveDevices(SigfoxDevice sigfoxDevice)
        {
            foreach (var item in sigfoxDevice.data)
            {
                Device _addDevice = new Device();
                _addDevice.Id = item.id;
                _addDevice.Name = item.name;
                _addDevice.SequenceNumber = item.sequenceNumber;
                _addDevice.LastCom = item.lastCom;
                _addDevice.State = item.state;
                _addDevice.ComState = item.comState;
                _addDevice.Pac = item.pac;
                _addDevice.LocationLat = item.location.lat;
                _addDevice.LocationLng = item.location.lng;
                _addDevice.DeviceTypeId = item.deviceType.id;
                _addDevice.GroupId = item.group.id;
                _addDevice.Lqi = item.lqi;
                _addDevice.ActivationTime = item.activationTime;
                _addDevice.TokenState = item.token.state;
                _addDevice.TokenDetailMessage = item.token.detailMessage;
                _addDevice.TokenEnd = item.token.end;
                _addDevice.ContractId = item.contract.id;
                _addDevice.CreationTime = item.creationTime;
                _addDevice.ModemCertificateId = item.modemCertificate.id;
                _addDevice.Prototype = item.prototype;
                _addDevice.AutomaticRenewal = item.automaticRenewal;
                _addDevice.AutomaticRenewalStatus = item.automaticRenewalStatus;
                _addDevice.CreatedBy = item.createdBy;
                _addDevice.LastEditionTime = item.lastEditionTime;
                _addDevice.LastEditedBy = item.lastEditedBy;
                _addDevice.Activable = item.activable;

                Device oldDevice = _context.Devices.Find(_addDevice.Id);
                if (oldDevice == null)
                    _context.Devices.Add(_addDevice);
                else
                {
                    _context.Devices.Attach(oldDevice);
                    _context.Entry<Device>(oldDevice).CurrentValues.SetValues(_addDevice);
                }

                _context.SaveChanges(true);
                _log.Log($"Dispositivo {_addDevice.Name} criado/atualizado.");
            }
        }

        public void SigfoxSaveMessages(SigfoxMessage sigfoxMessage)
        {
            List<Message> listMessages = new List<Message>();
            foreach (var item in sigfoxMessage.data)
            {
                if (!VerifyExistMessage(item.time.ToString()))
                {
                    Message newMessage = new Message();
                    newMessage.Id = item.time.ToString(); // using for PK of table
                    newMessage.DeviceId = item.device.id;
                    newMessage.Time = item.time;
                    newMessage.Data = item.data;
                    newMessage.RolloverCounter = item.rolloverCounter;
                    newMessage.SeqNumber = item.seqNumber;
                    newMessage.NbFrames = item.nbFrames;
                    newMessage.Operator = item._operator;
                    newMessage.Country = item.country;
                    newMessage.Lqi = item.lqi;
                    newMessage.OperationDate = Utils.TimeStampToDateTime(item.time).ToUniversalTime();

                    listMessages.Add(newMessage);
                }
            }

            if (listMessages.Count > 0)
            {
                _context.Messages.AddRange(listMessages);
                _context.SaveChanges();
                _log.Log($"Dados do dispositivo criados.");
            }
        }

        public void UpdateOperationDateFromMessages()
        {
            var messages = _context.Messages.Where(c => c.OperationDate.Value == null).ToList();
            foreach (var message in messages)
            {
                message.OperationDate = Utils.TimeStampToDateTime(message.Time).ToUniversalTime();
            }

            if (messages.Count > 0)
            {
                _context.Messages.UpdateRange(messages);
                _context.SaveChanges();
                _log.Log($"Data da operação atualizada.");
            }
        }

        public IList<Device> GetAllDevices()
        {
            return _context.Devices.AsNoTracking().Where(w => w.Activable).ToList();
        }

        private bool VerifyExistMessage(string id)
        {
            IQueryable<Message> messages = _context.Messages.Where(w => w.Id == id);
            return messages.Count() > 0;
        }

        private string CreatePackDownloadLink(string typePackage, int numeroEnvios, int tempoTransmissao, bool tipoEnvio, int tensaoMinima, string alimentacaoMinima, bool downloadLink)
        {
            string start = typePackage;
            string b0 = Utils.ZerosForLeft(Utils.BinaryStringToHexString($"000000{Convert.ToInt32(downloadLink)}{Convert.ToInt32(tipoEnvio)}"), 2);
            string b1_b2 = Utils.ZerosForLeft(Utils.DecimalToHexa(tempoTransmissao.ToString()), 4);
            string b3 = tensaoMinima > 0 ? Utils.DecimalToHexa(tensaoMinima.ToString()) : alimentacaoMinima;
            string b4 = Utils.ZerosForLeft(Utils.DecimalToHexa(numeroEnvios.ToString()), 2);
            string b5 = "00";
            string _calcEnd = Utils.SumHexValuesOfPack($"{start}{b0}{b1_b2}{b3}{b4}{b5}");
            string end = _calcEnd.Length > 2 ? _calcEnd.Substring(1, 2) : _calcEnd;

            return $"{start}{b0}{b1_b2}{b3}{b4}{b5}{end}";
        }

        public void DownloadMessageByDevice(string deviceId)
        {
            _log.Log($"Baixando dados do dispositivo {deviceId}.");

            var messages = SigfoxGetMessagesByDevice(deviceId);
            if (messages.data.Length > 0)
                SigfoxSaveMessages(messages);

            _log.Log($"Finalizado download de dados do dispositivo {deviceId}.");
        }

    }
}