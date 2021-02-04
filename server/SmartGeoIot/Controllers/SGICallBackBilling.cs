using System;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using Box.Security.Services;
using Box.Common.Services;
using Box.Security.Data;
using SmartGeoIot.Data;
using SmartGeoIot.Services;

namespace SmartGeoIot.Controllers
{
    [Route("/[controller]")]
    public class SGICallBackBilling : Controller
    {
        private readonly SecurityService _securityService;
        private readonly IConfiguration _configuration;
        private readonly SmartGeoIotContext _context;
        private LogService _log { get; set; }
        private readonly IEmailSender _emailSender;
        SmartGeoIotService _sgiService;

        public SGICallBackBilling(
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
            _securityService = securityService;
            _emailSender = emailSender;
            _sgiService = sgiService;
        }

        public void UpdateBillingStatus(string id)
        {
            var token = Request.QueryString;
            _log.Log($"Atualizando faturamento via call-back, token recebido {token}. Request: {Request.Path.ToString()}.", null, true);
        }

        [HttpPost]
        public void Post(string token)
        {
            _log.Log($"Atualizando faturamento via call-back método post, token recebido {token}. Request: {Request.Path.ToString()}");
        }

    }
}