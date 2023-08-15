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

            modelBuilder.Entity<Message>()
                .HasKey(aa => new { aa.Id, aa.DeviceId });
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
        public virtual DbSet<ReportResil> ReportResil { get; set; }
        public virtual DbSet<B972> B972s { get; set; }
        public virtual DbSet<ProjectDevice> VW_DevicesByProjectCode { get; set; }
        public virtual DbSet<ResetTotalPartial> ResetTotalPartials { get; set; }
        public virtual DbSet<B982_S> B982_S { get; set; }
        public virtual DbSet<MCond> MConds { get; set; }
        public virtual DbSet<B979> B979s { get; set; }
        public virtual DbSet<B979RequestToDevice> B979RequestToDevices { get; set; }
        public virtual DbSet<B975> B975s { get; set; }
        public virtual DbSet<ServiceDesk> ServiceDesks { get; set; }
        public virtual DbSet<ServiceDeskRecord> ServiceDeskRecords { get; set; }
    }
    
}