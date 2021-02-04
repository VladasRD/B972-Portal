using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Box.CMS.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<CMSContext>
    {
        public CMSContext CreateDbContext(string[] args)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory() + "\\..\\Box.Adm\\")
                .AddJsonFile("appsettings.json")
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .Build();
    
            var builder = new DbContextOptionsBuilder<CMSContext>();
    
            var connectionString = configuration.GetConnectionString("DefaultConnection");
    
            // MySQL
            //builder.UseMySql(connectionString);

            // SQL Server
            builder.UseSqlServer(connectionString);
    
            return new CMSContext(builder.Options);
        }
    }
}