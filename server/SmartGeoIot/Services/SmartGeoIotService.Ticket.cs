using System;
using SmartGeoIot.Models;
using Newtonsoft.Json;
using static SmartGeoIot.ViewModels.GerenciaNetViewModels;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        public dynamic CreateBillet(Client client, ClientBilling clientBilling)
        {
            // _log.Log("Executando método CreateBillet para gerar boleto.");
            dynamic endpoints = new GerenciaNetEndpoints(_sgiSettings.GERENCIA_NET_CLIENT_ID, _sgiSettings.GERENCIA_NET_CLIENT_SECRET, true);
            var _value = $"{((int)client.Value).ToString()}00";

            dynamic response = null;
            if (client.DocumentType != (int)DocumentType.CPF)
            {
                var body = new
                {
                    items = new[]
                    {
                        new {
                            name = client.Item,
                            value = Convert.ToInt32(_value),
                            amount = 1
                        }
                    },
                    payment = new
                    {
                        banking_billet = new
                        {
                            customer = new
                            {
                                name = client.Name,
                                email = client.Email,
                                cpf = client.Cpf,
                                birth = FormatDate(client.Birth.Value),
                                phone_number = client.Phone,
                                juridical_person = new
                                {
                                    corporate_name = client.Name,
                                    cnpj = client.Document
                                }
                            },
                            expire_at = FormatDate(clientBilling.PaymentDueDate.Value)
                        }
                    },
                    metadata = new
                    {
                        custom_id = clientBilling.ClientBillingUId,
                        notification_url = _sgiSettings.GERENCIA_NET_URL_CALL_BACK
                    }
                };

                response = endpoints.OneStep(null, body);
            }
            else
            {
                var body = new
                {
                    items = new[]
                    {
                        new {
                            name = client.Item,
                            value = Convert.ToInt32(_value),
                            amount = 1
                        }
                    },
                    payment = new
                    {
                        banking_billet = new
                        {
                            customer = new
                            {
                                name = client.Name,
                                email = client.Email,
                                cpf = client.Cpf,
                                birth = FormatDate(client.Birth.Value),
                                phone_number = client.Phone
                            },
                            expire_at = FormatDate(clientBilling.PaymentDueDate.Value),
                        }
                    },
                    metadata = new
                    {
                        custom_id = clientBilling.ClientBillingUId,
                        notification_url = _sgiSettings.GERENCIA_NET_URL_CALL_BACK
                    }
                };

                response = endpoints.OneStep(null, body);
            }

            // _log.Log("Finalizando método CreateBillet para gerar boleto.");
            return JsonConvert.DeserializeObject<ChargeResponse>(response.ToString());
        }

        public dynamic DetailCharge(int id)
        {
            dynamic endpoints = new GerenciaNetEndpoints(_sgiSettings.GERENCIA_NET_CLIENT_ID, _sgiSettings.GERENCIA_NET_CLIENT_SECRET, true);
            var param = new {
                id = id
            };
            var response = endpoints.DetailCharge(param);
            return JsonConvert.DeserializeObject<ViewModels.DetailCharge.DetailChargeResponde>(response.ToString());
        }

        public dynamic GetNotification(string token)
        {
            dynamic endpoints = new GerenciaNetEndpoints(_sgiSettings.GERENCIA_NET_CLIENT_ID, _sgiSettings.GERENCIA_NET_CLIENT_SECRET, true);
            var param = new {
                token = token
            };
            var response = endpoints.GetNotification(param);
            return JsonConvert.DeserializeObject<ViewModels.DetailCharge.DetailChargeResponde>(response.ToString());
        }

    }
}