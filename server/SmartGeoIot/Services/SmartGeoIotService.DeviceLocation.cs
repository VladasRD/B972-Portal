using System;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Models;

namespace SmartGeoIot.Services
{
    public partial class SmartGeoIotService
    {
        public DeviceLocation GetDeviceLocationByDeviceId(string deviceId)
        {
            return _context.DevicesLocations.Where(c => c.DeviceId == deviceId).OrderByDescending(o => o.Time).Take(1).SingleOrDefault();
        }

        public void SaveDeviceLocation(DeviceLocation deviceLocation)
        {
            _context.Entry<DeviceLocation>(deviceLocation).State = EntityState.Added;
            _context.SaveChanges(true);
            _log.Log($"Registro de localização do dispositivo {deviceLocation.DeviceId} foi criado/alterado.");
        }

        public string LocationDecimalToDegrees(decimal decimalValue, string type)
        {
            Tuple<int, int, int> tuple = Tuple.Create(Convert.ToInt32(decimal.Truncate(decimalValue)), Convert.ToInt32((decimal.Truncate(Math.Abs(decimalValue) * 60)) % 60), Convert.ToInt32((Math.Abs(decimalValue) * 3600) % 60));
            string[] converted = tuple.ToString().Replace("-", "").Replace("(", "").Replace(")", "").Split(",");
            if (converted.Length > 2)
                return $"{converted[0].Trim()}° {converted[1].Trim()}' {converted[2].Trim()}\" {type}";

            return string.Empty;
        }

        public string RadiusFormated(string value)
        {
            return $"{Decimal.Parse(String.Format("{0:N}", value)).ToString("n0")} m";
        }

        public void VerifyIsLockOnDeviceDJRF(string data, string deviceId, long time)
        {
            // Crio uma mensagem para verificar se o estado de bloqueio está true
            // Isso mostrar que teve um bloqueio no dispositivo, para projetos DJRF
            Models.Message message = new Models.Message { Data = data };
            if (message.Bits.EstadoBloqueio || message.Bits.EstadoSaidaRastreador)
            {
                _log.Log($"Detectado estado de bloqueio no disposito {deviceId}. Inínio de envio de notificações.");

                // Pegamos todos os clientes que tem aquele dispositivo e que estão ativos (cliente e dispositivo)
                var clients = GetClientsByDevice(deviceId);
                foreach (var client in clients)
                {
                    if (client.EmailNotification)
                    {
                        string[] emails = new string[]
                        {
                           client.Email,
                           _sgiSettings.ADM_EMAIL,
                           _sgiSettings.TECHNICAL_EMAIL
                        };

                        try
                        {
                            SendEmailLockedDevice(emails, deviceId, time);
                        }
                        catch (System.Exception ex)
                        {
                            _log.Log($"Erro no método de envio de email de bloqueio[SendEmailLockedDevice]. Erro message: {ex.Message}");
                        }
                    }

                    if (client.SMSNotification)
                    {
                        string[] numbers = new string[]
                        {
                           $"+55{client.Phone}",
                           _sgiSettings.ADM_PHONE,
                           _sgiSettings.TECHNICAL_PHONE
                        };

                        try
                        {
                            SendSMSLockedDevice(numbers, deviceId, time);
                        }
                        catch (System.Exception ex)
                        {
                            _log.Log($"Erro no método de envio de SMS de bloqueio[SendSMSLockedDevice]. Erro message: {ex.Message}");
                        }
                    }

                    if (client.WhatsAppNotification)
                    {
                        string[] numbers = new string[]
                        {
                           $"+55{client.Phone}",
                           _sgiSettings.ADM_PHONE,
                           _sgiSettings.TECHNICAL_PHONE
                        };

                        try
                        {
                            SendWhatsAppLockedDevice(numbers, deviceId, time);
                        }
                        catch (System.Exception ex)
                        {
                            _log.Log($"Erro no método de envio de WhatsApp de bloqueio[SendWhatsAppLockedDevice]. Erro message: {ex.Message}");
                        }
                    }
                }
            }
        }


    }
}