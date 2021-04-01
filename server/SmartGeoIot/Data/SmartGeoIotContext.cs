using System.ComponentModel.DataAnnotations.Schema;
using Box.Security.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartGeoIot.Models;

namespace SmartGeoIot.Data
{
    public class SmartGeoIotContext : DbContext
    {
        public SmartGeoIotContext(DbContextOptions<SmartGeoIotContext> options) : base(options) { }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Descomentar sempre que rodar migration para não incluir as views como tabelas.
            modelBuilder.Ignore<ApplicationUser>();
            // modelBuilder.Ignore<VW_Outgoing>();

            modelBuilder.Entity<Client>()
                .Property(b => b.Created)
                .HasDefaultValueSql("getdate()");
        }
        public virtual DbSet<Device> Devices { get; set; }
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Client> Clients { get; set; }
        public virtual DbSet<ClientDevice> ClientsDevices { get; set; }
        public virtual DbSet<ClientUser> ClientsUsers { get; set; }
        public virtual DbSet<ClientBilling> ClientsBillings { get; set; }
        public virtual DbSet<DeviceRegistration> DevicesRegistration { get; set; }
        public virtual DbSet<Project> Projects { get; set; }
        public virtual DbSet<Package> Packages { get; set; }
        public virtual DbSet<DeviceLocation> DevicesLocations { get; set; }
        public virtual DbSet<Outgoing> Outgoings { get; set; }
        public virtual DbSet<VW_Outgoing> VW_Outgoings { get; set; }
    }
}