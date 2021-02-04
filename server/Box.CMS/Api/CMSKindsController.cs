using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box.Common;
using Box.Common.Web;
using Box.CMS.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Box.CMS.Api
{
    [Route("api/[controller]")]
    public class CMSKindsController : Controller
    {
        private readonly CMS.Services.CMSService _cmsService;

        public CMSKindsController(CMS.Services.CMSService cmsService)
        {
            _cmsService = cmsService;
        }

        [HttpGet]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "CONTENT.WRITE")]
        public IEnumerable<ContentKind> Get() {            
            return _cmsService.GetContentKinds();            
        }

        [HttpGet("{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "CONTENT.WRITE")]
        public ContentKind Get(string id) {            
            return _cmsService.GetContentKind(id, true);            
        }


    }
}