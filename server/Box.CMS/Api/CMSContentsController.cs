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
    public class CMSContentsController : Controller
    {
        private readonly CMS.Services.CMSService _cmsService;

        public CMSContentsController(CMS.Services.CMSService cmsService)
        {
            _cmsService = cmsService;
        }


        [HttpGet("{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "CONTENT.READ")]
        public ContentHead Get(string id) {
            ContentHead head = _cmsService.GetContent(id);
            _cmsService.VerifyAuthorizationToEditContent(User, head.Kind);
            return head;
        }

        [HttpGet]          
        [PaginationHeaderFilter]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "CONTENT.READ")]
        public IEnumerable<ContentHead> Get([FromQuery] string filter, [FromQuery] bool? lockedOut, [FromQuery] string location = null, [FromQuery] string kind = null, [FromQuery] int skip = 0, [FromQuery] int top = 0, [FromQuery] bool onlyPublished = false, [FromQuery] string orderBy = "Date", [FromQuery] string area = null)
        {
            var totalCount = new OptionalOutTotalCount();

            object order;
            Enum.TryParse(typeof(Orders), orderBy, true, out order);
            if(order==null) {
                order = Orders.Date;
            }

            IEnumerable<ContentHead> contents = null;

            if(!string.IsNullOrEmpty(area))
                contents = this._cmsService.GetCrossLinksFrom(area, top: top, order: (Orders) order, onlyPublished: false);
            else
                contents = this._cmsService.GetContents(filter, skip, top, location, new string[] { kind }, totalCount, (Orders) order, null, null, false, onlyPublished).ToList();
            
            Request.SetListTotalCount(totalCount.Value);
            
            return contents;
        }

        [HttpPost]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ContentHead Post([FromBody] ContentHead content, [FromQuery] int publishNow = 0) {

            this._cmsService.VerifyAuthorizationToEditContent(User, content.Kind);

            content.ContentUId = Guid.NewGuid().ToString();
            content.CreateDate = DateTime.Now.ToUniversalTime();
            if(content.Data==null) {
                content.Data = new ContentData();
            }
            content.Data.ContentUId = content.ContentUId;
            FormatContentTags(content);
            FormatContentCrossLinks(content);

            
            FormatLocation(content);
            

            if (content.ContentDate == DateTime.MinValue)
                content.ContentDate = DateTime.Now.ToUniversalTime();

            if (publishNow > 0) {
                DateTime now = DateTime.Now.ToUniversalTime();
                content.PublishAfter = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            }

            SetCustomInfo(content);

            return this._cmsService.SaveContent(content);
        }

        [HttpPut("{id}")]           
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public ContentHead Put(string id, [FromBody] ContentHead content, [FromQuery] int publishNow = 0) {

            var head = this._cmsService.GetContent(id);
            this._cmsService.VerifyAuthorizationToEditContent(User, head.Kind);
            
            if(content.Data==null) {
                content.Data = new ContentData();
            }
            content.Data.ContentUId = content.ContentUId;
            FormatContentTags(content);
            FormatContentCrossLinks(content);
            FormatLocation(content);
            if (content.ContentDate == DateTime.MinValue)
                content.ContentDate = DateTime.Now.ToUniversalTime();

            if (publishNow > 0) {
                DateTime now = DateTime.Now.ToUniversalTime();
                content.PublishAfter = new DateTime(now.Year, now.Month, now.Day, now.Hour, 0, 0);
            }

            SetCustomInfo(content);

            return this._cmsService.SaveContent(content);
        }

        [HttpDelete("{id}")]        
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public void Delete(string id)
        {
            var head = this._cmsService.GetContent(id);
            this._cmsService.VerifyAuthorizationToEditContent(User, head.Kind);
            _cmsService.RemoveContent(id);
        }



        private void SetCustomInfo(ContentHead head) {
            if (head.CustomInfo == null)
                return;
            head.CustomInfo.ContentUId = head.ContentUId;
        }

        private void FormatContentCrossLinks(ContentHead content) {
            if(content.CrossLinks==null) {
                content.CrossLinks = new CrossLink[0];
            }
            foreach (CrossLink x in content.CrossLinks)
                x.ContentUId = content.ContentUId;
        }
        
        private void FormatContentTags(ContentHead content) {
            if(content.Tags==null) {
                content.Tags = new ContentTag[0];
            }
            foreach (ContentTag t in content.Tags) {
                t.ContentUId = content.ContentUId;
                t.Tag = t.Tag.Trim();
            }
            content.Tags =  content.Tags.Distinct().ToArray();
        }

        private void FormatLocation(ContentHead content) {
            content.Name = content.Name.Trim();
            content.CanonicalName = _cmsService.CanonicalName(content.Name);
            if (!content.Location.EndsWith("/"))
                content.Location = content.Location + "/";
            content.Location = content.Location.ToLower();

            
            ContentHead contentWithSameUrl = _cmsService.GetContentHeadByUrlAndKind(content.Location + content.CanonicalName, null, false);
            if (contentWithSameUrl == null || contentWithSameUrl.ContentUId == content.ContentUId)
                return;

            // if already exists a different content with the same url and browsable, throws an error
            ContentKind ckind = _cmsService.GetContentKind(content.Kind);
            if (ckind.Browsable.HasValue && (bool)ckind.Browsable) {
                content.CanonicalName = MakeCanonicalNameUnique(content);
                contentWithSameUrl = _cmsService.GetContentHeadByUrlAndKind(content.Location + content.CanonicalName, null, false);
                if (contentWithSameUrl != null && contentWithSameUrl.ContentUId != content.ContentUId)
                    throw new Exception("Content with same name already exists");
            }

            return;
        }

         private string MakeCanonicalNameUnique(ContentHead content) {
            return content.CanonicalName += "-" + content.ContentDate.ToString("dd-MMM-yyyy-HH-mm-ss");            
        }
    }
}
