using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        public async Task<bool> SendClientBilling(Client client, ClientBilling clientBilling)
        {
            string subject = String.Format("{0} - {1}", _boxSettings.APPLICATION_NAME, "Faturamento");
            string dataVencimento = clientBilling.PaymentDueDate?.ToString("dd/MM/yyyy");

            // create the template
            dynamic data = _templateService.CreateBasicModel();
            data.subject = subject;
            data.paymentDueDate = dataVencimento;
            var body = await _templateService.RenderTemplate("ClientBilling", data, lang: "pt");

            try
            {
                await _emailSender.SendEmailAsync(
                       client.Email,
                       subject,
                       body);

                _log.Log($"E-mail enviado para o cliente {client.Email} com faturamento que vence em {dataVencimento}.", saveParameters: false);
                return true;
            }
            catch (Exception ex)
            {
                _log.Log($"Erro no envio do faturamento do cliente {client.Name} com vencimento em {dataVencimento}.", ex.Message, saveParameters: false);
                return false;
            }
        }

        public async Task SendEmailLockedDevice(string[] emails, string deviceId, long time)
        {
            string subject = $"{_boxSettings.APPLICATION_NAME} - Bloqueio do dispositivo {deviceId}";

            // create the template
            dynamic data = _templateService.CreateBasicModel();
            data.subject = subject;
            data.deviceId = deviceId;
            data.date = Utils.TimeStampSecondsToDateTime(time).ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss");
            var body = _templateService.RenderTemplate("AlertDeviceLocked", data, lang: "pt");

            // SendEmail(emails, subject, body, "bloqueio do dispositivo", deviceId);
            foreach (var email in emails)
            {            
                await _emailSender.SendEmailAsync(email, subject, body);
            }
        }

        // public async Task SendStateChangedDevice(string[] emails, Message message)
        // {
        //     string subject = $"{_boxSettings.APPLICATION_NAME} - Alteração de status do dispositivo {message.DeviceId}";

        //     // create the template
        //     dynamic data = _templateService.CreateBasicModel();
        //     data.subject = subject;
        //     data.deviceId = message.DeviceId;
        //     data.state = message.CalhaAlerta;
        //     data.date = message.Date.ToShortDateString();
        //     // var body = await _templateService.RenderTemplate("AlertDeviceStateChanged", data, null, "pt");
        //     string body = "teste";

        //     foreach (var email in emails)
        //     {            
        //         await _emailSender.SendEmailAsync(email, subject, body);
        //     }

        // }

        public async Task NotifyStateAlertsChangedDeviceB987(string[] emails, MCond currentMessage, MCond lastMessage, string deviceFather, string[] phoneNumbers)
        {
            string subject = $"{_boxSettings.APPLICATION_NAME} - Alteração de alertas do dispositivo {currentMessage.DeviceId}";
            var device = GetDeviceRegistration(deviceFather);

            // create the template
            CultureInfo ptbr = new CultureInfo("pt-BR");
            dynamic data = _templateService.CreateBasicModel();
            data.subject = subject;
            data.deviceName = device != null ? device.NickName : deviceFather;
            data.currentMessage = currentMessage;
            data.lastMessage = lastMessage;
            var emailBody = await _templateService.RenderTemplate("AlertDeviceStateAlertsB987", data, null, "pt");

            foreach (var email in emails)
            {            
                await _emailSender.SendEmailAsync(email, subject, emailBody);
                _log.Log("Email:SendStateAlertsChangedDeviceB987", JsonConvert.SerializeObject(emailBody), true);
            }

            var whatsappBody = "PORTAL RADIODADOS ANALÍTICA" + Environment.NewLine + Environment.NewLine + "Atenção!" + Environment.NewLine + Environment.NewLine +
            "Veja abaixo as informações do alerta" + Environment.NewLine + $"Dispositivo: {data.deviceName}" + Environment.NewLine + Environment.NewLine;

            if (currentMessage.SupAlarmLevelMax != lastMessage.SupAlarmLevelMax)
            {
                whatsappBody += "Alerta de nível alto: " + (currentMessage.SupAlarmLevelMax ? "Alto" : "Normal") + Environment.NewLine;
            }
            if (currentMessage.SupAlarmLevelMin != lastMessage.SupAlarmLevelMin)
            {
                whatsappBody += "Alerta de nível baixo: " + (currentMessage.SupAlarmLevelMin ? "Baixo" : "Normal") + Environment.NewLine;
            }
            if (currentMessage.SupStateBomb != lastMessage.SupStateBomb)
            {
                whatsappBody += "Estado da Bomba: " + (currentMessage.SupStateBomb ? "Ligada" : "Normal") + Environment.NewLine;
            }
            if (currentMessage.PortFireAlarm != lastMessage.PortFireAlarm)
            {
                whatsappBody += "Alarme de incêndio ativado: " + (currentMessage.PortFireAlarm ? "Não" : "Sim") + Environment.NewLine;
            }
            if (currentMessage.PortFireState != lastMessage.PortFireState)
            {
                whatsappBody += "Incêndio: " + (currentMessage.PortFireState ? "Acionado" : "Normal") + Environment.NewLine;
            }
            if (currentMessage.PortIvaAlarm != lastMessage.PortIvaAlarm)
            {
                whatsappBody += "Alarme IVA ativado: " + (currentMessage.PortIvaAlarm ? "Não" : "Sim") + Environment.NewLine;
            }
            if (currentMessage.PortIvaState != lastMessage.PortIvaState)
            {
                whatsappBody += "IVA: " + (currentMessage.PortIvaState ? "Acionado" : "Normal") + Environment.NewLine;
            }

            foreach (var phone in phoneNumbers)
            {
                await SendWhatsappMessage(data.deviceName, whatsappBody, phone);
                _log.Log("Whatsapp:SendStateAlertsChangedDeviceB987", JsonConvert.SerializeObject(whatsappBody), true);
            }
        }

        public async Task SendStateChangedDeviceTSP(string[] emails, Message message, string lastState)
        {
            string subject = $"{_boxSettings.APPLICATION_NAME} - Alteração de status do dispositivo {message.DeviceId}";
            var currentDashboard = GetDashboard(message.DeviceId, null, 0, null, 0, Utils.EnumToAnnotationText(ProjectCode.B972_P));

            // create the template
            CultureInfo ptbr = new CultureInfo("pt-BR");
            dynamic data = _templateService.CreateBasicModel();
            data.subject = subject;
            data.message = currentDashboard;
            data.lastState = lastState;
            var emailBody = await _templateService.RenderTemplate("AlertDeviceStateChangedTSP", data, null, "pt");

            foreach (var email in emails)
            {            
                await _emailSender.SendEmailAsync(email, subject, emailBody);
                _log.Log("SendStateChangedDeviceTSP", JsonConvert.SerializeObject(emailBody), true);
            }
        }

        public async Task SendStateChangedDeviceTRM(string[] emails, Message message, string lastState)
        {
            string subject = $"{_boxSettings.APPLICATION_NAME} - Alteração de status do dispositivo {message.DeviceId}";
            var currentDashboard = GetDashboard(message.DeviceId, null, 0, null, 0, Utils.EnumToAnnotationText(ProjectCode.B972_P));

            // create the template
            CultureInfo ptbr = new CultureInfo("pt-BR");
            dynamic data = _templateService.CreateBasicModel();
            data.subject = subject;
            data.message = currentDashboard;
            data.lastState = lastState;
            var emailBody = await _templateService.RenderTemplate("AlertDeviceStateChangedTRM", data, null, "pt");

            foreach (var email in emails)
            {            
                await _emailSender.SendEmailAsync(email, subject, emailBody);
                _log.Log("SendStateChangedDeviceTRM", JsonConvert.SerializeObject(emailBody), true);
            }
        }

        public async Task SendStateChangedDeviceTQA(string[] emails, Message lastMessage, Message currentMessage)
        {
            string subject = $"{_boxSettings.APPLICATION_NAME} - Alteração de status do dispositivo {currentMessage.DeviceId}";
            var currentDashboard = GetDashboard(currentMessage.DeviceId, null, 0, null, 0, Utils.EnumToAnnotationText(ProjectCode.B981));

            // create the template
            CultureInfo ptbr = new CultureInfo("pt-BR"); 
            dynamic data = _templateService.CreateBasicModel();
            data.subject = subject;
            data.message = currentDashboard;
            data.lastRele7 = null;
            data.lastRele6 = null;
            data.lastRele5 = null;
            data.lastRele4 = null;
            data.lastRele3 = null;
            data.lastRele2 = null;
            data.lastRele1 = null;

            if (lastMessage.ReleBoolean.Rele7 != currentMessage.ReleBoolean.Rele7)
                data.lastRele7 = lastMessage.ReleBoolean.Rele7;

            if (lastMessage.ReleBoolean.Rele6 != currentMessage.ReleBoolean.Rele6)
                data.lastRele6 = lastMessage.ReleBoolean.Rele6;

            if (lastMessage.ReleBoolean.Rele5 != currentMessage.ReleBoolean.Rele5)
                data.lastRele5 = lastMessage.ReleBoolean.Rele5;
            
            if (lastMessage.ReleBoolean.Rele4 != currentMessage.ReleBoolean.Rele4)
                data.lastRele4 = lastMessage.ReleBoolean.Rele4;

            if (lastMessage.ReleBoolean.Rele3 != currentMessage.ReleBoolean.Rele3)
                data.lastRele3 = lastMessage.ReleBoolean.Rele3;

            if (lastMessage.ReleBoolean.Rele2 != currentMessage.ReleBoolean.Rele2)
                data.lastRele2 = lastMessage.ReleBoolean.Rele2;

            if (lastMessage.ReleBoolean.Rele1 != currentMessage.ReleBoolean.Rele1)
                data.lastRele1 = lastMessage.ReleBoolean.Rele1;

            // if (lastMessage.ReleBoolean.Rele7 != currentMessage.ReleBoolean.Rele7)
            //     data.lastRele7 = lastMessage.ReleBoolean.Rele7 ? "Ligado" : "Desligado";

            // if (lastMessage.ReleBoolean.Rele6 != currentMessage.ReleBoolean.Rele6)
            //     data.lastRele6 = lastMessage.ReleBoolean.Rele6 ? "Ligado" : "Desligado";

            // if (lastMessage.ReleBoolean.Rele5 != currentMessage.ReleBoolean.Rele5)
            //     data.lastRele5 = lastMessage.ReleBoolean.Rele5 ? "Ligado" : "Desligado";
            
            // if (lastMessage.ReleBoolean.Rele4 != currentMessage.ReleBoolean.Rele4)
            //     data.lastRele4 = lastMessage.ReleBoolean.Rele4 ? "Ligado" : "Desligado";

            // if (lastMessage.ReleBoolean.Rele3 != currentMessage.ReleBoolean.Rele3)
            //     data.lastRele3 = lastMessage.ReleBoolean.Rele3 ? "Ligado" : "Desligado";

            // if (lastMessage.ReleBoolean.Rele2 != currentMessage.ReleBoolean.Rele2)
            //     data.lastRele2 = lastMessage.ReleBoolean.Rele2 ? "Ligado" : "Desligado";

            // if (lastMessage.ReleBoolean.Rele1 != currentMessage.ReleBoolean.Rele1)
            //     data.lastRele1 = lastMessage.ReleBoolean.Rele1 ? "Ligado" : "Desligado";
            
            var emailBody = await _templateService.RenderTemplate("AlertDeviceStateChangedTQA", data, null, "pt");
            foreach (var email in emails)
            {            
                await _emailSender.SendEmailAsync(email, subject, emailBody);
                _log.Log("SendStateChangedDeviceTQA", JsonConvert.SerializeObject(emailBody), true);
            }
        }
        // internal async Task NotifyFencerRegistrationSuccessAsync(Models.FencerRegistrationRequest fencerRegistrationRequest, string password)
        // {
        //     try
        //     {
        //         CultureInfo ptbr = new CultureInfo("pt-BR");
        //         dynamic data = _templateService.CreateBasicModel();
        //         data.email = fencerRegistrationRequest.Email;
        //         data.password = password;
        //         string emailBody = await _templateService.RenderTemplate("NotifyApprovedFencer", data);

        //         await _emailSender.SendEmailAsync(fencerRegistrationRequest.Email, "Cadastro aprovado", emailBody);
        //         await _appService.LogDebugAsync("RegistrationService.NotifyFencerRegistrationSuccessAsync: Sent");
        //     }
        //     catch (System.Exception ex)
        //     {
        //         await _appService.LogErrorAsync("RegistrationService.NotifyFencerRegistrationSuccessAsync: Error notify approvers fencer registration.", ex.Message.Truncate(300), true, new { fencerRegistrationRequest });
        //     }
        // }

        public async Task SendEmailReceiveLocationDevice(string[] emails, string deviceId, long time, string deviceType, string fixedLat, string fixedLng)
        {
            string subject = $"{_boxSettings.APPLICATION_NAME} - Localização do dispositivo {deviceId}";

            // create the template
            dynamic data = _templateService.CreateBasicModel();
            data.subject = subject;
            data.deviceId = deviceId;
            data.latitude = fixedLat;
            data.longitude = fixedLng;
            data.date = Utils.TimeStampSecondsToDateTime(time).ToUniversalTime().ToString("dd/MM/yyyy hh:mm:ss");
            var body = _templateService.RenderTemplate("AlertLocationDeviceCallBack", data, lang: "pt");

            // SendEmail(emails, subject, body, "localização do dispositivo", deviceId);
            foreach (var email in emails)
            {            
                await _emailSender.SendEmailAsync(email, subject, body);
            }
        }

        // public Task SendEmail(string[] emails, string subject, string body, string typeAlert, string deviceId)
        // {
        //     return Task.Run(async () =>
        //     {
        //     try
        //     {
        //         foreach (var email in emails)
        //         {
        //             if (!string.IsNullOrWhiteSpace(email))
        //             {
        //                 await _emailSender.SendEmailAsync(
        //                         email,
        //                         subject,
        //                         body);
        //             }

        //             _log.Log($"E-mail enviado para {email}, de {typeAlert} {deviceId}.");
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         _log.Log($"Erro no envio de {typeAlert} {deviceId}.", ex.Message);
        //     }
        //     });
        // }

        public Task SendSMSLockedDevice(string[] numbers, string deviceId, long time)
        {
            return Task.Run(async () =>
            {
                try
                {
                    string _dataHora = Utils.TimeStampSecondsToDateTime(time).ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss");
                    string _subject = $"{_boxSettings.APPLICATION_NAME} - Bloqueio do dispositivo {deviceId} as {_dataHora}.";
                    string accountSid = _sgiSettings.SERVICE_SMS_ACCOUNTID;
                    string authToken = _sgiSettings.SERVICE_SMS_TOKEN;

                    foreach (var numberTo in numbers)
                    {
                        HttpClient wc = new HttpClient();
                        wc.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Basic", Utils.CreateBasicOauth(accountSid, authToken));
                        var parameters = new Dictionary<string, string>();
                        parameters.Add("Content-Type", "multipart/form-data;");
                        parameters.Add("Body", _subject);
                        parameters.Add("From", _sgiSettings.SERVICE_SMS_NUMBER_FROM);

                        if (!string.IsNullOrWhiteSpace(numberTo))
                        {
                            try
                            {
                                parameters.Add("To", numberTo);
                                var req = new HttpRequestMessage(HttpMethod.Post, string.Format(_sgiSettings.SERVICE_SMS_WHATSAPP_URL, accountSid))
                                {
                                    Content = new FormUrlEncodedContent(parameters)
                                };
                                HttpResponseMessage msg = await wc.SendAsync(req);

                                string jsonResult = string.Empty;
                                if (msg.StatusCode == System.Net.HttpStatusCode.Created)
                                    _log.Log($"SMS enviado para o número {numberTo}, de alerta de bloqueio do dispositivo {deviceId}. {msg.RequestMessage}");
                                else
                                    _log.Log($"Erro no envio de SMS enviado para o número {numberTo}, de alerta de bloqueio do dispositivo {deviceId}. {msg.RequestMessage}");
                            }
                            catch (System.Exception ex)
                            {
                                _log.Log($"Erro no envio de SMS enviado para o número {numberTo}, de alerta de bloqueio do dispositivo {deviceId}. {ex.Message}");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    _log.Log($"Erro no envio de SMS de alerta de bloqueio do dispositivo {deviceId}.", ex.Message);
                }
            });
        }

        // public Task SendWhatsAppLockedDevice(string[] numbers, string deviceId, long time)
        // {
        //     return Task.Run(async () =>
        //     {
        //         try
        //         {
        //             string _dataHora = Utils.TimeStampSecondsToDateTime(time).ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss");
        //             string _subject = $"{_boxSettings.APPLICATION_NAME} - Bloqueio do dispositivo {deviceId} as {_dataHora}.";

        //             foreach (var numberTo in numbers)
        //             {
        //                 await CreateMessageWhatsApp(deviceId, _subject, numberTo);
        //             }
        //         }
        //         catch (System.Exception ex)
        //         {
        //             _log.Log($"Erro no envio de WhatsApp de alerta de bloqueio do dispositivo {deviceId}.", ex.Message);
        //         }
        //     });
        // }

        // public async Task CreateMessageWhatsApp(string deviceId, string _subject, string numberTo)
        // {
        //     HttpClient wc = new HttpClient();
        //     wc.DefaultRequestHeaders.Authorization =
        //     new AuthenticationHeaderValue("Basic", Utils.CreateBasicOauth(_sgiSettings.SERVICE_SMS_ACCOUNTID, _sgiSettings.SERVICE_SMS_TOKEN));
        //     var parameters = new Dictionary<string, string>();
        //     parameters.Add("Content-Type", "multipart/form-data;");
        //     parameters.Add("Body", _subject);
        //     parameters.Add("From", $"whatsapp:{_sgiSettings.SERVICE_WHATSAPP_NUMBER_FROM}");

        //     if (!string.IsNullOrWhiteSpace(numberTo))
        //     {
        //         try
        //         {
        //             parameters.Add("To", $"whatsapp:{numberTo}");
        //             var req = new HttpRequestMessage(HttpMethod.Post, string.Format(_sgiSettings.SERVICE_SMS_WHATSAPP_URL, _sgiSettings.SERVICE_SMS_ACCOUNTID))
        //             {
        //                 Content = new FormUrlEncodedContent(parameters)
        //             };
        //             HttpResponseMessage msg = await wc.SendAsync(req);

        //             string jsonResult = string.Empty;
        //             if (msg.StatusCode == System.Net.HttpStatusCode.Created)
        //                 _log.Log($"WhatsApp enviado para o número {numberTo}, de alerta de bloqueio do dispositivo {deviceId}. {msg.RequestMessage}");
        //             else
        //                 _log.Log($"Erro no envio de WhatsApp enviado para o número {numberTo}, de alerta de bloqueio do dispositivo {deviceId}. {msg.RequestMessage}");
        //         }
        //         catch (System.Exception ex)
        //         {
        //             _log.Log($"Erro no envio de WhatsApp enviado para o número {numberTo}, de alerta de bloqueio do dispositivo {deviceId}. {ex.Message}");
        //         }
        //     }
        // }

        public async Task SendWhatsappMessage(string _deviceId, string _body, string _numberTo)
        {
            try
            {
                string accountSid = _sgiSettings.SERVICE_SMS_ACCOUNTID;
                string authToken = _sgiSettings.SERVICE_SMS_TOKEN;
                string numberFrom = _sgiSettings.SERVICE_SMS_NUMBER_FROM;

                TwilioClient.Init(accountSid, authToken);

                var message = await MessageResource.CreateAsync(
                    body: _body,
                    from: new Twilio.Types.PhoneNumber($"whatsapp:{numberFrom}"),
                    to: new Twilio.Types.PhoneNumber($"whatsapp:{_numberTo}")
                );
            }
            catch (System.Exception ex)
            {
                _log.Log($"Erro no envio de WhatsApp enviado para o número {_numberTo}, de alerta de bloqueio do dispositivo {_deviceId}. {ex.Message}");
            }
        }



    }
}