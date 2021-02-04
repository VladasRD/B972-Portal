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
        private readonly Data.SecurityDbContext _context;

        private readonly SignInManager<Models.ApplicationUser> _signInManager;
        private readonly UserManager<Models.ApplicationUser> _userManager;
        private readonly RoleManager<Models.ApplicationRole> _roleManager;

        private readonly IHttpContextAccessor _httpContextAccessor;
        private LogService _log { get; set; }
        private readonly Common.Services.IEmailSender _emailSender;
        private readonly Common.Services.TemplateService _templateService;
        private readonly IStringLocalizer<Box.Common.Strings> Strings;
        private readonly Box.Common.BoxSettings _boxSettings;


        /// <summary>
        /// The Security service with methods for authentication, user and role management.
        /// </summary>
        /// <param name="context">The Db context</param>
        /// <param name="userManager">The AspNet Identity user manager</param>
        /// <param name="roleManager">The AspNet Identity role manager</param>
        /// <param name="log">The log service</param>
        public SecurityService(
            Data.SecurityDbContext context,
            SignInManager<Models.ApplicationUser> signinManager,
            UserManager<Models.ApplicationUser> userManager,
            RoleManager<Models.ApplicationRole> roleManager,
            LogService log,
            IOptions<Box.Common.BoxSettings> boxSettings,
            IStringLocalizer<Box.Common.Strings> strings,
            IHttpContextAccessor httpContextAccessor,
            Common.Services.IEmailSender emailSender,
            Common.Services.TemplateService templateService)
        {
            _context = context;
            _signInManager = signinManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _log = log;
            _boxSettings = boxSettings.Value;
            _templateService = templateService;
            _emailSender = emailSender;
            _httpContextAccessor = httpContextAccessor;
            this.Strings = strings;            
        }

        public async Task<Models.ApplicationUser> GetSignedUser() {
            var httpContext = _httpContextAccessor.HttpContext;
            if(httpContext==null) {
                return null;
            }            
            return await _userManager.GetUserAsync(httpContext.User);
        }

        public SignInManager<Models.ApplicationUser> SignInManager {
            get {
                return _signInManager;
            }
        }

        /// <summary>
        /// Signs in a user using an email and password.
        /// </summary>
        /// <param name="email">The user's email</param>
        /// <param name="password">The user's password</param>
        /// <param name="rememberMe">True to remeber the user</param>
        /// <returns>The sign in result</returns>
         public async Task<Microsoft.AspNetCore.Identity.SignInResult> SignIn(string email, string password, bool rememberMe)
        {
            
            var result = await _signInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure: true);
            if (result.Succeeded)
            {
                _log.Log($"User {email} logged in.", saveParameters:false);            
            }
            else if (result.IsLockedOut)
            {
                _log.Log($"User account {email} locked out.", saveParameters:false);                
            }
            else
            {             
                _log.Log($"Invalid login attempt from {email}.", saveParameters:false);             
            }

            return result;

        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>        
        public async void SignOut() {
            await _signInManager.SignOutAsync();
            _log.Log("User logged out.", saveParameters:false);
        }
        
        /// <summary>
        /// Verify if user is locked.
        /// </summary>
        /// <param name="id">The id of the user to verify is locked</param>
        public bool VerifyUserIsLockedByKey(string id)
        {
            var user = GetUser(id);
            return user.LockoutEnd != null;
        }

        /// <summary>
        /// Verify if user is locked.
        /// </summary>
        /// <param name="id">The email of the user to verify is locked</param>
        public bool VerifyUserIsLockedByEmail(string email)
        {
            var user = GetUserByEmail(email);
            if (user == null)
                return false;
            return user.Result.LockoutEnd != null;
        }

        /// <summary>
        /// Signs in a user authenticated ata external provider.
        /// The user must exist in the database prior to the authentication.
        /// </summary>
        /// <param name="info">The external provider authetication info</param>
        /// <returns>The user is success, null if case of fail.</returns>
        public async Task<Models.ApplicationUser> ExternalSignIn(ExternalLoginInfo info) {
                        
            if (info == null)
            {
                return null;
            }

            // verify if user is in database
            Models.ApplicationUser user = null;
            string userNick = info.ProviderKey;
            if (info.Principal.Identity.AuthenticationType == "Windows")
            {
                //user = await _userManager.FindByNameAsync(info.ProviderKey);
                userNick = info.ProviderKey;
                user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            }
            else
            {
                var emailClaim = info.Principal.Claims.SingleOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email);
                if (emailClaim != null)
                {
                    userNick = emailClaim.Value;
                    user = await _userManager.FindByEmailAsync(userNick);
                }
            }

            if (user != null)
            {
                await _userManager.AddLoginAsync(user, info);
                await _signInManager.SignInAsync(user, isPersistent: false);
                _log.Log($"User {userNick} was authenticated externally with success.", saveParameters:false);
                return user;
            }
            
            _log.Log($"User {userNick} was authenticated externally but is not registered in the applicaton (provider={info.LoginProvider}).", saveParameters:false);
            await _signInManager.SignOutAsync();
            return null;
            
        }
                
        /// <summary>
        /// Unlocks a user.
        /// </summary>
        /// <param name="id">The id of the user to be unlocked</param>
        public void UnlockUser(string id) {
            var user = GetUser(id);
            user.LockoutEnd = null;
            user.AccessFailedCount = 0;
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            _log.Log($"User {user.Email} was unlocked.", saveParameters:false);
        }

        /// <summary>
        /// Locks a user.
        /// </summary>
        /// <param name="id">The id of the user to be locked</param>
        public void LockUser(string id) {
            var user = GetUser(id);
            user.LockoutEnd = DateTimeOffset.MaxValue;            
            _context.Entry(user).State = EntityState.Modified;
            _context.SaveChanges();
            _log.Log($"User {user.Email} was locked.", saveParameters:false);
        }

        /// <summary>
        /// Sends an email with a link to the user reset his password.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="url">Url helper used to create the link</param>
        /// <param name="request">The request</param>        
        public async void SendForgotPasswordEmail(Models.ApplicationUser user, IUrlHelper url, HttpRequest request) {

            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            var callbackUrl = url.ResetPasswordCallbackLink(user.Id, code, request.Scheme);
            
            dynamic data = _templateService.CreateBasicModel();
            data.callbackUrl = callbackUrl;                
            
            var body = await _templateService.RenderTemplate("ForgotPassword", data);

            try {
                await _emailSender.SendEmailAsync(
                    user.Email,
                    String.Format(Strings["ResetPasswordSubject"], _boxSettings.APPLICATION_NAME),
                    body);
                _log.Log($"An email was send to {user.Email} reset his password.", saveParameters:false);
            }
            catch(Exception ex) {
                _log.Log("Sending reset password email.", ex.Message);
            }
            
        }

        /// <summary>
        /// Resets the user password.
        /// </summary>
        /// <param name="user">The user</param>
        /// <param name="code">The code that was send to his email</param>
        /// <param name="newPassword">The new password</param>
        /// <returns>The result</returns>
        public async Task<IdentityResult> ResetUserPassword(Models.ApplicationUser user, string code, string newPassword) {
            var result = await _userManager.ResetPasswordAsync(user, code, newPassword);
            if (result.Succeeded)
            {
                _log.Log($"User {user.Email} reseted his password.", saveParameters: false);
            }
            return result;
        }

        public async Task SetUserPassword(Models.ApplicationUser user, string currentPass, string newPass) {
            var result = await _userManager.ChangePasswordAsync(user, currentPass, newPass);
            if(!result.Succeeded) {
                throw new BoxLogicException("Could not set user password: " + result.Errors.First().Description, result.Errors.First().Description);
            }            
            _log.Log($"User {user.UserName}'s password was changed.", saveParameters: false);

            
            dynamic data = _templateService.CreateBasicModel();
            var body = await _templateService.RenderTemplate("PasswordChanged", data);
            try {
                await _emailSender.SendEmailAsync(
                    user.Email,
                    String.Format(Strings["PasswordChangedSubject"], _boxSettings.APPLICATION_NAME),
                    body);
            }
            catch(Exception ex) {
                _log.Log("Sending change password email.", ex.Message, false);
            }
                
            
            
        }
       
    }
}
