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

        private const string WINDOWS_LOGIN_PROVIDER = "LOCAL AUTHORITY";


        /// <summary>
        /// Gets a given user by his id.
        /// </summary>
        /// <param name="id">The user id</param>
        /// <param name="includeAllClaims">True to include the user's claims</param>
        /// <returns>The user</returns>
        public Models.ApplicationUser GetUser(string id, bool includeAllClaims = false)
        {
            IQueryable<Models.ApplicationUser> users;

            if (includeAllClaims)
                users = _context.Users.Include(u => u.UserClaims).Include(u => u.UserRoles).AsQueryable();
            else
                users = _context.Users.AsQueryable();

            var user = users.SingleOrDefault(u => u.Id == id);

            // gets the login NT
            var loginNT = _context.UserLogins.SingleOrDefault(u => u.LoginProvider == WINDOWS_LOGIN_PROVIDER && u.UserId == id);
            if (loginNT != null)
            {
                user.LoginNT = loginNT.ProviderKey;
            }

            return user;
        }

        public async Task<Models.ApplicationUser> GetUserByEmail(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        /// <summary>
        /// Return users filtered by a given filter.
        /// </summary>
        /// <param name="filter">The filter</param>
        /// <param name="skip">Users to skip for pagination</param>
        /// <param name="top">Number os users for pagination</param>
        /// <param name="totalCount">Object to hold the total count</param>
        /// <param name="includeNameClaim">Include only the given_name claim</param>
        /// <param name="includeAllClaims">Include all claims</param>
        /// <param name="atRoles">filter only users at those roles</param>
        /// <param name="withClaims">filter only users with those claims</param>
        /// <returns></returns>
        public IEnumerable<Models.ApplicationUser> GetUsers(string filter = null, bool? lockedOut = null, int skip = 0, int top = 0, OptionalOutTotalCount totalCount = null, bool includeNameClaim = true, bool includeAllClaims = false, string[] atRoles = null, string[] withClaims = null)
        {

            IQueryable<Models.ApplicationUser> users;

            if (includeAllClaims || includeNameClaim)
                users = _context.Users.Include(u => u.UserClaims).Include(u => u.UserRoles).AsQueryable();
            else
                users = _context.Users.AsQueryable();

            if (atRoles != null && atRoles.Length > 0)
            {
                users = users.Where(u => atRoles.Any(rId => u.UserRoles.Any(ur => ur.RoleId == rId)));
            }

            if (withClaims != null && withClaims.Length > 0)
            {
                users = users.Where(u => withClaims.Any(c => u.UserClaims.Any(uc => uc.ClaimType == "role" && uc.ClaimValue == c)));
            }

            if (!string.IsNullOrEmpty(filter))
            {
                var tags = filter.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                users = users.Where(u =>
                tags.Any(t => u.UserName.ToLower().Contains(t))
                || tags.Any(t => u.Email.ToLower().Contains(t))
                || tags.Any(t => u.UserClaims.Any(c => c.ClaimType == "given_name" && c.ClaimValue.ToLower().Contains(t))));
            }

            // filter locked users
            var now = DateTime.Now;
            if (lockedOut == true)
            {
                users = users.Where(u => u.LockoutEnd != null && u.LockoutEnd >= now);
            }
            if (lockedOut == false)
            {
                users = users.Where(u => u.LockoutEnd == null || u.LockoutEnd < now);
            }

            users = users.OrderBy(u => u.UserName);

            if (totalCount != null)
            {
                totalCount.Value = users.Count();
            }

            if (skip != 0)
                users = users.Skip(skip);

            if (top != 0)
                users = users.Take(top);

            return users.ToArray();

        }

        /// <summary>
        /// Verifies if the password has any validation errors according the rules.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="password">The password</param>
        /// <returns>A list with validation errors</returns>
        public async Task<IEnumerable<IdentityError>> GetPasswordValidatorErrors(Models.ApplicationUser user, string password)
        {
            foreach (var v in _userManager.PasswordValidators)
            {
                var result = await v.ValidateAsync(_userManager, user, password);
                if (!result.Succeeded)
                    return result.Errors;
            }
            return new IdentityError[0];
        }

        /// <summary>
        /// Verifies if the new password is oh with the password rules.
        /// If not throws an exception.
        /// </summary>
        /// <param name="user">The user object with the CleanPassword field</param>
        /// <returns></returns>
        private async Task VerifyNewPassword(Models.ApplicationUser user)
        {
            if (String.IsNullOrEmpty(user.CleanPassword))
                return;

            var passErrors = await GetPasswordValidatorErrors(user, user.CleanPassword);
            if (passErrors.Count() > 0)
            {
                throw new BoxLogicException(passErrors.First().Description);
            }

        }


        /// <summary>
        /// Updates a given user.
        /// </summary>
        /// <param name="user">The user to be updated</param>
        /// <returns>The updated user</returns>
        public async Task<Models.ApplicationUser> UpdateUser(Models.ApplicationUser user)
        {

            await VerifyNewPassword(user);

            var oldUser = GetUser(user.Id, true);
            if (oldUser == null)
            {
                throw new Box.Common.BoxLogicException($"Cant find user {user.Id} to update");
            }

            // updates user fields
            _context.Entry(oldUser).State = EntityState.Modified;
            user.NormalizedUserName = _userManager.NormalizeKey(user.UserName);
            user.NormalizedEmail = _userManager.NormalizeKey(user.Email);
            user.SecurityStamp = Guid.NewGuid().ToString();
            user.PasswordHash = oldUser.PasswordHash;

            _context.Entry(oldUser).CurrentValues.SetValues(user);

            // if the password was changed
            if (!string.IsNullOrEmpty(user.CleanPassword))
            {
                oldUser.PasswordHash = _userManager.PasswordHasher.HashPassword(oldUser, user.CleanPassword);
            }

            // ROLES
            // remove all roles from the user
            var userRoles = oldUser.UserRoles.ToArray();
            _context.UserRoles.RemoveRange(userRoles);

            // now add new roles
            var newRoles = user.UserRoles.ToArray();
            _context.UserRoles.AddRange(newRoles);

            // CLAIMS
            // remove all roles from the user
            var userClaims = oldUser.UserClaims.ToArray();
            _context.UserClaims.RemoveRange(userClaims);

            // now add new roles
            var newClaims = user.UserClaims.ToArray();
            _context.UserClaims.AddRange(newClaims);

            // LOGIN NT
            UpdateUserLoginNT(user);

            _context.SaveChanges();

            _log.Log($"User {user.UserName} was updated.");

            return user;

        }

        private void UpdateUserLoginNT(Models.ApplicationUser user)
        {

            var loginNT = _context.UserLogins.SingleOrDefault(u => u.LoginProvider == WINDOWS_LOGIN_PROVIDER && u.UserId == user.Id);
            if (loginNT != null)
            {
                _context.UserLogins.Remove(loginNT);
            }

            // if LoginNT is empty, get out
            if (String.IsNullOrWhiteSpace(user.LoginNT))
            {
                return;
            }

            // create the new one            
            var newLoginNT = new IdentityUserLogin<string>();
            newLoginNT.LoginProvider = WINDOWS_LOGIN_PROVIDER;
            newLoginNT.UserId = user.Id;
            newLoginNT.ProviderDisplayName = "Windows";
            newLoginNT.ProviderKey = user.LoginNT;

            // add the new one
            _context.UserLogins.Add(newLoginNT);

        }

        /// <summary>
        /// Creates a new user.
        /// </summary>
        /// <param name="user">The new user</param>
        /// <returns>The created user</returns>
        public async Task<Models.ApplicationUser> CreateUser(Models.ApplicationUser user)
        {
            IdentityResult result;

            await VerifyNewPassword(user);

            result = await _userManager.CreateAsync(user);

            // now sets the password
            if (result.Succeeded && !string.IsNullOrEmpty(user.CleanPassword))
            {
                result = await _userManager.AddPasswordAsync(user, user.CleanPassword);
            }

            if (!result.Succeeded)
            {
                // verifica se o usuário já existe
                if (result.Errors.First().Code.ToLower().Equals("duplicateusername"))
                    throw new BoxLogicException($"Já existe um usuário com o e-mail {user.UserName}.", result.Errors.First().Description);
                    
                throw new BoxLogicException($"Error creating user {user.UserName}.", result.Errors.First().Description);
            }

            if (!string.IsNullOrEmpty(user.LoginNT))
            {
                var loginNT = new UserLoginInfo("LOCAL AUTHORITY", user.LoginNT, "Windows");
                result = await _userManager.AddLoginAsync(user, loginNT);
            }

            if (!result.Succeeded)
            {
                throw new BoxLogicException($"Error creating user {user.UserName}.", result.Errors.First().Description);
            }


            _log.Log($"User {user.UserName} was created.");

            return user;
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="id">The user id to be deleted</param>
        /// <returns></returns>
        public async Task DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new BoxLogicException($"Error deleting user {user.UserName}.", result.Errors.First().Description);
            }
            _log.Log($"User {user.UserName} was deleted.");
        }

        /// <summary>
        /// If the database does not have any user, creates the default adm.
        /// </summary>
        /// <returns></returns>
        public async Task CreateFirstUseAdm()
        {
            var userCount = _context.Users.Count();
            if (userCount > 0)
                return;

            var admClaims = new IdentityUserClaim<string>[6];
            admClaims[0] = new IdentityUserClaim<string>() { ClaimType = "role", ClaimValue = "USER.READ" };
            admClaims[1] = new IdentityUserClaim<string>() { ClaimType = "role", ClaimValue = "USER.WRITE" };
            admClaims[2] = new IdentityUserClaim<string>() { ClaimType = "role", ClaimValue = "ROLE.READ" };
            admClaims[3] = new IdentityUserClaim<string>() { ClaimType = "role", ClaimValue = "ROLE.WRITE" };
            admClaims[4] = new IdentityUserClaim<string>() { ClaimType = "role", ClaimValue = "LOG.READ" };
            admClaims[5] = new IdentityUserClaim<string>() { ClaimType = "given_name", ClaimValue = "Box adm" };
            var user = new Models.ApplicationUser() { Email = "adm@localhost", UserName = "adm@localhost", UserClaims = admClaims };

            var result = await _userManager.CreateAsync(user);
            if (!result.Succeeded)
                return;

            result = await _userManager.AddPasswordAsync(user, "Box#123");
        }

        public string GetUserGravatarUrl(string email, int size, string noImage = "mm")
        {
            return "http://www.gravatar.com/avatar/" + GravatarHashEmail(email) + ".jpg?s=" + size + "&d=" + noImage;
        }

        private string GravatarHashEmail(string email)
        {
            // Create a new instance of the MD5CryptoServiceProvider object.  
            System.Security.Cryptography.MD5 md5Hasher = System.Security.Cryptography.MD5.Create();

            // Convert the input string to a byte array and compute the hash.  
            byte[] data = md5Hasher.ComputeHash(System.Text.Encoding.Default.GetBytes(email));

            // Create a new Stringbuilder to collect the bytes  
            // and create a string.  
            System.Text.StringBuilder sBuilder = new System.Text.StringBuilder();

            // Loop through each byte of the hashed data  
            // and format each one as a hexadecimal string.  
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }

            return sBuilder.ToString();  // Return the hexadecimal string. 
        }

        public async Task SendDefaultUserCreatedEmail(Models.ApplicationUser user)
        {
            try
            {
                await _userManager.AddPasswordAsync(user, "Temp#123");

                dynamic emailModel = _templateService.CreateBasicModel();
                emailModel.email = user.Email;
                emailModel.password = "Temp#123";
                emailModel.siteUrl = _templateService.CLIENT_URL;
                emailModel.siteName = _boxSettings.APPLICATION_NAME;
                var emailBody = await _templateService.RenderTemplate("DefaultUserCreated", emailModel);
                await _emailSender.SendEmailAsync(
                    user.Email,
                    "Cadastro realizado",
                    emailBody);
            }
            catch (Exception ex)
            {
                await _log.LogErrorAsync($"Sending default user creation email {user.Email}.", ex.Message);
            }
        }

    }
}
