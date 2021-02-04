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
    public class RolesController : Controller
    {
        private readonly Services.SecurityService _securityService;

        public RolesController(Services.SecurityService securityService)
        {
            _securityService = securityService;
        }

        [HttpGet]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ROLE.READ")]
        public IEnumerable<ApplicationRole> Get([FromQuery] string filter, [FromQuery] int skip = 0, [FromQuery] int top = 0)
        {
            var totalCount = new OptionalOutTotalCount();

            var roles = _securityService.GetRoles(filter, skip, top, totalCount).ToList();
            Request.SetListTotalCount(totalCount.Value);

            return roles;
        }

        [HttpGet("system")]
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "SGI-CLIENT.READ, SGI-SUBCLIENT.READ")]
        public IEnumerable<ApplicationRole> system([FromQuery] string filter, [FromQuery] int skip = 0, [FromQuery] int top = 0)
        {
            var totalCount = new OptionalOutTotalCount();

            var roles = _securityService.GetRoles(filter, skip, top, totalCount, true).ToList();
            Request.SetListTotalCount(totalCount.Value);

            return roles;
        }

        [HttpGet("{id}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ROLE.READ")]
        public ApplicationRole Get(string id)
        {
            var role = _securityService.GetRole(id);
            return role;
        }

        [HttpPost]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ROLE.WRITE")]
        public async Task<ApplicationRole> Post([FromBody] ApplicationRole role)
        {
            role = await _securityService.CreateRole(role);
            return role;
        }

        [HttpPut("{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ROLE.WRITE")]
        public ApplicationRole Put(string id, [FromBody] ApplicationRole role)
        {
            role.Id = id;
            foreach(var claim in role.RoleClaims) {
                claim.RoleId = role.Id;                
            }
            role = _securityService.UpdateRole(role);
            return role;
        }

        [HttpDelete("{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "ROLE.WRITE")]
        public async Task Delete([FromRoute] string id)
        {
            await _securityService.DeleteRole(id);
        }

        [HttpGet("{id}/members")]          
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.READ")]
        public IEnumerable<ApplicationUser> GetMembers([FromRoute] string id, [FromQuery] int skip = 0, [FromQuery] int top = 0)
        {
            var totalCount = new OptionalOutTotalCount();

            var users = _securityService.GetUsers(null, null, skip, top, totalCount, true, false, new string[] { id }).ToList();
            users.ForEach(u => { u.RemoveNoIdentityClaims(); });

            Request.SetListTotalCount(totalCount.Value);
            
            return users;
        }

        [HttpPut("{id}/members/{userId}")]          
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.WRITE")]
        public async Task AddMember([FromRoute] string id, [FromRoute] string userId)
        {
            var user = _securityService.GetUser(userId);
            var role = _securityService.GetRole(id);
            if (user==null || role==null) return;
            await _securityService.AddUserToRole(user, role.Name);
        }

        [HttpDelete("{id}/members/{userId}")]          
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "USER.WRITE")]
        public async Task RemoveMember([FromRoute] string id, [FromRoute] string userId)
        {
            var user = _securityService.GetUser(userId);
            var role = _securityService.GetRole(id);
            if (user==null || role==null) return;
            await _securityService.RemoveUserFromRole(user, role.Name);
        }
    }
}
