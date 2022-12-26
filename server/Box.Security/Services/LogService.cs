using Box.Security.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Box.Common.Web;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Box.Security.Services
{
    public class LogService
    {

        private Data.SecurityDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<Models.ApplicationUser> _userManager;
        private readonly BoxLoggerSettings _settings;
        private readonly IOptions<BoxLoggerSettings> _options;

        public LogService(
            UserManager<Models.ApplicationUser> userManager,
            Data.SecurityDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IOptions<BoxLoggerSettings> loggingSettings)
        {
            _userManager = userManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _settings = loggingSettings.Value;
            _options = loggingSettings;
        }

        public LogService CreateThreadSafeInstance(Data.SecurityDbContext context) {
            return new LogService(_userManager, context, _httpContextAccessor, _options);
        }

        /// <summary>
        /// Logs an action at the database log.
        /// </summary>
        /// <param name="actionDescription">The action</param>
        /// <param name="errorDescription">The error description in case of error</param>
        /// <param name="saveParameters">Tru to save th request parameters at the database</param>
        public void Log(string actionDescription, string errorDescription = null, bool saveParameters = true)
        {
            LogAsync(actionDescription, errorDescription, saveParameters).Wait();
        }

        /// <summary>
        /// Logs an action at the database log.
        /// </summary>
        /// <param name="actionDescription">The action</param>
        /// <param name="errorDescription">The error description in case of error</param>
        /// <param name="saveParameters">Tru to save th request parameters at the database</param>
        public async Task LogErrorAsync(string actionDescription, string errorDescription = null, bool saveParameters = true)
        {
            await LogAsync(actionDescription, errorDescription, saveParameters);
        }

        /// <summary>
        /// Logs an action at the database log async.
        /// </summary>
        /// <param name="actionDescription">The action</param>
        /// <param name="errorDescription">The error description in case of error</param>
        /// <param name="saveParameters">Tru to save th request parameters at the database</param>
        private async Task LogAsync(string actionDescription, string errorDescription = null, bool saveParameters = true)
        {

            if(!_settings.IsEnabled)
                return;

            string url = "unknow";
            string login = "unknow";
            string ip = "unknow";
            DateTime when = DateTime.Now.ToUniversalTime();
            string parameters = String.Empty;

            var httpContext = _httpContextAccessor.HttpContext;
            if (httpContext != null)
            {

                url = httpContext.Request.Path;
                ip = httpContext.Connection.RemoteIpAddress.ToString();

                var user = await _userManager.GetUserAsync(httpContext.User);

                if (user != null)
                    login = user.UserName;
                
                if (saveParameters)
                {
                    //Salvar dados que são enviados no corpo da requisição
                    try
                    {
                        if (httpContext.Request.Body.CanRead && httpContext.Request.Body.CanSeek)
                        {
                            httpContext.Request.Body.Seek(0, System.IO.SeekOrigin.Begin);
                            System.IO.StreamReader sr = new System.IO.StreamReader(httpContext.Request.Body);
                            var json = sr.ReadToEnd();

                            // remove sensitive data
                            JToken token = JObject.Parse(json);
                            var passToken = token.SelectToken("cleanPassword") as JValue;

                            if (passToken!=null && !string.IsNullOrEmpty(passToken.Value<string>()))
                            {                                
                                passToken.Value = "XXXXXXX";
                                
                            }
                            parameters = JsonConvert.SerializeObject(token);

                        }
                    } catch(Exception ex)
                    {
                        parameters = "ERROR READING PARAMETERS: " + ex.Message;
                    }
                }
                
            }
            
            short logType = 0;
            if (!string.IsNullOrEmpty(errorDescription))
            {
                logType = 1;
            }

            Log log = new Log() { LogUId = Guid.NewGuid(), ActionDescription = actionDescription, ErrorDescription = errorDescription, LogType = logType, SignedUser = login, Url = url, UserIp = ip, When = when, Parameters = parameters };

            try
            {                
                _context.Logs.Add(log);
                _context.SaveChanges();

                // delete old records
                // int years = _settings.KeepLogsForYears > 0 ? _settings.KeepLogsForYears : 1;
                // DateTime yearAgo = when.AddYears(-years);
                // _context.Database.ExecuteSqlCommand("DELETE FROM Logs WHERE Logs.[When] < {0}",  yearAgo);

                // DateTime hoursAgo = when.AddHours(-12);
                // _context.Database.ExecuteSqlCommand("DELETE FROM Logs WHERE Logs.[When] < {0}",  hoursAgo);
                
            }
            catch (Exception) { }

        }

        public IEnumerable<Log> GetLogs(string filter = null, int skip = 0, int top = 0, DateTime? fromDate = null, DateTime? toDate = null, OptionalOutTotalCount totalCount = null)
        {
            
            IQueryable<Log> logs = _context.Logs;

            if (!String.IsNullOrEmpty(filter))
            {

                string[] tags = filter.ToLower().Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                logs = logs.Where(l => tags.All(t =>
                    l.SignedUser.ToLower() == t ||
                    l.Url.ToLower().Contains(t) ||
                    l.ActionDescription.ToLower().Contains(t) ||
                    l.UserIp.ToLower() == t));
            }

            if (fromDate != null)
            {
                fromDate = fromDate.Value.Date;
                logs = logs.Where(l => l.When >= fromDate.Value);
            }

            if (toDate != null)
            {
                DateTime dataAteDiaSeguinte = toDate.Value.AddDays(1).Date;
                logs = logs.Where(l => l.When < dataAteDiaSeguinte);
            }

            logs = logs.OrderByDescending(l => l.When);

            if (totalCount != null)
            {
                totalCount.Value = logs.Count();
            }


            if (skip != 0)
                logs = logs.Skip(skip);

            if (top != 0)
                logs = logs.Take(top);

            // dont return error description here
            return logs.ToList().Select(l => new Log
            {
                ActionDescription = l.ActionDescription,
                ErrorDescription = l.ErrorDescription,
                LogType = l.LogType,
                LogUId = l.LogUId,
                SignedUser = l.SignedUser,
                Url = l.Url,
                UserIp = l.UserIp,
                When = l.When
            }).ToArray();


        }
        
        public Log GetLog(Guid logUId) {
            return _context.Logs.SingleOrDefault(l => l.LogUId ==logUId);
        }        
    }

    public class BoxLoggerSettings {
        public int KeepLogsForYears {get;set;}  = 1;      
        public bool IsEnabled {get;set;} = true;
    }
}
