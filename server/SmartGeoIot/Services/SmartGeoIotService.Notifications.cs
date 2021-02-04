using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;

namespace SmartGeoIot.Services
{
    public partial class SmartGeoIotService
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

        public void SendEmailLockedDevice(string[] emails, string deviceId, long time)
        {
            string subject = $"{_boxSettings.APPLICATION_NAME} - Bloqueio do dispositivo {deviceId}";

            // create the template
            dynamic data = _templateService.CreateBasicModel();
            data.subject = subject;
            data.deviceId = deviceId;
            data.date = Utils.TimeStampSecondsToDateTime(time).ToLocalTime().ToString("dd/MM/yyyy hh:mm:ss");
            var body = _templateService.RenderTemplate("AlertDeviceLocked", data, lang: "pt");

            SendEmail(emails, subject, body, "bloqueio do dispositivo", deviceId);
        }

        public void SendEmailReceiveLocationDevice(string[] emails, string deviceId, long time, string deviceType, string fixedLat, string fixedLng)
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

            SendEmail(emails, subject, body, "localização do dispositivo", deviceId);
        }

        public Task SendEmail(string[] emails, string subject, string body, string typeAlert, string deviceId)
        {
            return Task.Run(async () =>
            {
                try
                {
                    foreach (var email in emails)
                    {
                        if (!string.IsNullOrWhiteSpace(email))
                        {
                            await _emailSender.SendEmailAsync(
                                   email,
                                   subject,
                                   body);
                        }

                        _log.Log($"E-mail enviado para {email}, de {typeAlert} {deviceId}.");
                    }
                }
                catch (Exception ex)
                {
                    _log.Log($"Erro no envio de {typeAlert} {deviceId}.", ex.Message);
                }
            });
        }

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

        public Task SendWhatsAppLockedDevice(string[] numbers, string deviceId, long time)
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
                        parameters.Add("From", $"whatsapp:{_sgiSettings.SERVICE_WHATSAPP_NUMBER_FROM}");

                        if (!string.IsNullOrWhiteSpace(numberTo))
                        {
                            try
                            {
                                parameters.Add("To", $"whatsapp:{numberTo}");
                                var req = new HttpRequestMessage(HttpMethod.Post, string.Format(_sgiSettings.SERVICE_SMS_WHATSAPP_URL, accountSid))
                                {
                                    Content = new FormUrlEncodedContent(parameters)
                                };
                                HttpResponseMessage msg = await wc.SendAsync(req);

                                string jsonResult = string.Empty;
                                if (msg.StatusCode == System.Net.HttpStatusCode.Created)
                                    _log.Log($"WhatsApp enviado para o número {numberTo}, de alerta de bloqueio do dispositivo {deviceId}. {msg.RequestMessage}");
                                else
                                    _log.Log($"Erro no envio de WhatsApp enviado para o número {numberTo}, de alerta de bloqueio do dispositivo {deviceId}. {msg.RequestMessage}");
                            }
                            catch (System.Exception ex)
                            {
                                _log.Log($"Erro no envio de WhatsApp enviado para o número {numberTo}, de alerta de bloqueio do dispositivo {deviceId}. {ex.Message}");
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    _log.Log($"Erro no envio de WhatsApp de alerta de bloqueio do dispositivo {deviceId}.", ex.Message);
                }
            });
        }



    }
}