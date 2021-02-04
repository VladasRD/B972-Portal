using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using RazorLight;

namespace Box.Common.Services
{

    // https://github.com/toddams/RazorLight
    public class TemplateService {

        private string appPath;
        
        private readonly Box.Common.BoxSettings _boxSettings;
        private readonly IConfiguration _configuration;

        public TemplateService(
            IHostingEnvironment env,
            IOptions<Box.Common.BoxSettings> boxSettings,
            IConfiguration configuration) {
            appPath = env.ContentRootPath;            
            _boxSettings = boxSettings.Value;
            _configuration = configuration;
        }

        /// <summary>
        /// Creates a basic model with the siteUrl and siteName properties.
        /// </summary>        
        /// <param name="request">Request used to create the siteUrl</param>
        /// <returns>The basic model</returns>
        public ExpandoObject CreateBasicModel(HttpRequest request) {
            dynamic data = new ExpandoObject();                         
            data.siteUrl = request.Scheme + "://" + request.Host;
            data.siteClientUrl = CLIENT_URL;
            data.siteName = _boxSettings.APPLICATION_NAME;
            return data;
        }

        /// <summary>
        /// Creates a basic model with the siteUrl, siteClientUrl and siteName properties.
        /// </summary>
        /// <returns>The basic model</returns>
        public ExpandoObject CreateBasicModel() {
            dynamic data = new ExpandoObject();                         
            data.siteUrl = ADM_URL;
            data.siteClientUrl = CLIENT_URL;
            data.siteName = _boxSettings.APPLICATION_NAME;
            return data;
        }
        
        public async Task<String> RenderTemplate(string templateName, dynamic model, ExpandoObject viewBag = null, string lang = null) 
        {
            var engine = CreateRazorLightEngine();
            
            if(String.IsNullOrEmpty(lang))
                lang = System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag;

            templateName = templateName + "." + lang + ".cshtml";

            try {
                return await engine.CompileRenderAsync(templateName, model);
            } catch(Exception ex) {
                throw new Exception("Error compiling email template " + templateName + ".\n" + ex.Message);
            }
        }

        private RazorLightEngine CreateRazorLightEngine() 
        {
            if (string.IsNullOrEmpty(appPath))
                appPath = GetApplicationRoot();

            var templatePath = System.IO.Path.Combine(appPath, "App_Data\\EmailTemplates");
            
            RazorLightEngine engine;
            try {
                engine = new RazorLightEngineBuilder()
                    .UseFilesystemProject(templatePath)
                    .UseMemoryCachingProvider()
                    .Build();
            } catch(Exception ex) {
                throw new Exception("Error reading email templates from " + templatePath + "\n" + ex.Message);
            }

            return engine;
        }

        private string ADM_URL {
            get {
                return _configuration.GetValue<string>("IdentitySettings:IDENTITY_URL");
            }
        }

        public string CLIENT_URL {
            get {
                return _configuration.GetValue<string>("IdentitySettings:DEFAULT_CLIENT_URL");
            }
        }

        public string GetApplicationRoot()
        {
            var exePath = Path.GetDirectoryName(System.Reflection
                              .Assembly.GetExecutingAssembly().CodeBase);
            Regex appPathMatcher = new Regex(@"(?<!fil)[A-Za-z]:\\+[\S\s]*?(?=\\+bin)");
            var appRoot = appPathMatcher.Match(exePath).Value;
            return appRoot;
        }

    }
}