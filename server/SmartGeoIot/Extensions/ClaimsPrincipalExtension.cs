using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace SmartGeoIot.Extensions
{
    public static class ClaimsPrincipalExtension
    {
        /// <summary>
        /// Gets all user programs based on his claims.
        /// </summary>
        /// <param name="user">The user</param>
        /// <returns>The user programs</returns>
        public static SmartGeoIot.Models.Client[] GetClients(this ClaimsPrincipal user)
        {
            List<SmartGeoIot.Models.Client> userClients = new List<SmartGeoIot.Models.Client>();
            if(!user.IsInRole("SGI-CLIENT.READ") || !user.IsInRole("SGI-CLIENT.WRITE"))
                return null;

            // if(user.IsInRole(Seleto.Services.SeletoService.AtacadoAccessClaim))
            //     userPrograms.Add(Seleto.Models.Programs.SeletoAtacado);

            // if(user.IsInRole(Seleto.Services.SeletoService.BoostinAccessClaim))
            //     userPrograms.Add(Seleto.Models.Programs.SeletoBoostin);

            return new SmartGeoIot.Models.Client[]{}.ToArray();
        }

        /// <summary>
        /// Returns true if the user can access a given program based on his claims.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="client">The program</param>
        /// <returns>True if the user can access the program</returns>
        public static bool CanAccessClient(this ClaimsPrincipal user, string clientUId)
        {
            var clients = GetClients(user);
            return clients.Any(p => p.ClientUId == clientUId);
        }

        public static string GetId(this ClaimsPrincipal user)
        {
            var subClaim = user.Claims.SingleOrDefault(c => c.Type=="sub");
            if(subClaim==null)
                return null;
            return subClaim.Value;
        }
    }
}