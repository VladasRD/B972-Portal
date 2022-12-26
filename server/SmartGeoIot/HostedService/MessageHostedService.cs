using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;
using System;
using System.Net.Http;

namespace SmartGeoIot.HostedService
{
    public class MessageHostedService : IHostedService
    {
        private Timer _timer;
        private int _timerMiliseconds = 300000;
        private string _urlSigfoxGetData = "https://rdportal.com.br/sgisigfox";
        
        // private string _urlSigfoxGetData = "https://radiodadosanalitica.azurewebsites.net/sgisigfox";

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DownloadMessage, null, 0, _timerMiliseconds);
            return Task.CompletedTask;
        }

        void DownloadMessage(object state)
        {
            HttpClient wc = new HttpClient();
            try
            {
                wc.GetAsync($"{_urlSigfoxGetData}/DownloadMessagesSigfox");
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                string actionMessage = "MessageHostedService: Erro ao iniciar o método DownloadMessage.";
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