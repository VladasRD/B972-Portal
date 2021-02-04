using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Box.CMS.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Box.CMS.Web
{
    public class ContentRenderPageModel : PageModel
    {
        public Models.ContentHead HEAD { get; private set; }
        public dynamic CONTENT = null;
        public string CONTENT_URL = null;

        protected readonly Services.SiteService _siteService;


        public ContentRenderPageModel(Services.SiteService siteService) {
            _siteService = siteService;
        }

        public void OnGet(Guid id)
        {
            HEAD = _siteService.GetContent(id.ToString());   
            if (HEAD == null)
                throw new Exception("page not found");

            LogPageView();

            CONTENT_URL = Request.Scheme + "://" + Request.Host.Host + (Request.Host.Port!=80?":" + Request.Host.Port:"") + HEAD.Location+HEAD.CanonicalName;
            CONTENT = HEAD.CONTENT;
         
        }

        public void LogPageView() {
            _siteService.LogPageView(HEAD.ContentUId);
        }

        public void LogFBPageShare(bool logShare = true, bool logComments = true)
        {              
            string host = Request.Scheme + "://" + Request.Host.ToUriComponent();
            var task = _siteService.LogFBPageShare(HEAD, host, logShare, logComments);
        }

    }
}
