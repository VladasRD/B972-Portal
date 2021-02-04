using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Box.Security.Services;
using Box.Common.Services;
using SmartGeoIot.Data;
using SmartGeoIot.Services;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace SmartGeoIot.Controllers
{
    public class BillingController : Controller
    {
        private readonly SecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly SmartGeoIotContext _context;
        private LogService _log { get; set; }
        private readonly IEmailSender _emailSender;
        SmartGeoIotService _sgiService;

        public BillingController(
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
            _emailSender = emailSender;
            _sgiService = sgiService;
        }

        public void SendBilling()
        {
            _log.Log("Início da rotina de faturamento.");
            var clients = _context.Clients.Where(w => w.Active).ToArray();

            foreach (var client in clients)
            {
                if (client.StartBilling == null)
                    continue;

                int startMonth = client.StartBilling.Value.Month;

                for (int i = startMonth; i <= DateTime.Now.Month; i++)
                {
                    if (!_sgiService.HasBillingCurrentMonth(client.ClientUId, DateTime.Now.Month))
                    {
                        if (DateTime.Now.Day < client.DueDay)
                        {
                            var clientBilling = _sgiService.CreateBilling(client);
                        }

                        // Envia e-mail para o cliente com boleto
                        // var result = await _sgiService.SendClientBilling(client, clientBilling);
                        // if (result)
                        //     _sgiService.UpdateBillingSended(client, clientBilling);
                    }
                }
            }
            _log.Log("Término da rotina de faturamento.");
        }

        public void VerifyResendBilling()
        {
            // Verificar os faturamentos que não foram enviados e reenviar
        }

        public void VerifyBillingPayment()
        {
            _log.Log("Início da rotina de verificação de faturamentos pagos.");
            var clients = _context.Clients.Include(i => i.Billings).ToArray();

            // verificar status
            // https://dev.gerencianet.com.br/docs/transacoes
            foreach (var client in clients)
            {
                foreach (var billing in client.Billings)
                {
                    ViewModels.DetailCharge.DetailChargeResponde detailCharge = _sgiService.DetailCharge(billing.ExternalId);
                    if (detailCharge.data.status.ToLower().Equals("paid"))
                    {
                        if (!billing.Status.ToLower().Equals(detailCharge.data.status.ToLower()))
                        {
                            billing.Status = detailCharge.data.status;
                            billing.PaymentDate = DateTime.UtcNow;
                            _context.Entry<Models.ClientBilling>(billing).State = EntityState.Modified;
                            _context.SaveChanges();
                            _log.Log($"Faturamento do cliente {client.Name}, com vencimento em {billing.PaymentDueDate.Value.ToShortDateString()} foi atualizado.");
                        }
                    }
                }
            }

            _log.Log("Término da rotina de verificação de faturamentos pagos.");
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