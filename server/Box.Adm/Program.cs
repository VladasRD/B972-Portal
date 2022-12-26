using System.IO;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;

namespace Box.Adm
{
    public class Program
    {
        public static void Main(string[] args)
        {
            BuildWebHost(args).Run();
        }

        public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
            .UseKestrel()
            //.UseWebListener(options => options.ListenerSettings.Authentication.Schemes = AuthenticationSchemes.NTLM)
            //.UseUrls("http://localhost:5000")
            .UseContentRoot(Directory.GetCurrentDirectory())            
            .UseIISIntegration()
            .UseStartup<Startup>()
            .Build();
    }
}
