using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Box.Adm.Models;
using Box.Security.Models;
using Box.Adm.Models.AccountViewModels;
using Box.Adm.Services;
using Box.Common.Services;
using IdentityServer4;
using IdentityModel;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;
using System.Dynamic;
using Microsoft.AspNetCore.Http;
using System.Security.Principal;
using Box.Security.Services;

namespace Box.Adm.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {

        private readonly IStringLocalizer<Box.Common.Strings> Strings;
        private readonly IOptions<IdentityConfig> _identityConfig;
        private readonly Security.Services.SecurityService _securityService;
        private readonly SignInManager<Box.Security.Models.ApplicationUser> _signInManager;
        private LogService _log { get; set; }

        public AccountController(
            IOptions<IdentityConfig> identityConfig,
            IStringLocalizer<Box.Common.Strings> Strings,
            Security.Services.SecurityService securityService,
            SignInManager<Box.Security.Models.ApplicationUser> signInManager,
            LogService log)
        {

            _identityConfig = identityConfig;
            _securityService = securityService;
            this.Strings = Strings;
            _signInManager = signInManager;
            _log = log;
        }

        private void SetCultureCookieUsingRequest()
        {
            if (String.IsNullOrEmpty(Request.Query["culture"]))
            {
                return;
            }
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(Request.Query["culture"])),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );
        }


        [TempData]
        public string ErrorMessage { get; set; }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            SetCultureCookieUsingRequest();

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // creates the first adm user if the the db is empty
            await _securityService.CreateFirstUseAdm();

            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _securityService.SignIn(model.Email, model.Password, model.RememberMe);
                if (result.Succeeded)
                {
                    bool userIsLocked = _securityService.VerifyUserIsLockedByEmail(model.Email);
                    if (userIsLocked)
                        return RedirectToAction(nameof(AccessDenied));

                    return RedirectToLocal(returnUrl);
                }
                // if (result.RequiresTwoFactor)
                // {
                //     return RedirectToAction(nameof(LoginWith2fa), new { returnUrl, model.RememberMe });
                // }
                if (result.IsLockedOut)
                {
                    return RedirectToAction(nameof(Lockout));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Strings["Invalid login attempt"]);
                    return View(model);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Lockout()
        {
            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        [HttpGet]
        public IActionResult Logout()
        {
            _securityService.SignOut();
            return new RedirectResult(_identityConfig.Value.DEFAULT_CLIENT_URL + "/bye");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout2()
        {
            _securityService.SignOut();
            return RedirectToAction(nameof(Login));
        }

        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public IActionResult ExternalLogin(string provider, string returnUrl = null)
        //{
        //    // Request a redirect to the external login provider.
        //    var redirectUrl = Url.Action(nameof(ExternalLoginCallbackSecurity), "Account", new { returnUrl });
        //    var properties =  _securityService.SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        //    var result = Challenge(properties, provider);
        //    return result;
        //}

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLogin(string provider, string returnUrl = null)
        {

            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action(nameof(ExternalLoginCallbackSecurity), "Account", new { returnUrl });
            var properties = _securityService.SignInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);

            if (provider != "Windows")
            {
                return Challenge(properties, provider);
            }

            // https://stackoverflow.com/questions/46729801/windows-authentication-does-not-accept-credentials
            // I only care about Windows as an external provider
            var result = await HttpContext.AuthenticateAsync("Windows");
            if (result?.Principal is WindowsPrincipal wp)
            {
                var id = new ClaimsIdentity(provider);
                id.AddClaim(new Claim(JwtClaimTypes.Subject, wp.Identity.Name));
                id.AddClaim(new Claim(JwtClaimTypes.Name, wp.Identity.Name));

                await HttpContext.SignInAsync(
                    IdentityServerConstants.ExternalCookieAuthenticationScheme,
                    new ClaimsPrincipal(id),
                    properties);
                return Redirect(properties.RedirectUri);
            }
            else
            {
                return Challenge("Windows");
            }
        }


        /// <summary>
        /// For some reason the _signInManager.GetExternalLoginInfoAsync is not working.
        /// So write my own here.
        /// </summary>
        /// <returns>The external login info</returns>
        private async Task<ExternalLoginInfo> GetExternalLoginInfoAsync()
        {
            // read external identity from the temporary cookie (tries differents cookies names)
            var result = await HttpContext.AuthenticateAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            if (result?.Succeeded != true)
            {
                result = await HttpContext.AuthenticateAsync("idsrv");
                if (result?.Succeeded != true)
                {
                    // try the old way
                    return await _securityService.SignInManager.GetExternalLoginInfoAsync();
                }
            }

            // retrieve claims of the external user
            var externalUser = result.Principal;
            if (externalUser == null)
            {
                throw new Exception("External authentication error: could not find Principal at auth cookie");
            }

            // retrieve claims of the external user
            var claims = externalUser.Claims.ToList();

            // try to determine the unique id of the external user - the most common claim type for that are the sub claim and the NameIdentifier
            // depending on the external provider, some other claim type might be used
            var userIdClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.Subject);
            if (userIdClaim == null)
            {
                userIdClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier);
            }
            if (userIdClaim == null)
            {
                throw new Exception("External authentication error: could not find userid at claims");
            }

            var userName = "";
            var userNameClaim = claims.FirstOrDefault(x => x.Type == JwtClaimTypes.PreferredUserName);
            if (userNameClaim == null)
            {
                userNameClaim = claims.FirstOrDefault(x => x.Type == ClaimTypes.Name);
            }
            if (userNameClaim != null)
            {
                userName = userNameClaim.Value;
            }

            return new ExternalLoginInfo(result.Principal, userIdClaim.Issuer, userIdClaim.Value, userName);
        }

        /// <summary>
        /// Tries to sign in an external user.
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallbackSecurity(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            // (1) Gest the external user info
            var info = await this.GetExternalLoginInfoAsync();
            if (info == null)            
                throw new Exception("External authentication error: could not find auth cookie (tried 'idsrv' and 'idsrv.external')");
            
            // (2) Tries to sign in (the user needs to exists already)
            var user = await _securityService.ExternalSignIn(info);
            if (user != null)
            {
                if (user.LockoutEnd != null)
                    return RedirectToAction(nameof(AccessDenied));

                return RedirectToLocal(returnUrl);
            }
            
            // (3) If the user does not exists, and should not create new users -> The access is denied
            if(!_identityConfig.Value.AUTO_CREATE_EXTERNAL_USERS)
                return RedirectToAction(nameof(AccessDenied));
                        
            // (4) Otherwise, create a new user with the external info, and sign him in 
            ApplicationUser appUser = await CreateExternalUser(info);
            if (appUser==null)
                return RedirectToAction(nameof(AccessDenied));

            await _signInManager.SignInAsync(appUser, isPersistent: false);

            return RedirectToLocal(returnUrl);
            
        }

        private async Task<ApplicationUser> CreateExternalUser(ExternalLoginInfo info) {
            
            var emailClaim = info.Principal.Claims.SingleOrDefault(x => x.Type == System.Security.Claims.ClaimTypes.Email);            
            if (emailClaim == null) {// cant create user without email
                _log.Log("Cant create external user without e-mail");
                return null;
            }
            
            var nameClaim = new IdentityUserClaim<string>() { ClaimType = "given_name", ClaimValue = info.ProviderDisplayName };

            var claims = new IdentityUserClaim<string>[1];
            claims[0] = nameClaim;

            var appUser = new ApplicationUser();
            appUser.Email = emailClaim.Value; 
            appUser.UserName = emailClaim.Value;
            appUser.LoginNT = info.ProviderKey;
            appUser.EmailConfirmed = true;
            appUser.NormalizedUserName = info.ProviderDisplayName;
            appUser.NormalizedEmail = emailClaim.Value;
            appUser.UserClaims = claims;
            
            // create the user
            var userCreated = await _securityService.SignInManager.UserManager.CreateAsync(appUser);
            if (!userCreated.Succeeded) {
                _log.Log($"Error creating external user '{appUser.Email}'.", userCreated.Errors.First().Description, saveParameters:false);
                return null;
            }

            _log.Log($"User {appUser.Email} was created with success.", saveParameters:false);
            return appUser;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _securityService.GetUserByEmail(model.Email);
                //if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                if (user == null)
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToAction(nameof(ForgotPasswordConfirmation));
                }

                _securityService.SendForgotPasswordEmail(user, Url, Request);

                return RedirectToAction(nameof(ForgotPasswordConfirmation));
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPassword(string code = null)
        {
            if (code == null)
            {
                throw new ApplicationException("A code must be supplied for password reset.");
            }
            var model = new ResetPasswordViewModel { Code = code };
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _securityService.GetUserByEmail(model.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            var result = await _securityService.ResetUserPassword(user, model.Code, model.ConfirmPassword);
            if (result.Succeeded)
            {
                return RedirectToAction(nameof(ResetPasswordConfirmation));
            }
            AddErrors(result);
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }



        /*-------------------------------------------------------------------------- */
        /* Those was not converted yet to BOX: "2FA", "REGISTER" and "CONFIRM EMAIL" */

        /*
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWith2fa(bool rememberMe, string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            if (user == null)
            {
                throw new ApplicationException("Unable to load two-factor authentication user.");
            }

            var model = new LoginWith2faViewModel { RememberMe = rememberMe };
            ViewData["ReturnUrl"] = returnUrl;

            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWith2fa(LoginWith2faViewModel model, bool rememberMe, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var authenticatorCode = model.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorAuthenticatorSignInAsync(authenticatorCode, rememberMe, model.RememberMachine);

            if (result.Succeeded)
            {
                _logger.Log($"User with ID {user.Id} logged in with 2fa.", saveParameters:false);
                return RedirectToLocal(returnUrl);
            }
            else if (result.IsLockedOut)
            {
                _logger.Log($"User with ID {user.Id} account locked out.", saveParameters:false);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.Log($"Invalid authenticator code entered for user with ID {user.Id}.", saveParameters: false);
                ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                return View();
            }
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> LoginWithRecoveryCode(string returnUrl = null)
        {
            // Ensure the user has gone through the username & password screen first
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            ViewData["ReturnUrl"] = returnUrl;

            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LoginWithRecoveryCode(LoginWithRecoveryCodeViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                throw new ApplicationException($"Unable to load two-factor authentication user.");
            }

            var recoveryCode = model.RecoveryCode.Replace(" ", string.Empty);

            var result = await _signInManager.TwoFactorRecoveryCodeSignInAsync(recoveryCode);

            if (result.Succeeded)
            {
                _logger.Log("User with ID {$user.Id} logged in with a recovery code.", saveParameters:false);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                _logger.Log($"User with ID {user.Id} account locked out.", saveParameters:false);
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                _logger.Log($"Invalid recovery code entered for user with ID {user.Id}", saveParameters:false);
                ModelState.AddModelError(string.Empty, "Invalid recovery code entered.");
                return View();
            }
        }
    
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    _logger.Log($"User {model.Email} created a new account with password.", saveParameters:false);

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                    await _emailSender.SendEmailConfirmationAsync(model.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _logger.Log($"User {user.Email} created a new account with password.", saveParameters:false);
                    return RedirectToLocal(returnUrl);
                }
                AddErrors(result);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null, string remoteError = null)
        {
            if (remoteError != null)
            {
                ErrorMessage = $"Error from external provider: {remoteError}";
                return RedirectToAction(nameof(Login));
            }

            //var info = await _signInManager.GetExternalLoginInfoAsync(IdentityServerConstants.ExternalCookieAuthenticationScheme);
            var info = await this.GetExternalLoginInfoAsync();            
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false, bypassTwoFactor: true);
            if (result.Succeeded)
            {
                _logger.Log($"User logged in with {info.LoginProvider} provider.", saveParameters:false);
                return RedirectToLocal(returnUrl);
            }
            if (result.IsLockedOut)
            {
                return RedirectToAction(nameof(Lockout));
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                ViewData["ReturnUrl"] = returnUrl;
                ViewData["LoginProvider"] = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                return View("ExternalLogin", new ExternalLoginViewModel { Email = email });
            }
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginViewModel model, string returnUrl = null)
        {
            if (ModelState.IsValid)
            {
                // Get the information about the user from the external login provider
                var info = await this.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    throw new ApplicationException("Error loading external login information during confirmation.");
                }
                var user = new ApplicationUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user);
                if (result.Succeeded)
                {
                    result = await _userManager.AddLoginAsync(user, info);
                    if (result.Succeeded)
                    {
                        await _signInManager.SignInAsync(user, isPersistent: false);
                        _logger.Log($"User created an account using {info.LoginProvider} provider.", saveParameters:false);
                        return RedirectToLocal(returnUrl);
                    }
                }
                AddErrors(result);
            }

            ViewData["ReturnUrl"] = returnUrl;
            return View(nameof(ExternalLogin), model);
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
            
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{userId}'.");
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            return View(result.Succeeded ? "ConfirmEmail" : "Error");
        }
        */


        #region Helpers

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }
        }

        #endregion
    }
}
