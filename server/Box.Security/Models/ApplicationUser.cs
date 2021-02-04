using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace Box.Security.Models
{
    // Add profile data for application users by adding properties to the ApplicationUser class
    public class ApplicationUser : IdentityUser
    {
        public void RemoveNoIdentityClaims()
        {
            if (UserClaims == null)
                return;

            for (var i = UserClaims.Count - 1; i >= 0; i--)
            {
                var claim = UserClaims.ElementAt(i);
                if (claim.ClaimType != "given_name")
                {
                    UserClaims.Remove(claim);
                }
            }

        }

        public ICollection<IdentityUserClaim<string>> UserClaims { get; set; }

        public ICollection<IdentityUserRole<string>> UserRoles { get; set; }

        // ignore sensitive fields at serialization

        [JsonIgnore]
        public override string NormalizedEmail { get; set; }

        [JsonIgnore]
        public override string NormalizedUserName { get; set; }

        [JsonIgnore]
        public override string PasswordHash { get; set; }

        [JsonIgnore]
        public override string SecurityStamp { get; set; }

        // used to edit the password during API requests only
        [NotMapped]
        public string CleanPassword { get; set; }

        // used to edit loginNT during API requests only
        [NotMapped]
        public string LoginNT { get; set; }

        [NotMapped]
        public string GivenName
        {
            get
            {

                if (UserClaims == null)
                    return String.Empty;

                var nameClaim = UserClaims.SingleOrDefault(c => c.ClaimType == "given_name");
                if (nameClaim == null)
                    return String.Empty;

                return nameClaim.ClaimValue;
            }
        }

        [NotMapped]
        public bool IsLocked
        {
            get
            {
                return this.LockoutEnd != null;
            }
        }

    }

}
