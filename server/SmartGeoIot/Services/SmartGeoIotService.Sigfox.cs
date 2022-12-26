using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartGeoIot.Models;
using SmartGeoIot.Extensions;
using System.Threading.Tasks;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        public async Task<SigfoxDevice> SigfoxGetDevices(string SIG_FOX_LOGIN, string SIG_FOX_PASSWORD)
        {
            try
            {
                // HttpClient wc = new HttpClient();
                // wc.DefaultRequestHeaders.Authorization =
                // new AuthenticationHeaderValue("Basic", Utils.CreateBasicOauth(_sgiSettings.SIG_FOX_LOGIN, _sgiSettings.SIG_FOX_PASSWORD));

                // var parameters = new Dictionary<string, string>();
                // parameters.Add("Content-Type", "application/json");
                // var req = new HttpRequestMessage(HttpMethod.Get, $"{_sgiSettings.SIG_FOX_URL}/devices/") { Content = new FormUrlEncodedContent(parameters) };
                // HttpResponseMessage msg = wc.SendAsync(req).Result;

                // string jsonResult = string.Empty;
                // if (msg.StatusCode == System.Net.HttpStatusCode.OK)
                // {
                //     jsonResult = msg.Content.ReadAsStringAsync().Result;
                // }

                // return JsonConvert.DeserializeObject<SigfoxDevice>(jsonResult);

                var client = new HttpClient();
                string jsonResult = string.Empty;
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_sgiSettings.SIG_FOX_URL}/devices?Content-Type=application%2Fjson"),
                    Headers =
                    {
                        { "Authorization", $"Basic {Utils.CreateBasicOauth(SIG_FOX_LOGIN, SIG_FOX_PASSWORD)}" },
                    },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    jsonResult = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<SigfoxDevice>(jsonResult);
                }
                
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao baixar dispositivos, erro {ex.Message}.");
                throw;
            }
        }

        public async Task<SigfoxMessage> SigfoxGetMessagesByDevice(string id, string SIG_FOX_LOGIN, string SIG_FOX_PASSWORD, string before = "0", string limit = "100", string next = null)
        {
            try
            {
                // HttpClient wc = new HttpClient();
                // wc.DefaultRequestHeaders.Authorization =
                // new AuthenticationHeaderValue("Basic", Utils.CreateBasicOauth(_sgiSettings.SIG_FOX_LOGIN, _sgiSettings.SIG_FOX_PASSWORD));

                // var parameters = new Dictionary<string, string>();
                // parameters.Add("Content-Type", "application/json");
                // parameters.Add("limit", limit);
                // parameters.Add("before", before);

                string _url = null;
                if (String.IsNullOrWhiteSpace(next))
                    _url = $"{_sgiSettings.SIG_FOX_URL}/devices/{id}/messages";
                else
                    _url = next;

                var client = new HttpClient();
                string jsonResult = string.Empty;
                var request = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri($"{_url}?Content-Type=application%2Fjson"),
                    Headers =
                    {
                        { "Authorization", $"Basic {Utils.CreateBasicOauth(SIG_FOX_LOGIN, SIG_FOX_PASSWORD)}" },
                    },
                };
                using (var response = await client.SendAsync(request))
                {
                    response.EnsureSuccessStatusCode();
                    jsonResult = await response.Content.ReadAsStringAsync();

                    return JsonConvert.DeserializeObject<SigfoxMessage>(jsonResult);
                }


                // var req = new HttpRequestMessage(HttpMethod.Get, _url) { Content = new FormUrlEncodedContent(parameters) };
                // HttpResponseMessage msg = wc.SendAsync(req).Result;
                // if (msg.StatusCode == System.Net.HttpStatusCode.OK)
                //     jsonResult = msg.Content.ReadAsStringAsync().Result;

                // return JsonConvert.DeserializeObject<SigfoxMessage>(jsonResult);
            }
            catch (Exception ex)
            {
                _log.Log($"Erro ao baixar informações do dispositivo {id}, erro {ex.Message}.");
                return null;
            }
        }

        public Task SendPackToUpdateHourDevice(string deviceId, string data)
        {
            return Task.Run(() =>
            {
                try
                {
                    SigfoxSendChangeHourDevice(deviceId);
                    _log.Log($"Pacote de atualização de horário do dispositivo {deviceId} enviado com sucesso as {DateTime.Now.ToString("HH:mm:ss")}.");
                }
                catch (Exception ex)
                {
                    _log.Log($"Erro no envio no pacote de atualização de horário do dispositivo {deviceId}.", ex.Message);
                }
            });
        }

        // void DownloadMessageByDevice(string deviceId)
        // {
        //     HttpClient wc = new HttpClient();
        //     try
        //     {
        //         wc.GetAsync($"{_sgiSettings.SERVER_URL}/DownloadMessagesSigfoxFromDevice/{deviceId}");
        //     }
        //     catch (Exception ex)
        //     {
        //         _log.Log($"Erro na atualização de dados do dispositivo {deviceId}.", ex.Message);
        //     }
        // }

        public void SendNotificationStateChangedTSP(DeviceLocation deviceLocation)
        {
            // return Task.Run(() =>
            // {
            try
            {
                // atualizando dados do dispositivo
                DownloadMessageByDevice(deviceLocation.DeviceId).Wait();

                var (lastMessage, currentMessage) = VerifyStateChangedTSP(deviceLocation);
                if (lastMessage == null)
                    return;

                // enviar notificação de estado alterado
                var clients = GetClientsByDevice(lastMessage.DeviceId);
                if (clients == null)
                    return;

                SendStateChangedDeviceTSP(clients.Select(s => s.Email).ToArray(), lastMessage, currentMessage).Wait();
                _log.Log($"Envio de notificação de estado alterado do dispositivo {deviceLocation.DeviceId}.");
            }
            catch (Exception ex)
            {
                _log.Log($"Erro no envio de notificação de estado alterado do dispositivo {deviceLocation.DeviceId}.", ex.Message);
            }
            // });
        }

        public void SendNotificationStateChangedTQA(DeviceLocation deviceLocation)
        {
            // return Task.Run(() =>
            // {
            try
            {
                // atualizando dados do dispositivo
                DownloadMessageByDevice(deviceLocation.DeviceId).Wait();

                var (lastMessage, currentMessage) = VerifyStateChangedTQA(deviceLocation);
                if (lastMessage == null)
                    return;

                // enviar notificação de estado alterado
                var clients = GetClientsByDevice(lastMessage.DeviceId);
                if (clients == null)
                    return;

                SendStateChangedDeviceTQA(clients.Select(s => s.Email).ToArray(), lastMessage, currentMessage).Wait();
                _log.Log($"Envio de notificação de estado alterado do dispositivo {deviceLocation.DeviceId}.");
            }
            catch (Exception ex)
            {
                _log.Log($"Erro no envio de notificação de estado alterado do dispositivo {deviceLocation.DeviceId}.", ex.Message);
            }
            // });
        }

        public void SendNotificationStateChangedTRM(DeviceLocation deviceLocation)
        {
            // return Task.Run(() =>
            // {
            try
            {
                // atualizando dados do dispositivo
                DownloadMessageByDevice(deviceLocation.DeviceId).Wait();

                var (lastMessage, lastState) = VerifyStateChangedTRM(deviceLocation);
                if (lastMessage == null)
                    return;
                
                // enviar notificação de estado alterado
                var clients = GetClientsByDevice(lastMessage.DeviceId);
                if (clients == null)
                    return;

                SendStateChangedDeviceTRM(clients.Select(s => s.Email).ToArray(), lastMessage, lastState).Wait();
                _log.Log($"Envio de notificação de estado alterado do dispositivo {deviceLocation.DeviceId}.");
            }
            catch (Exception ex)
            {
                _log.Log($"Erro no envio de notificação de estado alterado do dispositivo {deviceLocation.DeviceId}.", ex.Message);
            }
            // });
        }

        // public void SendNotificationStateChanged(DeviceLocation deviceLocation)
        // {
        //     // return Task.Run(() =>
        //     // {
        //     try
        //     {
        //         var currentMessage = VerifyStateChanged(deviceLocation);
        //         if (currentMessage == null)
        //             return;
                
        //         // enviar notificação de estado alterado
        //         var clients = GetClientsByDevice(currentMessage.DeviceId);
        //         if (clients == null)
        //             return;

        //         SendStateChangedDevice(clients.Select(s => s.Email).ToArray(), currentMessage).Wait();
        //         _log.Log($"Envio de notificação de estado alterado do dispositivo {deviceLocation.DeviceId}.");
        //     }
        //     catch (Exception ex)
        //     {
        //         _log.Log($"Erro no envio de notificação de estado alterado do dispositivo {deviceLocation.DeviceId}.", ex.Message);
        //     }
        //     // });
        // }

        public Message VerifyStateChanged(DeviceLocation deviceLocation)
        {
            deviceLocation.Time = Utils.TimeZerosForRight(deviceLocation.Time.ToString(), 13);
            Message currentMessage = new Message() { Data = deviceLocation.Data };
            Message lastinfoDevice = _context.Messages.OrderByDescending(o => o.Time).FirstOrDefault(f => f.DeviceId == deviceLocation.DeviceId
                                                && f.Time != deviceLocation.Time && f.TypePackage.Equals("83"));

            if (lastinfoDevice == null)
                return null;

            if (currentMessage.CalhaAlerta.Equals(lastinfoDevice.CalhaAlerta))
                return null;

            if (string.IsNullOrEmpty(currentMessage.DeviceId))
                currentMessage.DeviceId = deviceLocation.DeviceId;

            return currentMessage;
        }

        public (Message, string) VerifyStateChangedTSP(DeviceLocation deviceLocation)
        {
            deviceLocation.Time = Utils.TimeZerosForRight(deviceLocation.Time.ToString(), 13);
            Message currentMessage = new Message() { Data = deviceLocation.Data, Time = deviceLocation.Time };
            Message lastinfoDevice = _context.Messages.OrderByDescending(o => o.Time).FirstOrDefault(f => f.DeviceId == deviceLocation.DeviceId
                                                && f.Time != deviceLocation.Time && int.Parse(f.TypePackage) == (int)Models.PackagesEnum.TSP);

            if (lastinfoDevice == null)
                return (null, null);

            if (currentMessage.CalhaAlerta.Equals(lastinfoDevice.CalhaAlerta))
                return (null, null);

            if (string.IsNullOrEmpty(currentMessage.DeviceId))
                currentMessage.DeviceId = deviceLocation.DeviceId;

            return (lastinfoDevice, lastinfoDevice.CalhaAlerta);
        }

        public (Message, string) VerifyStateChangedTRM(DeviceLocation deviceLocation)
        {
            deviceLocation.Time = Utils.TimeZerosForRight(deviceLocation.Time.ToString(), 13);
            Message currentMessage = new Message() { Data = deviceLocation.Data, Time = deviceLocation.Time };
            Message lastinfoDevice = _context.Messages.OrderByDescending(o => o.Time).FirstOrDefault(f => f.DeviceId == deviceLocation.DeviceId
                                                && f.Time != deviceLocation.Time && f.TypePackage.Equals("23"));

            if (lastinfoDevice == null)
                return (null, null);

            var _currentDisplay = Consts.GetDisplayTRM10(currentMessage.Bits.BAlertaMax, currentMessage.Bits.ModoFechado, currentMessage.Bits.ModoAberto);
            var _lastDisplay = Consts.GetDisplayTRM10(lastinfoDevice.Bits.BAlertaMax, lastinfoDevice.Bits.ModoFechado, lastinfoDevice.Bits.ModoAberto);

            if (_currentDisplay.DisplayEstado.Equals(_lastDisplay.DisplayEstado))
                return (null, null);

            if (string.IsNullOrEmpty(currentMessage.DeviceId))
                currentMessage.DeviceId = deviceLocation.DeviceId;

            return (lastinfoDevice, _lastDisplay.DisplayEstado);//currentMessage;
        }

        public (Message, Message) VerifyStateChangedTQA(DeviceLocation deviceLocation)
        {
            deviceLocation.Time = Utils.TimeZerosForRight(deviceLocation.Time.ToString(), 13);
            Message currentMessage = new Message() { Data = deviceLocation.Data, Time = deviceLocation.Time };
            Message lastinfoDevice = _context.Messages.OrderByDescending(o => o.Time).FirstOrDefault(f => f.DeviceId == deviceLocation.DeviceId
                                                && f.Time != deviceLocation.Time && int.Parse(f.TypePackage) == (int)Models.PackagesEnum.TQA_S);

            if (lastinfoDevice == null)
                return (null, null);

            if (currentMessage.ReleBoolean.Rele7.Equals(lastinfoDevice.ReleBoolean.Rele7) &&
                currentMessage.ReleBoolean.Rele6.Equals(lastinfoDevice.ReleBoolean.Rele6) &&
                currentMessage.ReleBoolean.Rele5.Equals(lastinfoDevice.ReleBoolean.Rele5) &&
                currentMessage.ReleBoolean.Rele4.Equals(lastinfoDevice.ReleBoolean.Rele4) &&
                currentMessage.ReleBoolean.Rele3.Equals(lastinfoDevice.ReleBoolean.Rele3) &&
                currentMessage.ReleBoolean.Rele2.Equals(lastinfoDevice.ReleBoolean.Rele2) &&
                currentMessage.ReleBoolean.Rele1.Equals(lastinfoDevice.ReleBoolean.Rele1))
            {
                return (null, null);
            }

            if (string.IsNullOrEmpty(currentMessage.DeviceId))
                currentMessage.DeviceId = deviceLocation.DeviceId;

            return (lastinfoDevice, currentMessage);
        }

        public async Task<bool> ExistPackProcessed(long time, string pack, string deviceId)
        {
            return await _context.Messages.AnyAsync(c => c.DeviceId == deviceId && c.Time == time && c.Data.Substring(0, 2).Equals(pack));
        }

        // public List<B982_S> GetVazaosPack84(string dataPack84, long time, string deviceId)
        // {
        //      Message currentDataDevice = _context.Messages
        //         .OrderByDescending(o => o.Time)
        //         .FirstOrDefault(f => f.DeviceId == deviceId && f.TypePackage.Equals("83"));
            
        //     List<B982_S> list = new List<B982_S>();
        //     int seconds = 60;
        //     int miliseconds = 1000;

        //     long currentTime = 0;
        //     currentTime = time;

        //     var (_data, _operator) = GeneratePack83(currentDataDevice.Data, dataPack84, 0, 4);
        //     list.Add(new B982_S()
        //     {
        //         Id = currentTime.ToString(),
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         Data = _data,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+1,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     currentTime = time - (3 * seconds * miliseconds);
        //     var (_data2, _operator2) = GeneratePack83(currentDataDevice.Data, dataPack84, 4, 4);
        //     list.Add(new B982_S()
        //     {
        //         Id = currentTime.ToString(),
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         Data = _data2,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+2,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator2,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     currentTime = time - (6 * seconds * miliseconds);
        //     var (_data3, _operator3) = GeneratePack83(currentDataDevice.Data, dataPack84, 8, 4);
        //     list.Add(new B982_S()
        //     {
        //         Id = currentTime.ToString(),
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         Data = _data3,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+3,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator3,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     currentTime = time - (9 * seconds * miliseconds);
        //     var (_data4, _operator4) = GeneratePack83(currentDataDevice.Data, dataPack84, 12, 4);
        //     list.Add(new B982_S()
        //     {
        //         Id = currentTime.ToString(),
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         Data = _data4,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+4,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator4,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     currentTime = time - (12 * seconds * miliseconds);
        //     var (_data5, _operator5) = GeneratePack83(currentDataDevice.Data, dataPack84, 16, 4);
        //     list.Add(new B982_S()
        //     {
        //         Id = currentTime.ToString(),
        //         DeviceId = deviceId,
        //         Time = currentTime,
        //         Data = _data5,
        //         RolloverCounter = currentDataDevice.RolloverCounter,
        //         SeqNumber = currentDataDevice.SeqNumber+5,
        //         NbFrames = currentDataDevice.NbFrames,
        //         Operator = _operator5,
        //         Country = currentDataDevice.Country,
        //         Lqi = currentDataDevice.Lqi,
        //         OperationDate = Utils.TimeStampSecondsToDateTimeByTimestapInformed(currentTime)
        //     });
            
        //     return list;
        // }

        public void CreatePack83ByData84(Message message)
        {
            try
            {     
                _context.Entry<Message>(message).State = EntityState.Added;
                _context.SaveChanges(true);
            }
            catch (System.Exception ex)
            {
                _log.Log("SmartGeoIotService.CreatePack83ByData84: Error create pack pack 83 by pack 84.", ex.InnerException == null ? ex.Message : ex.InnerException.Message, true);
            }
        }

        public void SetPackProcessed(string deviceId, string package, long time)
        {
            try
            {     
                var pack = GetPackageByDeviceId(deviceId, package, time);
                if (pack != null)
                {
                    pack.WasProcessed = true;
                    _context.Entry<Message>(pack).State = EntityState.Modified;
                    _context.SaveChanges(true);
                }
            }
            catch (System.Exception ex)
            {
                _log.Log("SmartGeoIotService.SetPackProcessed: Error set pack processed.", ex.InnerException == null ? ex.Message : ex.InnerException.Message, true);
            }
        }

        internal Message GetPackageByDeviceId(string deviceId, string package, long time)
        {
            return _context.Messages.AsNoTracking().SingleOrDefault(c => c.DeviceId == deviceId && c.Data == package && c.Time == time);
        }

        // public void CreateB982_SByData84(List<B982_S> b982_S)
        // {
        //     // _context.Entry<B982_S>(b982_S).State = EntityState.Added;
        //     _context.B982_S.AddRange(b982_S);
        //     _context.SaveChanges(true);
        //     // _log.Log($"Criação do pacote B982_S: {b982_S.OriginPack} com base no 84.");
        // }

        internal (string, string) GeneratePack83(string currentPack83, string currentPack84, int idxStart, int idxEnd)
        {
            idxStart = idxStart + 2;
            Decimal vazao = Utils.HexaToDecimal(currentPack84.Substring(idxStart, idxEnd));
            
            // string customVazao = String.Format("{0:0.000}", Convert.ToDecimal(vazao.ToString().Replace(vazao.ToString().Substring(vazao.ToString().Length - 2, 2), $".{vazao.ToString().Substring(vazao.ToString().Length - 2, 2)}")) / 100);
            // customVazao = customVazao.Replace('.', ',');
            string customVazao = String.Format("{0:0.000}", (vazao / 100));
            
            // string newVazao = Utils.DecimalToHexa(vazao.ToString());
            string newVazao = Utils.ZerosForLeft(Utils.ConvertLongToHexa(Utils.ConvertDoubleToLong(Convert.ToDouble(vazao))), 8);
            string lastPack83 = currentPack83.Substring(10, currentPack83.Length-10);
            
            return ($"83{newVazao}{lastPack83}", customVazao);
        }

        public void SigfoxSendChangeHourDevice(string deviceId)
        {
            try
            {
                ViewModels.DashboardViewModels dashboard = GetDashboard(deviceId);
                DateTime _dataHora = DateTime.UtcNow.AddHours(-3);
                string typePackage = "56";
                string downlinkDataString = CreatePack56DownloadLink(typePackage, _dataHora);

                DeviceType data = new DeviceType()
                {
                    id = deviceId,
                    downlinkDataString = downlinkDataString
                };

                HttpClient client = new HttpClient();
                string _url = $"{_sgiSettings.SIG_FOX_URL}/device-types/{deviceId}";
                string _contentType = "application/json";

                client.BaseAddress = new Uri(_url);
                client.DefaultRequestHeaders.Add("Authorization", string.Format("Basic {0}", Utils.CreateBasicOauth(_sgiSettings.SIG_FOX_LOGIN, _sgiSettings.SIG_FOX_PASSWORD)));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_contentType));

                HttpContent _Body = new StringContent(JsonConvert.SerializeObject(data));
                _Body.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
                HttpResponseMessage response = client.PutAsync(_url, _Body).Result;
                if (response.StatusCode != System.Net.HttpStatusCode.OK && response.StatusCode != System.Net.HttpStatusCode.NoContent)
                {
                    _log.Log($"Erro enviar alterações de data e hora do dispositivo {deviceId}, erro {response.RequestMessage}, status code {response.StatusCode}.");
                    throw new Box.Common.BoxLogicException("Erro ao enviar as informações de alteração de data e hora do dispositivo, consulte o log para mais informações.");
                }
                _log.Log($"Envio de DownloadLink {downlinkDataString} tipo pacote {typePackage} para o dispositivo {deviceId} com a data e hora {_dataHora}");
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                    _log.Log($"Erro enviar alterações de data e hora do dispositivo {deviceId}, erro {ex.InnerException.Message}.");
                else
                    _log.Log($"Erro enviar alterações de data e hora do dispositivo {deviceId}, erro {ex.Message}.");

                // throw new Box.Common.BoxLogicException("Erro ao enviar as informações de alteração de data e hora do dispositivo, consulte o log para mais informações.");
            }
        }

        public void SigfoxSendChangesDeviceTypes(Device device, int numeroEnvios, int tempoTransmissao, bool tipoEnvio, int tensaoMinima)
        {
            try
            {
                ViewModels.DashboardViewModels dashboard = GetDashboard(device.Id);
                // string typePackage = (dashboard.TypePackage.ToLower().Equals("12") ? "54" : "11");
                string typePackage = "54";
                string downlinkDataString = CreatePackDownloadLink(typePackage, numeroEnvios, tempoTransmissao, tipoEnvio, tensaoMinima, dashboard.Package.Substring(8, 2), dashboard.Bits != null ? dashboard.Bits.Downlink : dashboard.DownloadLink.TipoEnvio);

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
                if (ex.InnerException != null)
                    _log.Log($"Erro enviar alterações do dispositivo {device.Id}, erro {ex.InnerException.Message}.");
                else
                    _log.Log($"Erro enviar alterações do dispositivo {device.Id}, erro {ex.Message}.");
                // throw new Box.Common.BoxLogicException("Erro ao enviar as informações de alteração do dispositivo, consulte o log para mais informações.");
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
            List<Message> listMessagesReport = new List<Message>();
            // List<Message> listMessagesB987 = new List<Message>();
            foreach (var item in sigfoxMessage.data)
            {
                try
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

                    listMessagesReport.Add(newMessage);

                    // if (int.Parse(newMessage.TypePackage) == (int)Models.PackagesEnum.B987)
                    // {
                    //     if (VerifyDeviceIsProjectB987(newMessage.DeviceId))
                    //         listMessagesB987.Add(newMessage);
                    // }
                    
                    if (!VerifyExistMessage(item.time.ToString()))
                        listMessages.Add(newMessage);
                }
                catch (System.Exception ex)
                {
                    _log.Log("Erro.SigfoxSaveMessages.", ex.Message, true);
                    continue;
                }
            }

            try
            {
                if (listMessagesReport.Count > 0)
                {
                    try
                    {                     
                        ProcessReportResil(listMessagesReport);
                    }
                    catch (System.Exception ex)
                    {
                        _log.Log("SigFox.SigfoxSaveMessages.ProcessReportResil: Erro ao processar relatório.", ex.Message);
                    }

                    try
                    {
                         ProcessDataB972(listMessagesReport);
                    }
                    catch (System.Exception ex)
                    {
                        _log.Log("SigFox.SigfoxSaveMessages.ProcessDataB972: Erro ao processar dados B972.", ex.Message);
                    }

                    // try
                    // {
                    //     listMessagesB987 = listMessagesB987.OrderBy(c => c.OperationDate).ToList();
                    //     ProcessDataB987(listMessagesB987); // Projeto MCond
                    // }
                    // catch (System.Exception ex)
                    // {
                    //     _log.Log("SigFox.SigfoxSaveMessages.ProcessDataB987: Erro ao processar dados B987.", ex.Message);
                    // }

                }
            }
            catch (System.Exception ex)
            {
                _log.Log("Erro ao processar report resil.", ex.Message, true);
            }

            if (listMessages.Count > 0)
            {
                _context.Messages.AddRange(listMessages);
                _context.SaveChanges();
                _log.Log($"Dados do dispositivo criados.");
            }
        }

        // internal void ProcessDataB972(List<Message> messages)
        // {
        //     _log.Log("Processando dados do projeto B972.", "ProcessDataB972", true);

        //     List<B972> listB972 = new List<B972>();
        //     int seconds = 60;
        //     int miliseconds = 1000;
        //     int multiplied = 10;
        //     var projectDevices = GetDevicesByProjectCode(Utils.EnumToAnnotationText(ProjectCode.B972));
        //     var devices = projectDevices != null ? projectDevices.Select(s => s.DeviceId).ToArray() : null;

        //     if (devices == null)
        //         return;

        //     messages = messages
        //     .Where(c =>
        //         c.TypePackage.Equals("84") ||
        //         c.TypePackage.Equals("85") ||
        //         c.TypePackage.Equals("86") ||
        //         c.TypePackage.Equals("87"))
        //     .ToList();
        //     messages = messages.Where(c => devices.Any(a => c.DeviceId == a)).OrderBy(o => o.Time).ToList();

        //     foreach (var message in messages)
        //     {
        //         var oldB972 = GetB972(message.DeviceId, message.Time);
        //         if (oldB972)
        //             continue;
                
        //         var oldB972_85 = GetB972_85(message.DeviceId, message.Time);
        //         if (oldB972_85)
        //             continue;

        //         if (Convert.ToInt32(message.TypePackage) == (int)PackagesEnum.B972_84)
        //         {
        //             long currentTime = message.Time;

        //             listB972.Add(new B972()
        //             {
        //                 DeviceId = message.DeviceId.TrimStart().TrimEnd(),
        //                 Time = message.Time,
        //                 Position = 1,
        //                 Flow = message.VazaoTempo1 * multiplied,
        //                 Velocity = null,
        //                 RSSI = null,
        //                 Source = "SIGFOX",
        //                 Lqi = message.Lqi,
        //                 Iq = B972_IQ_Enum.Partial,
        //                 Date = message.Date
        //             });

        //             currentTime = (currentTime - (5 * seconds * miliseconds));
        //             listB972.Add(new B972()
        //             {
        //                 DeviceId = message.DeviceId.TrimStart().TrimEnd(),
        //                 Time = currentTime,
        //                 Position = 2,
        //                 Flow = message.VazaoTempo2 * multiplied,
        //                 Velocity = null,
        //                 RSSI = null,
        //                 Source = "SIGFOX",
        //                 Lqi = message.Lqi,
        //                 Iq = B972_IQ_Enum.Partial,
        //                 Date = Utils.Timestamp_Milisecodns_ToDateTime_UTC(currentTime)
        //             });

        //             currentTime = (currentTime - (5 * seconds * miliseconds));
        //             listB972.Add(new B972()
        //             {
        //                 DeviceId = message.DeviceId.TrimStart().TrimEnd(),
        //                 Time = currentTime,
        //                 Position = 3,
        //                 Flow = message.VazaoTempo3 * multiplied,
        //                 Velocity = null,
        //                 RSSI = null,
        //                 Source = "SIGFOX",
        //                 Lqi = message.Lqi,
        //                 Iq = B972_IQ_Enum.Partial,
        //                 Date = Utils.Timestamp_Milisecodns_ToDateTime_UTC(currentTime)
        //             });

        //             currentTime = (currentTime - (5 * seconds * miliseconds));
        //             listB972.Add(new B972()
        //             {
        //                 DeviceId = message.DeviceId.TrimStart().TrimEnd(),
        //                 Time = currentTime,
        //                 Position = 4,
        //                 Flow = message.VazaoTempo4 * multiplied,
        //                 Velocity = null,
        //                 RSSI = null,
        //                 Source = "SIGFOX",
        //                 Lqi = message.Lqi,
        //                 Iq = B972_IQ_Enum.Partial,
        //                 Date = Utils.Timestamp_Milisecodns_ToDateTime_UTC(currentTime)
        //             });

        //             currentTime = (currentTime - (5 * seconds * miliseconds));
        //             listB972.Add(new B972()
        //             {
        //                 DeviceId = message.DeviceId.TrimStart().TrimEnd(),
        //                 Time = currentTime,
        //                 Position = 5,
        //                 Flow = message.VazaoTempo5 * multiplied,
        //                 Velocity = null,
        //                 RSSI = null,
        //                 Source = "SIGFOX",
        //                 Lqi = message.Lqi,
        //                 Iq = B972_IQ_Enum.Partial,
        //                 Date = Utils.Timestamp_Milisecodns_ToDateTime_UTC(currentTime)
        //             });

        //             SaveB972(listB972);
        //             listB972 = new List<B972>();
        //         }

        //         if (Convert.ToInt32(message.TypePackage) == (int)PackagesEnum.B972_85)
        //         {
        //             var b972_p1 = GetLastB972(message.DeviceId, 1);
        //             if (b972_p1 == null)
        //                 continue;

        //             long currentTime = message.Time;
        //             b972_p1.Velocity = message.VelocidadeTempo1 * multiplied;
        //             b972_p1.VelocityTime = currentTime;
        //             b972_p1.Iq = B972_IQ_Enum.Complete;
        //             // listB972.Add(b972_p1);
        //             UpdateB972(b972_p1);



        //             var b972_p2= GetLastB972(message.DeviceId, 2);
        //             if (b972_p2 == null)
        //                 continue;
                    
        //             currentTime = (currentTime - (5 * seconds * miliseconds));
        //             b972_p2.Velocity = message.VelocidadeTempo2 * multiplied;
        //             b972_p2.VelocityTime = currentTime;
        //             b972_p2.Iq = B972_IQ_Enum.Complete;
        //             // listB972.Add(b972_p2);
        //             UpdateB972(b972_p2);



        //             var b972_p3 = GetLastB972(message.DeviceId, 3);
        //             if (b972_p3 == null)
        //                 continue;
                    
        //             currentTime = (currentTime - (5 * seconds * miliseconds));
        //             b972_p3.Velocity = message.VelocidadeTempo3 * multiplied;
        //             b972_p3.VelocityTime = currentTime;
        //             b972_p3.Iq = B972_IQ_Enum.Complete;
        //             // listB972.Add(b972_p3);
        //             UpdateB972(b972_p3);



        //             var b972_p4 = GetLastB972(message.DeviceId, 4);
        //             if (b972_p4 == null)
        //                 continue;
                    
        //             currentTime = (currentTime - (5 * seconds * miliseconds));
        //             b972_p4.Velocity = message.VelocidadeTempo4 * multiplied;
        //             b972_p4.VelocityTime = currentTime;
        //             b972_p4.Iq = B972_IQ_Enum.Complete;
        //             // listB972.Add(b972_p4);
        //             UpdateB972(b972_p4);




        //             var b972_p5 = GetLastB972(message.DeviceId, 5);
        //             if (b972_p5 == null)
        //                 continue;
                    
        //             currentTime = (currentTime - (5 * seconds * miliseconds));
        //             b972_p5.Velocity = message.VelocidadeTempo5 * multiplied;
        //             b972_p5.VelocityTime = currentTime;
        //             b972_p5.Iq = B972_IQ_Enum.Complete;
        //             // listB972.Add(b972_p5);
        //             UpdateB972(b972_p5);
        //         }

        //         if (Convert.ToInt32(message.TypePackage) == (int)PackagesEnum.B972_86)
        //         {
        //             var _total = Utils.FromFloatSafe(message.Total_P982U4);
        //             var _parcial = Utils.FromFloatSafe(message.Parcial_P982U4);

        //             UpdateB972_P982U4_86(message.DeviceId, (decimal)_total, (decimal)_parcial, message.Temperatura_P982U4, message.Time);
        //         }

        //         if (Convert.ToInt32(message.TypePackage) == (int)PackagesEnum.B972_87)
        //         {
        //             UpdateB972_P982U4_87(message.DeviceId, message.Flags_P982U4, message.Quality_P982U4, message.Time);
        //         }
        //     }

        //     _log.Log("Finalizando dados do projeto B972.", "ProcessDataB972", true);
        // }

        public IEnumerable<ProjectDevice> GetDevicesByProjectCode(string projectCode)
        {
            return _context.VW_DevicesByProjectCode.Where(c => c.Code == projectCode).ToArray();
        }

        // internal void ProcessReportResil(List<Message> messages)
        // {
        //     _log.Log("Processando dados do relatório resil.", "Report.Resil", true);

        //     List<ReportResil> listReports = new List<ReportResil>();
        //     messages = messages.Where(c => c.TypePackage.Equals("23") || c.TypePackage.Equals("24")).ToList();
        //     int finalHour = 23;

        //     DateTime dayInitial = messages.Min(m => m.OperationDate.Value);
        //     DateTime dayFinal = messages.Max(m => m.OperationDate.Value);
        //     for (DateTime x = dayInitial; x <= dayFinal.AddDays(1); x = x.AddDays(1))
        //     {
        //         var tmpMessages = messages.Where(c => c.OperationDate.Value.Day == x.Day && c.OperationDate.Value.Month == x.Month && c.OperationDate.Value.Year == x.Year).ToList();

        //         for (int i = 0; i <= finalHour; i++)
        //         {
        //             try
        //             {
        //                 var currentMessages = tmpMessages.Where(c => c.OperationDate.Value.Hour == i).ToList();
        //                 if (currentMessages.Count == 0)
        //                     continue;

        //                 var currentMessage23 = currentMessages.OrderBy(o => o.Time).FirstOrDefault(f => f.TypePackage.Equals("23"));
        //                 var currentMessage24 = currentMessages.OrderBy(o => o.Time).FirstOrDefault(f => f.TypePackage.Equals("24"));

        //                 if (currentMessage23 == null || currentMessage24 == null)
        //                     continue;

        //                 var currentDashboard = CreateDashboard_Pack23_24ViewModel(currentMessage23, currentMessage24);

        //                 listReports.Add(new ReportResil()
        //                 {
        //                     Id = currentDashboard.Time.ToString(),
        //                     DeviceId = currentDashboard.DeviceId,
        //                     Time = currentDashboard.Time,
        //                     Day = currentDashboard.Date.Day,
        //                     Month = currentDashboard.Date.Month,
        //                     Year = currentDashboard.Date.Year,
        //                     Hour = currentDashboard.Date. Hour,
        //                     Minute = currentDashboard.Date.Minute,
        //                     ConsumoHora = decimal.Parse(currentDashboard.ConsumoAgua),
        //                     ConsumoDia = decimal.Parse(currentDashboard.ConsumoDia),
        //                     ConsumoSemana = decimal.Parse(currentDashboard.ConsumoSemana),
        //                     ConsumoMes = decimal.Parse(currentDashboard.ConsumoMes),
        //                     Fluxo = decimal.Parse(currentDashboard.FluxoAgua),
        //                     Modo = currentDashboard.Modo,
        //                     Estado = currentDashboard.Estado,
        //                     Valvula = currentDashboard.Valvula,
        //                     Date = currentDashboard.Date
        //                 });
        //             }
        //             catch (System.Exception ex)
        //             {
        //                 _log.Log("Erro ao processar report resil.", ex.Message, true);
        //                 continue;
        //             }
        //         }

        //         // x = x.AddDays(1);
        //     }

        //     SaveReportResil(listReports);
        // }

        // internal void SaveB972(List<B972> b972)
        // {
        //     if (b972.Count > 0)
        //     {
        //         foreach (var item in b972)
        //         {
        //             bool oldB972 = GetB972(item.DeviceId, item.Time);
        //             if (!oldB972)
        //             {
        //                 _context.B972s.Add(item);
        //                 _context.SaveChanges();
        //                 _log.Log("B972 criado.");
        //             }
        //         }
        //     }
        // }

        // internal void UpdateB972(B972 b972)
        // {
        //     try
        //     {
        //          _context.Entry<Models.B972>(b972).State = EntityState.Modified;
        //          _context.SaveChanges();
        //     }
        //     catch (System.Exception ex)
        //     {
        //         _log.Log("Sigfox.UpdateB972: Error update.", ex.Message, true);
        //     }
        // }

        // internal void UpdateListB972(List<B972> b972)
        // {
        //     try
        //     {     
        //         if (b972.Count > 0)
        //         {
        //             _context.UpdateRange(b972);
        //             _context.SaveChanges();
        //         }
        //     }
        //     catch (System.Exception ex)
        //     {
        //         _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
        //     }
        // }

        // internal void UpdateB972_P982U4_86(string deviceId, decimal total, decimal partial, decimal temperature, long time)
        // {
        //     var listB972 = _context.B972s.Where(c => c.DeviceId == deviceId && (c.Total == null || c.Partial == null || c.Temperature == null)).ToArray();
        //     foreach (var item in listB972)
        //     {
        //         try
        //         {     
        //             item.Total = total;
        //             item.Partial = partial;
        //             item.Temperature = temperature/10;
        //             item.Pack86Time = time;
                    
        //             _context.Entry<Models.B972>(item).State = EntityState.Detached;
        //             _context.Entry<Models.B972>(item).State = EntityState.Modified;

        //             // _context.Entry(oldRole).State = EntityState.Modified;
        //             // _context.Entry(item).CurrentValues.SetValues(item);

        //             // _context.Update(item);
        //             _context.SaveChanges();

        //             // _context.B972s.Attach(item);
        //             // var entry = _context.Entry(item);
        //             // entry.State = EntityState.Modified;
        //             // _context.SaveChanges();
        //         }
        //         catch (System.Exception ex)
        //         {
        //             _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
        //         }
        //     }

        //     // try
        //     // {
        //     //      _context.UpdateRange(listB972);
        //     //      _context.SaveChanges();
        //     // }
        //     // catch (System.Exception ex)
        //     // {
        //     //     _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
        //     // }
        // }

        // internal void UpdateB972_P982U4_87(string deviceId, string flags, decimal quality, long time)
        // {
        //     var listB972 = _context.B972s.Where(c => c.DeviceId == deviceId && (c.Flags == null || c.Quality == null)).ToArray();
        //     foreach (var item in listB972)
        //     {
        //         try
        //         {     
        //             item.Flags = flags;
        //             item.Quality = quality;
        //             item.Pack87Time = time;

        //             _context.Entry<Models.B972>(item).State = EntityState.Detached;
        //             _context.Entry<Models.B972>(item).State = EntityState.Modified;

        //             // _context.Update(item);
        //             _context.SaveChanges();

        //             // _context.B972s.Attach(item);
        //             // var entry = _context.Entry(item);
        //             // entry.State = EntityState.Modified;
        //             // _context.SaveChanges();

        //         }
        //         catch (System.Exception ex)
        //         {
        //             _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
        //         }
        //     }

        //     // try
        //     // {
        //     //      _context.UpdateRange(listB972);
        //     //      _context.SaveChanges();
        //     // }
        //     // catch (System.Exception ex)
        //     // {
        //     //     _log.Log("Sigfox.UpdateB972: Error update range.", ex.Message, true);
        //     // }
        // }

        // internal void SaveReportResil(List<ReportResil> reportsResil)
        // {
        //     if (reportsResil.Count > 0)
        //     {
        //         foreach (var item in reportsResil)
        //         {
        //             bool oldReport = GetReportResil(item);
        //             if (!oldReport)
        //             {
        //                 _context.ReportResil.Add(item);
        //                 _context.SaveChanges();
        //                 _log.Log("Report Resil criado.");
        //             }
        //         }
        //     }
        // }

        // internal B972 GetLastB972(string deviceId, int position)
        // {
        //     return _context.B972s.OrderByDescending(o => o.Time).FirstOrDefault(c => c.DeviceId == deviceId && c.Position == position);
        // }

        // internal bool GetB972(string deviceId, long time)
        // {
        //     return _context.B972s.AsNoTracking().Any(c => c.DeviceId == deviceId && c.Time == time);
        // }

        // internal bool GetB972_85(string deviceId, long time)
        // {
        //     return _context.B972s.AsNoTracking().Any(c => c.DeviceId == deviceId && c.VelocityTime == time);
        // }

        // internal bool GetReportResil(ReportResil reportsResil)
        // {
        //     return _context.ReportResil.AsNoTracking().Any(c => c.DeviceId == reportsResil.DeviceId && c.Day == reportsResil.Day && c.Month == reportsResil.Month && c.Year == reportsResil.Year && c.Hour == reportsResil.Hour);
        // }

        public void UpdateOperationDateFromMessages()
        {
            var messages = _context.Messages.AsNoTracking().Where(c => c.OperationDate.Value == null).ToList();
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
            IQueryable<Message> messages = _context.Messages.AsNoTracking().Where(w => w.Id == id);
            return messages.Count() > 0;
        }

        private string CreatePack56DownloadLink(string typePackage, DateTime dataHora)
        {
            string start = typePackage;
            string b1 = Utils.ZerosForLeft(Utils.DecimalToHexa(dataHora.Hour.ToString()), 2);
            string b2 = Utils.ZerosForLeft(Utils.DecimalToHexa(dataHora.Minute.ToString()), 2);
            string b3 = Utils.ZerosForLeft(Utils.DecimalToHexa(dataHora.Second.ToString()), 2);
            string b4 = Utils.ZerosForLeft(Utils.DecimalToHexa(dataHora.Day.ToString()), 2);
            string b5 = Utils.ZerosForLeft(Utils.DecimalToHexa(dataHora.Month.ToString()), 2);
            string b6 = Utils.ZerosForLeft(Utils.DecimalToHexa(dataHora.Year.ToString()), 2);

            return $"{start}{b1}{b2}{b3}{b4}{b5}{b6}";
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

        public async Task DownloadMessageByDevice(string deviceId)
        {
            _log.Log($"Baixando dados do dispositivo {deviceId}.");

            for (int i = 0; i < _sigfoxLogins.Length; i++)
            {
                var messages = await SigfoxGetMessagesByDevice(deviceId, _sigfoxLogins[i], _sigfoxPasswords[i]);
                if (messages.data.Length > 0)
                    SigfoxSaveMessages(messages);
            }

            _log.Log($"Finalizado download de dados do dispositivo {deviceId}.");
        }

    }
}