using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Box.Common;
using Box.Common.Web;
using Microsoft.Extensions.Options;
using System.Dynamic;
using Microsoft.Extensions.Localization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Box.Security.Services
{
    public partial class SecurityService
    {

        /// <summary>
        /// Return roles filtered by a given filter.
        /// </summary>
        /// <param name="filter">The filter</param>
        /// <param name="skip">Users to skip for pagination</param>
        /// <param name="top">Number os users for pagination</param>
        /// <param name="totalCount">Object to hold the total count</param>
        /// <returns></returns>
        public IEnumerable<Models.ApplicationRole> GetRoles(string filter = null, int skip = 0, int top = 0, OptionalOutTotalCount totalCount = null, bool isSystemRole = false)
        {
            IQueryable<Models.ApplicationRole> roles = _context.Roles.Include(u => u.RoleClaims).AsQueryable();

            if (isSystemRole)
                roles = roles.Where(c => c.IsSystemRole);
   
            if (!string.IsNullOrEmpty(filter))
            {
                var tags = filter.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                roles = roles.Where(u =>
                tags.Any(t => u.Name.ToLower().Contains(t))
                || tags.Any(t => u.RoleClaims.Any(c => c.ClaimValue.ToLower().Contains(t))));
            }

            roles = roles.OrderBy(u => u.Name);

            if (totalCount != null)
            {
                totalCount.Value = roles.Count();
            }

            if (skip != 0)
                roles = roles.Skip(skip);

            if (top != 0)
                roles = roles.Take(top);

            return roles.ToArray();
        }

        /// <summary>
        /// Gets a given role by its id.
        /// </summary>
        /// <param name="id">The role id</param>
        /// <returns>The role</returns>
        public Models.ApplicationRole GetRole(string id)
        {
            return _context.Roles.Include(u => u.RoleClaims).SingleOrDefault(u => u.Id == id);
        }

        /// <summary>
        /// Updates a given role.
        /// </summary>
        /// <param name="role">The role to be updated</param>
        /// <returns>The updated role</returns>
        public Models.ApplicationRole UpdateRole(Models.ApplicationRole role)
        {
            // gets the old role and apply new values
            var oldRole = GetRole(role.Id);
            if(oldRole==null) {
                throw new Box.Common.BoxLogicException($"Can not find role {role.Id} to update.");
            }            
            role.NormalizedName = _roleManager.NormalizeKey(role.Name);
            _context.Entry(oldRole).State = EntityState.Modified;                
            _context.Entry(oldRole).CurrentValues.SetValues(role);

            // remove all claims from the role
            var roleClaims = oldRole.RoleClaims.ToArray();
            _context.RoleClaims.RemoveRange(roleClaims);
            
            // now add new claims
            var newClaims = role.RoleClaims.ToArray();            
            _context.RoleClaims.AddRange(newClaims);

            // ave everything at once
            _context.SaveChanges();
            
            _log.Log($"Role {role.Name} was updated.");

            return role;
        }

        /// <summary>
        /// Creates a new role
        /// </summary>
        /// <param name="role">The new role</param>
        /// <returns>The created role</returns>
        public async Task<Models.ApplicationRole> CreateRole(Models.ApplicationRole role)
        {
            var result = await _roleManager.CreateAsync(role);
            if (!result.Succeeded)
            {                
                throw new BoxLogicException($"Error creating role {role.Name}", result.Errors.First().Description);
            }

            _log.Log($"Role {role.Name} was created.");

            return role;
        }

        /// <summary>
        /// Deletes a given role.
        /// </summary>
        /// <param name="id">The id of the role to be deleted</param>
        /// <returns></returns>
        public async Task DeleteRole(string id)
        {            
            var role = await _roleManager.FindByIdAsync(id);
            if(role==null)
                return;

            if(HasRoleGotUsers(id)) {
                throw new BoxLogicException($"Can not delet role {role.Name} beacuse there are users associated with it.");
            }

            var result = await _roleManager.DeleteAsync(role);
            if (!result.Succeeded)
            {                
                throw new BoxLogicException($"Error deleting role {role.Name}", result.Errors.First().Description);
            }
            _log.Log($"Role {role.Name} was deleted.");
        }

        /// <summary>
        /// Checks if the role has users.
        /// </summary>
        /// <param name="id">The role id</param>
        /// <returns>True if the role has users</returns>
        public bool HasRoleGotUsers(string id) {
            return _context.UserRoles.Any(r => r.RoleId == id);
        }

        
        public async Task AddUserToRole(Models.ApplicationUser user, string role) {
            var result = await _userManager.AddToRoleAsync(user, role);            
            if (!result.Succeeded)
            {                
                throw new BoxLogicException($"Error adding user {user.UserName} to role {role}", result.Errors.First().Description);
            }
            _log.Log($"User {user.UserName} was added to role {role}.");
        }

        public async Task RemoveUserFromRole(Models.ApplicationUser user, string role) {
            var result = await _userManager.RemoveFromRoleAsync(user, role);
            if (!result.Succeeded)
            {                
                throw new BoxLogicException($"Error removing user {user.UserName} form role {role}", result.Errors.First().Description);
            }
            _log.Log($"User {user.UserName} was removed from role {role}.");
        }


    }
}
