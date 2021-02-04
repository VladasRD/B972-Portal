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
    public class CMSCrossLinksController : Controller
    {
        private readonly CMS.Services.CMSService _cmsService;

        public CMSCrossLinksController(CMS.Services.CMSService cmsService)
        {
            _cmsService = cmsService;
        }

        [HttpGet]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "CROSSLINK.WRITE")]
        public IEnumerable<CrossLinkArea> Get() {            
            return _cmsService.GetCrossLinkAreas();            
        }

        [HttpPut("{area}/{id}/changeOrder")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "CROSSLINK.WRITE")]
        public void Put([FromRoute] string area, [FromRoute] string id, [FromBody] short changeOrder) {
            _cmsService.AddCrossLink(id, area, changeDisplayOrderBy: changeOrder);
        }    

        [HttpDelete("{area}/{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "CROSSLINK.WRITE")]
        public void Delete([FromRoute] string area, [FromRoute] string id)
        {
            _cmsService.RemoveCrossLink(area, id);
        }


    }
}