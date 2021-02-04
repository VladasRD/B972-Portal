using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace SmartGeoIot.HostedService
{
    public class BillingHostedService : IHostedService
    {
        private Timer _timer;
        private int _timerMiliseconds = 43200000;
        private string _urlSigfoxGetData = "https://radiodadosanalitica.azurewebsites.net";
        // private string _urlSigfoxGetData = "http://rafaelestevao-001-site2.htempurl.com/billing";
        
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(SendBilling, null, 0, _timerMiliseconds);
            _timer = new Timer(VerifyBillingPayment, null, 0, _timerMiliseconds);
            return Task.CompletedTask;
        }

        // Envia o faturamento para os clientes por e-mail
        void SendBilling(object state)
        {
            HttpClient wc = new HttpClient();
            try
            {
                wc.GetAsync($"{_urlSigfoxGetData}/SendBilling");
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                string actionMessage = "BillingHostedService: Erro ao iniciar o método SendBilling.";
                wc.GetAsync($"{_urlSigfoxGetData}/SaveLogs?action={actionMessage}&error={error}");
            }
        }

        // Verifica se o faturamento dos clientes já foram pagos
        void VerifyBillingPayment(object state)
        {
            HttpClient wc = new HttpClient();
            try
            {
                wc.GetAsync($"{_urlSigfoxGetData}/VerifyBillingPayment");
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                string actionMessage = "BillingHostedService: Erro ao iniciar o método VerifyBillingPayment.";
                wc.GetAsync($"{_urlSigfoxGetData}/SaveLogs?action={actionMessage}&error={error}");
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            //New Timer does not have a stop. 
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }
    }
}