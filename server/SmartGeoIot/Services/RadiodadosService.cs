using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Box.Security.Services;
using SmartGeoIot.Models;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Localization;
using System.Security.Claims;
using SmartGeoIot.ViewModels;
using System.Globalization;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SmartGeoIot.Services
{
    public partial class RadiodadosService
    {
        private readonly Data.SmartGeoIotContext _context;
        private readonly Box.Security.Data.SecurityDbContext _securityContext;
        public string WebPath { get; private set; }
        public string AppPath { get; private set; }
        private LogService _log { get; set; }
        private readonly Box.Security.Services.SecurityService _securityService;
        private readonly Box.Common.Services.IEmailSender _emailSender;
        private readonly Box.Common.Services.TemplateService _templateService;
        private readonly Box.Common.BoxSettings _boxSettings;
        private readonly SmartGeoIot.SgiSettings _sgiSettings;
        private readonly IStringLocalizer<Box.Common.Strings> _strings;
        private string[] _sigfoxLogins;
        private string[] _sigfoxPasswords;
        private readonly Box.Common.Services.SMTPSettings _SMTPsettings;

        public RadiodadosService(
            Data.SmartGeoIotContext context,
            Box.Security.Data.SecurityDbContext securityContext,
            LogService log,
            IHostingEnvironment env,
            Box.Security.Services.SecurityService securityService,
            Box.Common.Services.IEmailSender emailSender,
            Box.Common.Services.TemplateService templateService,
            IOptions<Box.Common.BoxSettings> boxSettings,
            IOptions<SmartGeoIot.SgiSettings> sgiSettings,
            IOptions<Box.Common.Services.SMTPSettings> SMTPsettings,
            IStringLocalizer<Box.Common.Strings> strings)
        {
            _context = context;
            _securityContext = securityContext;
            _log = log;
            WebPath = env.WebRootPath;
            AppPath = env.ContentRootPath;
            _securityService = securityService;
            _emailSender = emailSender;
            _templateService = templateService;
            _boxSettings = boxSettings.Value;
            _sgiSettings = sgiSettings.Value;
            this._strings = strings;
            _SMTPsettings = SMTPsettings.Value;

            _sigfoxLogins = _sgiSettings.SIG_FOX_LOGIN.Split(";");
            _sigfoxPasswords = _sgiSettings.SIG_FOX_PASSWORD.Split(";");
        }
        
        #region UTILS
        private Bits CreateBits(string strbits)
        {
            return new Bits()
            {
                Iluminacao = Convert.ToBoolean(int.Parse(strbits.Substring(0, 1))),
                BombaCirculacao = Convert.ToBoolean(int.Parse(strbits.Substring(1, 1))),
                FalhaEnergia = Convert.ToBoolean(int.Parse(strbits.Substring(2, 1))),
                Automatico = Convert.ToBoolean(int.Parse(strbits.Substring(3, 1))),
                SensorNivelOperacional = Convert.ToBoolean(int.Parse(strbits.Substring(4, 1))),
                AlertaNivelMinimo = Convert.ToBoolean(int.Parse(strbits.Substring(5, 1))),
                AlertaNivelMaximo = Convert.ToBoolean(int.Parse(strbits.Substring(6, 1))),
                BombaOxigenacao = Convert.ToBoolean(int.Parse(strbits.Substring(7, 1)))
            };
        }

        // Return date on format "yyyy-MM-dd"
        public string FormatDate(DateTime date)
        {
            string _birthMonth = date.Month < 10 ? $"0{date.Month.ToString()}" : date.Month.ToString();
            string _birthDay = date.Day < 10 ? $"0{date.Day.ToString()}" : date.Day.ToString();
            return $"{date.Year}-{_birthMonth}-{_birthDay}";
        }

        public bool IsLastDayOfMonth()
        {
            var today = DateTime.UtcNow.AddHours(-3);
            var lastDayOfMonth = DateTime.DaysInMonth(today.Year, today.Month);
            if (lastDayOfMonth == today.Day)
                return true;
            
            return false;
        }
        #endregion
        
    }
}