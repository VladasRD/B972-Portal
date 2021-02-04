using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Box.Adm.Models;
using Microsoft.AspNetCore.Authorization;

namespace Box.Adm.Controllers
{
    public class HomeController : Controller
    {

        private readonly Security.Services.SecurityService _securityService;

        public HomeController(Security.Services.SecurityService securityService) {
            _securityService = securityService;
        }
        
        [Authorize]
        public async Task<IActionResult> Index()
        {
            var user = await _securityService.GetSignedUser();
            if (user!=null) {
                ViewData["UserName"] = user.UserName;
                ViewData["UserId"] = user.Id;
                ViewData["UserGravatarUrl"] = _securityService.GetUserGravatarUrl(user.Email, 50);
            }
            return View();
        }
        
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
