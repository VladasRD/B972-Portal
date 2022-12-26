using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Box.Security.Data;
using Box.Security.Models;
using IdentityServer4.AccessTokenValidation;
using IdentityServer4;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using SmartGeoIot.HostedService;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace Box.Adm
{
    public class Startup
    {
        private readonly string RSA_KEY = "<RSAKeyValue><Modulus>5bwqAZ2pw3lFfNOJOu2cQrYj7Fe1Bo2FWm0N1V8lEAbZnixP1+v74qpiodWj1YWtVd27SBNxHOX6Tv3ROjM/e97vp5NZ9/XinfVkhPeLEEQGIEZagidRCLtey/vdUp9+7oXIa747f7wIRjsIByP78i03/e4PZGeb1TOfuVqvrYhHn4z47No03m6bY8oVGLojTUJIPly9yGgU3qoT7q8b8Z5sk3qO2jJJapynBPNTTwm3Xv+E2Kn7TnaG0Ye1HPJ0u83E397QnfJlFJFBb+x6+r5HTRD7LoB2lnKNKYOJZ78UR7sYQmnCufnQ8Q5Sl4AXCPtOfv9uJ1eEAG7QyKuILQ==</Modulus><Exponent>AQAB</Exponent><P>6QDlL7QXoOWkmPKyVsr5r7g6YuV1l4mxB2COuz4trWeofx8Yisna2MdNYnt0AJFFZih0/7G/sLWKaMaQ9T/KB/oEIZ8AXz79kkaq467WncMGaEGzPbaiUN6Dm4CXwl0v8v7Bh6GEsJowbgAY2H+pVmkyPUkBalve0D/q7XrrPOc=</P><Q>/Giv1RRsvWyH9WOCMRT2RU/R5JGOpV+gO5wIs5LbREEGkSIRjXimk9L3r1fb6D99HZ9+z6X9Ke2cFkEaDzEH1YgW/o0S6epa5tV9tosfqMB/Jq32eqyAggcn9dcofLuhBLSzrd3VuuQokJH2yJk0C6MCUZ2J1JZK9LpY/Tn/O8s=</Q><DP>IczacmAaqWGGzhUu375UGDSOa7hDpbb4skxEiE8Ny0DlRYOaSM/damMHC8lC2643NgmaZ1k+qIC9UlOzxY/6W8vd+46YPDjkCesscRj00y/uZwNY4BP1WevdhZpS6YQhJ4vjQSyiFghYDC0Bba6fPwTFn/ROO0KWgPk0uDyDkXM=</DP><DQ>ybxyLH/ymUNRNxOdTtVOL/+n5mNf+1T+oCYJ3lyV22uNcBompUe0+5k2VXKHVIzJ1w9PL2+fdA9xHWhTB6lMdaIbnr/qSgMBPFWN8IQpfZq9BZEs2sdcvpAxpA+fUHI2M6ipo2EWHhWVRlcjQxkPu+1BQTIEo2Cr8AWwclQUZxk=</DQ><InverseQ>hBgXOh5VzIrdyl4cft6W6zF49YxmMTLHk9X+APLeXsVF+hdX3ElGxASRK7EmbdpV3Vj++DioLM8E/yJyzd52p3+SM5hie5tamXtvgHbi6feKBZ7VLFpOSoIAiQw0B1BZfDv3B1+Xxta1RcnNfzHx6nwmZ/pht/hE2XOyPhufERM=</InverseQ><D>iLjaGwoUzVOwCiDui9Z7Z6x0ZqSwBI1W7sD2OaUdLpOVEbB3eB5mUrhqaLv3fAzJYs/KNJP1fmhT9ozR5xw2zOaZpBZqtcptKuqu/v8/kNea+bxFAy6Y6GRyf12Okyx+4z9VTuHxEQz7s6hQF8fPmopUx1fVa0DKGjFV/n69DL1sV4gQkUtQmzp4ADyfxkt1DBWekwTV9W7kS/IiuCl7AtsJC6f9XDuSvp9RGDCUIUEIr/z08Ryuk6XWbMldj6L5vQnXQXWAUOGF8Nl/ofq7sLRd7KCWvOcmkv9pQKZD5AOLmKaa1NGdbcxBRu22/pGgusshnX6P1cs641XGhb0hFQ==</D></RSAKeyValue>";

        private readonly IHostingEnvironment _hostingEnv;

        public IConfiguration Configuration { get; }

        private IdentityConfig identityConfig;

        public Startup(IConfiguration configuration, IHostingEnvironment hostingEnv)
        {
            Configuration = configuration;
            _hostingEnv = hostingEnv;

            var builder = new ConfigurationBuilder()
                    .SetBasePath(_hostingEnv.ContentRootPath)
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{_hostingEnv.EnvironmentName}.json", optional: true);

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            // Add localization
            services.AddLocalization(options => options.ResourcesPath = "Resources");

            // configure settings
            services.Configure<Box.Common.BoxSettings>(Configuration.GetSection("BoxSettings"));
            services.Configure<Box.Common.Services.SMTPSettings>(Configuration.GetSection("SMTPSettings"));
            services.Configure<Box.Security.Services.BoxLoggerSettings>(Configuration.GetSection("BoxLoggerSettings"));
            services.Configure<IdentityConfig>(Configuration.GetSection("IdentitySettings"));
            services.Configure<SmartGeoIot.SgiSettings>(Configuration.GetSection("SgiSettings"));

            // get config from appsettings
            identityConfig = Configuration.GetSection("IdentitySettings").Get<IdentityConfig>();

            // add box services
            services.AddTransient<Box.Common.Services.TemplateService>();
            services.AddTransient<Box.Common.Services.IEmailSender, Common.Services.SMTPEmailSender>();
            services.AddTransient<Box.Security.Services.SecurityService>();
            services.AddTransient<Box.Security.Services.LogService>();
            services.AddTransient<Box.CMS.Services.CMSService>();
            services.AddTransient<SmartGeoIot.Services.RadiodadosService>();

            // itens adicionados para execução dos hosts de serviços (jobs)
            services.AddHostedService<DeviceHostedService>();
            services.AddHostedService<MessageHostedService>();
            services.AddHostedService<BillingHostedService>();

            // 1 - WEB API
            //-------------------------------------------------------------------------------------------
            services.AddMvcCore().AddAuthorization().AddJsonFormatters();

            services.AddAuthentication(IdentityServerAuthenticationDefaults.AuthenticationScheme)
            .AddIdentityServerAuthentication(options =>
            {
                options.Authority = identityConfig.IDENTITY_URL;
                options.RequireHttpsMetadata = false;

                options.ApiName = identityConfig.API_NAME;

                // for caching and referece tokens
                options.ApiSecret = identityConfig.API_SECRET;
                options.EnableCaching = true;
                options.CacheDuration = TimeSpan.FromMinutes(10);
            });


            services.AddCors(options =>
            {
                // this defines a CORS policy called "default"
                options.AddPolicy("default", policy =>
                {
                    policy.WithOrigins(identityConfig.DEFAULT_CLIENT_URL)
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                });
            });

            // 2- Add database contexts
            //-------------------------------------------------------------------------------------------
            services.AddDbContext<Security.Data.SecurityDbContext>(SqlDbOptions);
            services.AddDbContext<CMS.Data.CMSContext>(SqlDbOptions);
            services.AddDbContext<SmartGeoIot.Data.SmartGeoIotContext>(SqlDbOptions);

            // for MySQL
            // services.AddDbContext<SecurityDbContext>(MySqlDbOptions);
            // services.AddDbContext<CMS.Data.CMSContext>(MySqlDbOptions);
            // services.AddDbContext<SmartGeoIot.Data.SmartGeoIotContext>(MySqlDbOptions);


            services.AddIdentity<ApplicationUser, ApplicationRole>()
                .AddEntityFrameworkStores<SecurityDbContext>()
                .AddDefaultTokenProviders();

            // 3 - IDENTITY SERVER CONNECT WITH ASPNET IDENTITY            
            //-------------------------------------------------------------------------------------------
            services.AddTransient<IdentityServer4.Stores.IPersistedGrantStore, Security.Stores.PersistedGrantStore>();
            services.AddIdentityServer()
                //.AddDeveloperSigningCredential()                
                //.AddSigningCredential(Common.StartupHelper.GetCertificateFromPath(_hostingEnv))
                //.AddSigningCredential(Common.StartupHelper.GetCertificateFromStore(config.TOKEN_CERT_BLUEPRINT))
                .AddSigningCredential(Common.StartupHelper.GetFixedRSAKey(RSA_KEY))
                .AddInMemoryIdentityResources(identityConfig.IdentityResources)
                .AddInMemoryApiResources(identityConfig.ApiResources)
                .AddInMemoryClients(identityConfig.Clients)
                .AddAspNetIdentity<ApplicationUser>();

            // 4 - ADD EXTERNAL LOGINS PROVIDERS    
            //-------------------------------------------------------------------------------------------
            if (!String.IsNullOrEmpty(identityConfig.OAUTH_GOOGLE_CLIENT_ID))
            {
                services.AddAuthentication().AddGoogle("Google", options =>
                    {
                        options.SignInScheme = IdentityServerConstants.ExternalCookieAuthenticationScheme;
                        options.ClientId = identityConfig.OAUTH_GOOGLE_CLIENT_ID;
                        options.ClientSecret = identityConfig.OAUTH_GOOGLE_CLIENT_SECRET;
                        options.UserInformationEndpoint = "https://www.googleapis.com/oauth2/v2/userinfo";
                        options.ClaimActions.Clear();
                        options.ClaimActions.MapJsonKey(ClaimTypes.NameIdentifier, "id");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Name, "name");
                        options.ClaimActions.MapJsonKey(ClaimTypes.GivenName, "given_name");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Surname, "family_name");
                        options.ClaimActions.MapJsonKey("urn:google:profile", "link");
                        options.ClaimActions.MapJsonKey(ClaimTypes.Email, "email");
                    });
            }
            if (!String.IsNullOrEmpty(identityConfig.OAUTH_MS_CLIENT_ID))
            {
                services.AddAuthentication().AddMicrosoftAccount(options =>
                    {
                        options.ClientId = identityConfig.OAUTH_MS_CLIENT_ID;
                        options.ClientSecret = identityConfig.OAUTH_MS_CLIENT_SECRET;
                    });
            }

            // MVC
            services
                .AddMvc(
                    config =>
                    {
                        config.Filters.Add(typeof(Box.Adm.Filters.ApiErrorHandlingFilter));
                    })
                .AddViewLocalization()
                .AddDataAnnotationsLocalization(options =>
                {
                    options.DataAnnotationLocalizerProvider = (type, factory) =>
                    factory.Create(typeof(Box.Common.Strings));
                });



            // ANtiForegy for Angular
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-TOKEN");

            // no Windows auth
            // services.Configure<IISOptions>(iis =>
            // {
            //     iis.AuthenticationDisplayName = "Windows";
            //     iis.AutomaticAuthentication = false;
            //     iis.ForwardClientCertificate = true;                
            // });
            services.Configure<Common.Services.SMTPSettings>(Configuration.GetSection("SMTPSettings"));

            // Caching
            services.AddResponseCaching();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Caching
            app.UseResponseCaching();
            
            // handle Angular routes
            app.Use(async (context, next) =>
            {

                await next();

                if (context.Response.StatusCode == 404 && !System.IO.Path.HasExtension(context.Request.Path.Value))
                {                                        
                    //var pathBase = context.Request.PathBase.ToUriComponent();
                    context.Request.Path = "/index.html";
                    //Console.Out.WriteLine("redirecionando path:" + context.Request.Path);
                    context.Response.StatusCode = 200; // Make sure we update the status code, otherwise it returns 404
                    await next();
                }
            });

            if (!env.IsProduction())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            var supportedCultures = new[] { new CultureInfo("en"), new CultureInfo("pt") };

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("en"),
                SupportedCultures = supportedCultures, // Formatting numbers, dates, etc.                
                SupportedUICultures = supportedCultures // UI strings that we have localized.
            });


            app.UseDefaultFiles();
            app.UseStaticFiles();

            // 1 - WEB API
            app.UseCors("default");
            app.UseAuthentication();

            app.UseMvc(routes =>
            {

                // change this route to override the user photo provider
                routes.MapRoute(
                    name: "UserPhoto",
                    template: "user-photo/{id}",
                    defaults: new { controller = "BoxUserInfo", action = "Photo" }
                    //defaults: new { controller = "SGIUserInfo", action = "Photo" }
                );

                routes.MapRoute(
                    name: "File",
                    template: "files/{folder}/{id}",
                    defaults: new { controller = "CMSFilesReadOnly", action = "Index" }
                );

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });

            app.UseIdentityServer();

            // ANTI-FORGEY - ONLY WILL WORK AT SAME SERVER
            //app.Use(next => context =>
            //{
            //    if (context.Request.Path == "/")
            //    {
            //            //send the request token as a JavaScript-readable cookie, and Angular will use it by default
            //            var tokens = antiforgery.GetAndStoreTokens(context);
            //        context.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken, new CookieOptions { HttpOnly = false });
            //    }
            //    return next(context);
            //});

        }

        private void SqlDbOptions(DbContextOptionsBuilder options)
        {
            options.UseSqlServer(
                Configuration.GetConnectionString("DefaultConnection"),
                b => b.UseRowNumberForPaging())
                .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

        private void MySqlDbOptions(DbContextOptionsBuilder options)
        {
            options.UseMySql(Configuration.GetConnectionString("DefaultConnection"))
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
        }

    }
}
