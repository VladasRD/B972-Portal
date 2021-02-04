using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box.Common;
using Box.Common.Web;
using Box.Security.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Box.Security.Api
{
    [Route("api/[controller]")]
    public class ProfileController : Controller
    {
        private readonly Services.SecurityService _securityService;
 
        public ProfileController(Services.SecurityService securityService)
        {
            _securityService = securityService;
        }
        
        [HttpPut("password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task SetPassword([FromBody] ChangePasswordParameters change)
        {                  
            var user = await _securityService.GetSignedUser();
            if (user == null)
            {
                throw new Box.Common.BoxLogicException("Could not find autheticated user to change password.");
            }      
            await _securityService.SetUserPassword(user, change.OldPassword, change.NewPassword);
        }

        

    }

    
    public class ChangePasswordParameters {
        public string NewPassword { get;set; }
        public string OldPassword { get;set; }
    }
}
