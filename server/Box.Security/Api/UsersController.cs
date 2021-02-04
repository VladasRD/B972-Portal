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
    public class UsersController : Controller
    {
        private readonly Services.SecurityService _securityService;
 
        public UsersController(Services.SecurityService securityService)
        {
            _securityService = securityService;
        }
        
        [HttpGet("{id}")]          
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.READ")]
        public ApplicationUser Get(string id)
        {
            var user = _securityService.GetUser(id, true);            
            return user;
        }
        
        [HttpGet]          
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.READ")]
        public IEnumerable<ApplicationUser> Get([FromQuery] string filter, [FromQuery] bool? lockedOut, [FromQuery] int skip = 0, [FromQuery] int top = 0,
            [FromQuery] string[] atRoles = null, [FromQuery] string[] withClaims = null)
        {
            var totalCount = new OptionalOutTotalCount();

            var users = _securityService.GetUsers(filter, lockedOut, skip, top, totalCount, true, false, atRoles, withClaims).ToList();
            users.ForEach(u => { u.RemoveNoIdentityClaims(); });

            Request.SetListTotalCount(totalCount.Value);
            
            return users;
        }

        [HttpPost]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.WRITE")]
        public async Task<ApplicationUser> Post([FromBody] ApplicationUser user)
        {
            user = await _securityService.CreateUser(user);
            return user;
        }

        [HttpPut("{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.WRITE")]
        public async Task<ApplicationUser> Put(string id, [FromBody] ApplicationUser user)
        {            
            user.Id = id;       
            foreach(var role in user.UserRoles) {
                role.UserId = user.Id;                
            }     
            foreach(var claim in user.UserClaims) {
                claim.UserId = user.Id;                
            }     
            user = await _securityService.UpdateUser(user);            
            return user;
        }

        [HttpDelete("{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.WRITE")]
        public async Task Delete([FromRoute] string id)
        {
            await _securityService.DeleteUser(id);
        }

        [HttpPut("_unlock/{id}")]           
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.WRITE")]
        public void Unlock(string id)
        {                        
            _securityService.UnlockUser(id);            
        }

        [HttpPut("_lock/{id}")]         
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.WRITE")]
        public void Lock(string id)
        {                        
            _securityService.LockUser(id);            
        }

    }
}
