using Box.CMS.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Box.CMS.Services
{
    
    /// <summary>
    /// Service to access BOX contents at any web site.
    /// </summary>
    public class SiteService {

        private readonly Data.CMSContext _context;
        protected readonly CMS.Services.CMSService _cmsService;

        public SiteService(Data.CMSContext context, Services.CMSService cmsService) {
            _context = context;
            _cmsService = cmsService;
        }

        /// <summary>
        /// Gets a BOX content.
        /// The content data will be parsed at the CONTENT property.
        /// </summary>
        /// <param name="contentUId">The content UId</param>
        /// <returns>The content</returns>
        public ContentHead GetContent(string contentUId) {
    
            ContentHead content = _cmsService.GetContent(contentUId, false);
            if(content==null)
                return null;

            _cmsService.ParseContentData(content);
            return content;
        }

        /// <summary>
        /// Gets BOX contents.
        /// </summary>
        /// <param name="url">The url where the contents where published</param>
        /// <param name="order">The order the contents should be returned</param>
        /// <param name="kinds">Only contents of these kinds will be returned</param>
        /// <param name="createdFrom">Only contents created after this date will be returned</param>
        /// <param name="createdTo">Only contents created until this date will be returned</param>
        /// <param name="parseContent">The contents will be parsed at property CONTENT</param>
        /// <param name="skip">Skip for pagination</param>
        /// <param name="top">Top for pagination</param>
        /// <param name="filter">The contents will be filter by this words</param>
        /// <param name="queryFilter">Any query filter</param>
        /// <returns>The contents</returns>
        public IEnumerable<ContentHead> GetContents(string url, Orders order = Orders.Date, string[] kinds = null, System.DateTime? createdFrom = null, DateTime? createdTo = null, bool parseContent = false, int skip = 0, int top = 0, string filter = null, System.Linq.Expressions.Expression<Func<ContentHead, bool>> queryFilter = null) {
            bool onlyPublished = !CanSeeUnpublishedContents();
            var totalCount = new Box.Common.Web.OptionalOutTotalCount();
            IEnumerable<ContentHead> contents = _cmsService.GetContents(filter, skip, top, url, kinds, totalCount, order, createdFrom, createdTo, parseContent, onlyPublished, queryFilter);
            
            if (parseContent) 
                foreach (ContentHead c in contents) 
                    _cmsService.ParseContentData(c);
            
            return contents;
        }

        /// <summary>
        /// Gets the last time a content of a given kind was published at a given location.
        /// </summary>
        /// <param name="location">The location</param>
        /// <param name="kinds">The kind</param>
        /// <returns>The date when a content was last published</returns>
        public DateTime? GetLastPublishDate(string location, string[] kinds) {
            return _cmsService.GetLastPublishDate(location, kinds);
        }

        // TODO: fazer can see contents
        private bool CanSeeUnpublishedContents() {

            return false;
            
            // var context = System.Web.HttpContext.Current;
            // if (context == null)
            //     return false;
            
            // string token = context.Request.QueryString["previewToken"];
            // if (String.IsNullOrEmpty(token))
            //     return false;

            // Core.Services.SecurityService security = new Core.Services.SecurityService();
            // var admCMS = new Groups.ADM_CMS();
            // var admSEC = new Core.Groups.ADM_SEC();

            // Core.User u = security.GetUserByAuthToken(token);
            // if (u == null)
            //     return false;

            // string[] roles = security.GetUserRoles(u);
            // return roles.Contains(admCMS.UserGroupUId) || roles.Contains(admSEC.UserGroupUId);            

        }


        public void LogPageView(string contentUId) {
                        
            ContentPageViewCount pageCount = _context.ContentPageViewCounts.SingleOrDefault(c => c.ContentUId == contentUId);
            if (pageCount == null) {
                _context.ContentPageViewCounts.Add(pageCount = new ContentPageViewCount() { ContentUId = contentUId, Count = 0 });
            }
            else {
                _context.ContentPageViewCounts.Attach(pageCount);
            }
            
            pageCount.Count++;

            _context.SaveChanges();
            
        }
        
        public IEnumerable<ContentHead> GetCrossLinksFrom(string pageArea, Orders order = Orders.CrossLinkDisplayOrder, int top = 0, string[] kinds = null, bool parseContent = false, string[] pageAreaFallBacks = null) {

            bool onlyPublished = !CanSeeUnpublishedContents();

            IEnumerable<ContentHead> contents = _cmsService.GetCrossLinksFrom(pageArea, order, top, kinds, parseContent, pageAreaFallBacks, onlyPublished);
            
            if (parseContent)
                foreach (ContentHead c in contents)
                    _cmsService.ParseContentData(c);
            
            return contents;
        }

        public IEnumerable<ContentHead> GetRelatedContent(string id, int top, string location, string[] kinds, bool includeData = false) {
            return _cmsService.GetRelatedContent(id, top, location, kinds, includeData);
        }

        public ContentHead GetHotestContent(string[] kinds, string location, ContentRanks rankBy = ContentRanks.PageViews, DateTime? createdFrom = null, DateTime? createdTo = null) {
            return _cmsService.GetHotestContent(kinds, location, rankBy, createdFrom, createdTo);
        }

        public IEnumerable<ContentHead> GetRelatedContent(string[] tags, int top, string location, string[] kinds, bool includeData = false) {
            return _cmsService.GetRelatedContent(tags, top, location, kinds, includeData);
        }

        public ContentTagRank[] GetTagCloud(string location, string[] kinds) {

            IQueryable<ContentTag> tags = _context.ContentTags.Where(t => t.Tag!=null && t.Tag!="");


            if (location != null) {
                location = location.ToLower();
                if (!location.EndsWith("/"))
                    location = location + "/";
                tags = tags.Where(t => _context.ContentHeads.Any(c => c.ContentUId == t.ContentUId && c.Location == location));
            }

            if (kinds != null)
                tags = tags.Where(t => _context.ContentHeads.Any(c => c.ContentUId == t.ContentUId && kinds.Contains(c.Kind.ToLower())));

            ContentTagRank[] tagTanks = tags
                .GroupBy(t => t.Tag)
                .Select(g => new ContentTagRank { Tag = g.Key, Rank = g.Count() })
                .OrderByDescending(tr => tr.Rank)
                .ToArray<ContentTagRank>();

            return tagTanks;
            
        }

        private void UpdateShareCount(string id, long count) {
            
            ContentShareCount oldCount = _context.ContentSharesCounts.SingleOrDefault(c => c.ContentUId == id);
            if (oldCount == null)
                _context.ContentSharesCounts.Add(new ContentShareCount() { ContentUId = id, Count = count });
            else
                oldCount.Count = count;
            _context.SaveChanges();            
        }


        public void UpdateCommentCount(string id, int count) {            
            ContentCommentCount oldCount = _context.ContentCommentCounts.SingleOrDefault(c => c.ContentUId == id);
            if (oldCount == null)
                _context.ContentCommentCounts.Add(new ContentCommentCount() { ContentUId = id, Count = count });
            else
                oldCount.Count = count;
            _context.SaveChanges();            
        }

        public async Task LogFBPageShare(ContentHead content, string serverHost, bool logShare = true, bool logComments = false) {

            string url = serverHost + content.Location + content.CanonicalName;


            System.Net.Http.HttpClient wc = new System.Net.Http.HttpClient();

            var msg = await wc.GetAsync("https://graph.facebook.com/?id=" + url);

            // if (task.IsFaulted && _cmsService.Settings.CMS_DEBUG) {
            //     string filePath = System.IO.Path.Combine(_cmsService.AppPath, "App_Data/logShareError" + DateTime.Now.ToString().Replace("/", ".").Replace(":", ".") + ".txt");
            //     using (StreamWriter sw = System.IO.File.CreateText(filePath)) {
            //         sw.WriteLine("https://graph.facebook.com/?id=" + url);
            //         sw.WriteLine("Exception: " + task.Exception.Message);
            //         if(task.Exception.InnerException!=null)
            //             sw.WriteLine("InnerException: " + task.Exception.InnerException.Message);
            //         if (task.Exception.InnerException.InnerException != null)
            //             sw.WriteLine("InnerException 2: " + task.Exception.InnerException.InnerException.Message);
            //     }
            // }

            // if (task.IsFaulted || task.IsCanceled)
            //     return;
            // var msg = task.Result;
            dynamic data = null;

            if (msg.StatusCode != System.Net.HttpStatusCode.OK) {
                if(_cmsService.Settings.CMS_DEBUG) {
                    string filePath = System.IO.Path.Combine(_cmsService.AppPath, "App_Data/logShareError" + DateTime.Now.ToString().Replace("/", ".").Replace(":", ".") + ".txt");
                    using (StreamWriter sw = System.IO.File.CreateText(filePath)) {
                        sw.WriteLine("https://graph.facebook.com/?id=" + url);
                        sw.WriteLine("ERROR CODE: " + msg.StatusCode);
                        sw.WriteLine("ERROR: " + await msg.Content.ReadAsStringAsync());
                    }
                }
                return;
            }

            string json = msg.Content.ReadAsStringAsync().Result;
            data = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);

            if (logShare) {
                if (data.shares == null)
                    UpdateShareCount(content.ContentUId, 0);
                else
                    UpdateShareCount(content.ContentUId, (long)data.shares);
            }

            if (logComments) {
                if (data.comments == null)
                    UpdateCommentCount(content.ContentUId, 0);
                else
                    UpdateCommentCount(content.ContentUId, (int)data.comments);
            }

                
        }



    }

}