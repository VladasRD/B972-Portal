using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace SmartGeoIot.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<SmartGeoIotContext>
    {
        public SmartGeoIotContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "\\..\\Box.Adm\\")
                .AddJsonFile("appsettings.Stage.json")
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .Build();
    
            var builder = new DbContextOptionsBuilder<SmartGeoIotContext>();
    
            var connectionString = configuration.GetConnectionString("DefaultConnection");
    
            builder.UseSqlServer(connectionString);
            // builder.UseMySql(connectionString);
    
            return new SmartGeoIotContext(builder.Options);
        }
    }
}