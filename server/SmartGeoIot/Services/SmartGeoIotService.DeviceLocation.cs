using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using SmartGeoIot.Extensions;
using SmartGeoIot.Models;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        public DeviceLocation GetLastDeviceLocationByDeviceId(string deviceId)
        {
            return _context.DevicesLocations.Where(c => c.DeviceId == deviceId).OrderByDescending(o => o.Time).Take(1).SingleOrDefault();
        }

        public DeviceLocation GetDeviceLocationByTime(string deviceId, long time)
        {
            DateTime datetimeForMessage = Utils.Timestamp_ToDateTimeBrasilian(time);
            return _context.DevicesLocations.SingleOrDefault(c => c.DeviceId == deviceId && c.CreateDate == datetimeForMessage);
        }

        public void SaveDeviceLocation(DeviceLocation deviceLocation)
        {
            _context.Entry<DeviceLocation>(deviceLocation).State = EntityState.Added;
            _context.SaveChanges(true);
            // _log.Log($"Registro de localização do dispositivo {deviceLocation.DeviceId} foi criado/alterado.");
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
                // _log.Log($"Detectado estado de bloqueio no disposito {deviceId}. Inínio de envio de notificações.");

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
                            SendEmailLockedDevice(emails, deviceId, time).Wait();
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
                            // SendSMSLockedDevice(numbers, deviceId, time);
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
                            // SendWhatsAppLockedDevice(numbers, deviceId, time);
                        }
                        catch (System.Exception ex)
                        {
                            _log.Log($"Erro no método de envio de WhatsApp de bloqueio[SendWhatsAppLockedDevice]. Erro message: {ex.Message}");
                        }
                    }
                }
            }
        }

        public string GetCityByCoordinates(double latitude, double longitude)
        {
            try
            {    
                string _latitude = latitude.ToString().Replace(",", ".");
                string _longitude = longitude.ToString().Replace(",", ".");
                
                string _url = $"{_sgiSettings.GOOGLE_MAPS_URL}?key={_sgiSettings.GOOGLE_MAPS_KEY}&sensor=false&latlng={_latitude},{_longitude}";
                HttpClient _httpClient = new HttpClient();
                HttpResponseMessage response = _httpClient.GetAsync(_url).Result;

                if (!response.IsSuccessStatusCode)
                    return string.Empty;
                
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                Models.GoogleMapsViewModel data = JsonConvert.DeserializeObject<Models.GoogleMapsViewModel>(jsonResponse);

                foreach (var result in data.results.Take(1))
                {
                    string city = string.Empty;

                    // recupera o bairro
                    foreach (var component in result.address_components.Where(c => c.types.Contains("sublocality_level_1")))
                    {
                        city += component.long_name;
                    }

                    // recupera a cidade
                    foreach (var component in result.address_components.Where(c => c.types.Contains("administrative_area_level_2")))
                    {
                        city += " - " + component.long_name;
                    }

                    // recupera o estado
                    foreach (var component in result.address_components.Where(c => c.types.Contains("administrative_area_level_1")))
                    {
                        city += "/" + component.short_name;
                    }

                    return city;
                }

                return string.Empty;
            }
            catch (System.Exception)
            {
                return string.Empty;
            }
        }

        public LocationMaps GetLocationByCoordinates(double latitude, double longitude)
        {
            LocationMaps locationMaps = new LocationMaps();
            try
            {
                string _latitude = latitude.ToString().Replace(",", ".");
                string _longitude = longitude.ToString().Replace(",", ".");
                
                string _url = $"{_sgiSettings.GOOGLE_MAPS_URL}?key={_sgiSettings.GOOGLE_MAPS_KEY}&sensor=false&latlng={_latitude},{_longitude}";
                HttpClient _httpClient = new HttpClient();
                HttpResponseMessage response = _httpClient.GetAsync(_url).Result;

                if (!response.IsSuccessStatusCode)
                    return null;
                
                string jsonResponse = response.Content.ReadAsStringAsync().Result;
                Models.GoogleMapsViewModel data = JsonConvert.DeserializeObject<Models.GoogleMapsViewModel>(jsonResponse);

                foreach (var result in data.results.Take(1))
                {
                    // recupera o bairro
                    foreach (var component in result.address_components.Where(c => c.types.Contains("sublocality_level_1")))
                    {
                        locationMaps.Neighborhood = component.long_name;
                    }

                    // recupera a cidade
                    foreach (var component in result.address_components.Where(c => c.types.Contains("administrative_area_level_2")))
                    {
                        locationMaps.City = component.long_name;
                    }

                    // recupera o estado
                    foreach (var component in result.address_components.Where(c => c.types.Contains("administrative_area_level_1")))
                    {
                        locationMaps.State = component.short_name;
                    }
                }

                return locationMaps;
            }
            catch (System.Exception)
            {
                return null;
            }
        }


    }
}