using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Box.Security.Models;
using Microsoft.AspNetCore.Identity;

namespace Box.Security.Data
{
    
    public class SecurityDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, string>
    {

        private readonly DbContextOptions<SecurityDbContext> _options;
        public SecurityDbContext(DbContextOptions<SecurityDbContext> options) : base(options)        
        {
            _options = options;
        }

        public DbContextOptions<SecurityDbContext> StartUpOptions {
            get {
                return _options;
            }
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Add your customizations after calling base.OnModelCreating(modelBuilder);
            builder.Entity<ApplicationUser>().ToTable("Users");
            builder.Entity<ApplicationRole>().ToTable("Roles");
            builder.Entity<IdentityUserClaim<string>>().ToTable("UserClaims");
            builder.Entity<IdentityUserRole<string>>().ToTable("UserRoles");
            builder.Entity<IdentityUserLogin<string>>().ToTable("UserLogins");
            builder.Entity<IdentityRoleClaim<string>>().ToTable("RoleClaims");
            builder.Entity<IdentityUserToken<string>>().ToTable("UserTokens");

            builder.Entity<IdentityServer4.Models.PersistedGrant>().HasKey(p => p.Key);

            // add UserClaims and RoleClaims navigation property using existing FK
            builder.Entity<ApplicationUser>()
                .HasMany<IdentityUserClaim<string>>(u => u.UserClaims)
                .WithOne()
                .HasForeignKey(c => c.UserId);

            builder.Entity<ApplicationUser>()
                .HasMany<IdentityUserRole<string>>(u => u.UserRoles)
                .WithOne()
                .HasForeignKey(c => c.UserId);

            // add RoleClaims navigation property using existing FK
            builder.Entity<ApplicationRole>()
                .HasMany<IdentityRoleClaim<string>>(u => u.RoleClaims)
                .WithOne()
                .HasForeignKey(c => c.RoleId);
        }

        public DbSet<Models.Log> Logs { get; set; }
        
        public DbSet<IdentityServer4.Models.PersistedGrant> PersistedGrants { get; set; }

    }
}
