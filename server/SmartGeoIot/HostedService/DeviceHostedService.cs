using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using SmartGeoIot.Models;

namespace SmartGeoIot.HostedService
{
    public class DeviceHostedService : IHostedService
    {
        private Timer _timer;
        private int timerMiliseconds = 43200000;
        private string _urlSigfoxGetData = "https://radiodadosanalitica.azurewebsites.net/sgisigfox";
        // private string _urlSigfoxGetData = "http://rafaelestevao-001-site2.htempurl.com/sgisigfox";
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _timer = new Timer(DownloadDevice, null, 0, timerMiliseconds);
            return Task.CompletedTask;
        }

        void DownloadDevice(object state)
        {
            HttpClient wc = new HttpClient();
            try
            {
                wc.GetAsync($"{_urlSigfoxGetData}/DownloadDevicesSigfox");
            }
            catch (Exception ex)
            {
                string error = ex.Message;
                string actionMessage = "DeviceHostedService: Erro ao iniciar o método DownloadDevice.";
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