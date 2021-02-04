using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Box.Security.Models
{
    public class ApplicationRole : IdentityRole
    {
        public override string Name { get; set; }

        public string Description { get; set; }

        public bool IsSystemRole { get; set; }

        public ICollection<IdentityRoleClaim<string>> RoleClaims { get; set; }

        [JsonIgnore]
        public override string NormalizedName { get; set; }

        
    }
}
