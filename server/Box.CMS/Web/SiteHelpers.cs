using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Security.Policy;
using Box.CMS;
using Box.CMS.Services;
using System.Web;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Box.CMS.Models;
using Microsoft.AspNetCore.Mvc.Razor;
using System.IO;
using System.Text.Encodings.Web;
using Box.CMS.Extensions;

namespace Box.CMS.Web
{

    /// <summary>
    /// A bunch of helpers to help accessing BOX contents at Razor pages.
    /// </summary>    
    public class BoxSite
    {        

        public static IHtmlContent Image(dynamic file, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, string cssClass = "", string vAlign = "center", string hAlign = "center", string mode = "")
        {         
            return new HtmlString("<img src=\"" + BoxLib.GetFileUrl((string)file.folder, (string)file.fileUId, width, height, maxWidth, maxHeight, false, vAlign, hAlign, mode) + "\" alt=\"" + file.Caption + "\" title=\"" + file.Caption + "\" class=\"" + cssClass + "\" />");
        }
        
        public static IHtmlContent Figure(dynamic file, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, string cssClass = "", string vAlign = "center", string hAlign = "center", string mode = "")
        {
            string str = "<figure>{0}{1}</figure>";
            string caption = "";
            if (file.caption != null && ((string)file.caption) != "")
            {
                caption = "<figcaption>" + file.caption + "</figcaption>";
            }
            return new HtmlString(String.Format(str, Image(file, width, height, maxWidth, maxHeight, cssClass, vAlign, hAlign, mode), caption));
        }

        public static IHtmlContent ContentLink(Models.ContentHead content)
        {
            return new HtmlString("<a href=\"" + BoxLib.GetContentLink(content) + "\" title=\"" + content.Name + "\">" + content.Name + "</a>");
        }

        public static IHtmlContent Contents(string[] kinds = null, Func<ContentHead, HelperResult> itemTemplate = null, Orders order = Orders.Date, Periods period = Periods.AnyTime, DateTime? createdFrom = null, DateTime? createdTo = null, bool parseContent = false, int top = 0, string navigationId = null, string location = null, string filter = null, IHtmlContent noItemMessage = null, System.Linq.Expressions.Expression<Func<ContentHead, bool>> queryFilter = null, int manualSkip = 0)
        {
            if (itemTemplate == null) {
                itemTemplate = (head) => { return new HelperResult(w =>
                    {
                        return w.WriteAsync("<li>" + ContentLink(head) + "</li>");
                    });
                };
            }
            string str = "";

            int skip = manualSkip;
            if (navigationId != null)
            {
                top = BoxLib.GetListPageSize(navigationId);
                skip = BoxLib.GetPageSkipForList(navigationId) * top;
            }
            

            DateTime? startDate = createdFrom;            
            if (period != Periods.AnyTime)
            {
                DateTime? lastPublished = DateTime.Now;
                lastPublished = BoxLib.GetLastPublishDate(location, kinds);
                startDate = period.StartDate(lastPublished);
            }


            var contents = BoxLib.GetContents(location, order, kinds, startDate, createdTo, parseContent, skip, top + 1, filter, queryFilter);
            var sb = new StringBuilder();            
            using (TextWriter tw = new StringWriter(sb))
            {                               
                int i = 0;            
                foreach (ContentHead head in contents) {
                    if (i < top) {
                        head.OrderIndex = i;
                        itemTemplate(head).WriteTo(tw, System.Text.Encodings.Web.HtmlEncoder.Default);
                        i++;
                    }
                }
                str = sb.ToString();
            }
            
            if (navigationId != null) {

                bool hasMorePage = contents.Count() == top + 1;
                int realCount = contents.Count();
                if (realCount > top)
                    realCount = top;

                BoxLib.SetListIsOver(navigationId, !hasMorePage);
                BoxLib.SetListCount(navigationId, realCount);
            }

            if (contents.Count() == 0)
            {
                if (noItemMessage != null)
                    return noItemMessage;
                else
                    return new HtmlString("<li>No items.</li>");
            }

            return new HtmlString(str);
        }
    
        public static IHtmlContent ContentsAtUrl(string url = "$currentUrl", Func<ContentHead, HelperResult> itemTemplate = null, Orders order = Orders.Date, bool parseContent = false, int top = 0, string navigationId = null, IHtmlContent noItemMessage = null, System.Linq.Expressions.Expression<Func<ContentHead, bool>> queryFilter = null, int manualSkip = 0)
        {
            return Contents(null, itemTemplate, order, Periods.AnyTime, null, null, parseContent, top, navigationId, url, null, noItemMessage, queryFilter, manualSkip);
        }

        public static IHtmlContent CrossLinksFrom(string pageArea, Func<ContentHead, HelperResult> itemTemplate = null, Orders order = Orders.CrossLinkDisplayOrder, int top = 0, string[] kinds = null, IHtmlContent noItemMessage = null, bool parseContent = false, string navigationId = null, string[] pageAreaFallBacks = null)
        {            
            if (itemTemplate == null) {
                itemTemplate = (head) => { return new HelperResult(w =>
                    {
                        return w.WriteAsync("<div style=\"background-image: url(" + BoxLib.GetFileUrl(head.ThumbFilePath, asThumb: true) + "); width: 150px; height: 150px;\">" + ContentLink(head) + "</div>");
                    });
                };
            }

            var heads = BoxLib.GetCrossLinksFrom(pageArea, order, top, kinds, parseContent, pageAreaFallBacks);
            string str = "";            
            var sb = new StringBuilder();            
            using (TextWriter tw = new StringWriter(sb))
            {               
                int i = 0;                
                foreach (ContentHead head in heads)
                {
                    head.OrderIndex = i;
                    itemTemplate(head).WriteTo(tw, System.Text.Encodings.Web.HtmlEncoder.Default);
                    i++;
                }                
            }

            str = sb.ToString();
            if (heads.Count() == 0)
            {
                if (noItemMessage != null)
                    return noItemMessage;
                else
                    return new HtmlString("<div>No items.</div>");
            }

            if (navigationId != null) {
                BoxLib.SetListIsOver(navigationId, heads.Count() < top);
                BoxLib.SetListCount(navigationId, heads.Count());
            }

            return new HtmlString(str);
        }

        public static IHtmlContent ContentsRelated(string id = null, Func<ContentHead, HelperResult> itemTemplate = null, string headerText = null, string location = null, string[] kinds = null, bool parseContent = false, int top = 0, string navigationId = null)
        {
            if (itemTemplate == null) {
                itemTemplate = (head) => { return new HelperResult(w => {
                        return w.WriteAsync("<li>" + ContentLink(head) + "</li>");
                    });
                };
            }

            string str = "";
            var heads = BoxLib.GetRelatedContents(id, location, kinds, parseContent, top);
            var sb = new StringBuilder();            
            using (TextWriter tw = new StringWriter(sb))
            {               
                foreach (ContentHead head in heads)
                    itemTemplate(head).WriteTo(tw, System.Text.Encodings.Web.HtmlEncoder.Default);
            }
            str = sb.ToString();
            if (headerText != null && !String.IsNullOrEmpty(str))
                str = headerText + str;

            if (navigationId != null) {
                BoxLib.SetListIsOver(navigationId, heads.Count() < top);
                BoxLib.SetListCount(navigationId, heads.Count());
            }

            HtmlString html = new HtmlString(str);
            return html;
        }

        public static IHtmlContent ContentsRelated(string[] tags, Func<ContentHead, HelperResult> itemTemplate = null, string headerText = null, string location = null, string[] kinds = null, bool parseContent = false, int top = 0, string navigationId = null)
        {
            if (itemTemplate == null) {
                itemTemplate = (head) => { return new HelperResult(w => {
                        return w.WriteAsync("<li>" + ContentLink(head) + "</li>");
                    });
                };
            }

            string str = "";
            var heads = BoxLib.GetRelatedContents(tags, location, kinds, parseContent, top);
            var sb = new StringBuilder();            
            using (TextWriter tw = new StringWriter(sb))
            {               
                foreach (ContentHead head in heads)
                    itemTemplate(head).WriteTo(tw, System.Text.Encodings.Web.HtmlEncoder.Default);
            }
            str = sb.ToString();
            if (headerText != null && !String.IsNullOrEmpty(str))
                str = headerText + str;

            if (navigationId != null) {
                BoxLib.SetListIsOver(navigationId, heads.Count() < top);
                BoxLib.SetListCount(navigationId, heads.Count());
            }

            HtmlString html = new HtmlString(str);
            return html;
        }

        public static IHtmlContent ContentsRelatedWithHotestThread(Func<ContentHead, HelperResult> itemTemplate = null, string location = null, string[] kinds = null, ContentRanks rankBy = ContentRanks.PageViews, Periods period = Periods.LastDay, int top = 5, string navigationId = null)
        {
            DateTime? lastPublished = DateTime.Now;
            if (period != Periods.AnyTime)
                lastPublished = BoxLib.GetLastPublishDate(location, kinds);

            ContentHead hotContent = BoxLib.GetHotestContent(kinds, location, rankBy, period.StartDate(lastPublished), null);
            if (hotContent == null)
                return new HtmlString("");
            return ContentsRelated(hotContent.TagsToArray(), itemTemplate, null, null, null, false, top, navigationId);
        }

    
    }

    /// <summary>
    /// Methods that can be used at Razor page to access BOX contents.
    /// </summary>
    public class BoxLib {

        private static IHttpContextAccessor httpContextAccessor;
        public static void SetHttpContextAccessor(IHttpContextAccessor  accessor) {
            httpContextAccessor = accessor;
        }

        private static HttpContext httpContext {
            get {
                if(httpContextAccessor==null) {
                    return null;                    
                }
                return httpContextAccessor.HttpContext;
            }
        }

        private static SiteService siteService {
            get {
                if(httpContextAccessor==null || httpContextAccessor.HttpContext==null) {
                    return null;                    
                }
                return httpContextAccessor.HttpContext.RequestServices.GetService(typeof(Box.CMS.Services.SiteService)) as Services.SiteService;
            }
        }

        public static string GetFileUrl(dynamic file)
        {
            return GetFileUrl(file.folder, file.fileUId);
        }

        public static string GetFileUrl(string folder, string fileUId, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, bool asThumb = false, string vAlign = "center", string hAlign = "center", string mode = "")
        {
            return GetFileUrl("/files/" + folder + "/" + fileUId, width, height, maxWidth, maxHeight, asThumb, vAlign, hAlign, mode);
        }

        public static string GetFileUrl(string filePath, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, bool asThumb = false, string vAlign = "center", string hAlign = "center", string mode = "")
        {                    
            string url =  filePath + "/?height=" + height + "&maxHeight=" + maxHeight + "&asThumb=" + asThumb.ToString().ToLower() + "&width=" + width + "&maxWidth=" + maxWidth + "&vAlign=" + vAlign + "&hAlign=" + hAlign + "&mode=" + mode;            
            return url;
        }

        public static string GetFileUrl(dynamic file, int width = 0, int height = 0, int maxWidth = 0, int maxHeight = 0, bool asThumb = false, string vAlign = "center", string hAlign = "center") {
            return GetFileUrl((string)file.folder + "/" + (string)file.fileUId, width, height, maxWidth, maxHeight, asThumb, vAlign, hAlign);
        }

        public static string GetContentLink(Models.ContentHead head)
        {            
            if (!String.IsNullOrEmpty(head.ExternalLinkUrl))
                return head.ExternalLinkUrl;
            
            return head.Location + head.CanonicalName;
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
        public static IEnumerable<Models.ContentHead> GetContents(string url, Orders order = Orders.Date, string[] kinds = null, DateTime? createdFrom = null, DateTime? createdTo = null, bool parseContent = false, int skip = 0, int top = 0, string filter = null, System.Linq.Expressions.Expression<Func<Models.ContentHead, bool>> queryFilter = null)
        {
            if (url != null && url.ToLower() == "$currenturl" && httpContext != null)
            {
                url = httpContext.Request.Path;                
            }                        
            return siteService.GetContents(url, order, kinds, createdFrom, createdTo, parseContent, skip, top, filter, queryFilter);
        }

        public static void SetListPageSize(string listId, int size)
        {            
            httpContext.Items["__" + listId + "pageSize"] = size;
        }
        public static int GetListPageSize(string listId)
        {            
            int? size = httpContext.Items["__" + listId + "pageSize"] as int?;
            if (!size.HasValue)
                size = 20;
            return size.Value;
        }

        internal static int GetPageSkipForList(string listId)
        {
            int skip = 0;
            if (httpContext == null)
                return 0;
            string skipStr = httpContext.Request.Query["_pageSkip_" + listId];
            if (String.IsNullOrEmpty(skipStr))
                return 0;
            int.TryParse(skipStr, out skip);
            return skip;
        }
        public static string ListNavigatonNextLink(string listId, string formId = null)
        {
            if (httpContext == null)
                return null;

            int actualSkip = GetPageSkipForList(listId);
            actualSkip++;
            return ListNavigationLink(listId, actualSkip, formId);
        }

        public static string ListNavigatonPreviousLink(string listId, string formId = null)
        {
            if (httpContext == null)
                return null;
            int actualSkip = GetPageSkipForList(listId);
            actualSkip--;
            if (actualSkip < 0)
                actualSkip = 0;
            return ListNavigationLink(listId, actualSkip, formId);
        }

        public static string ListNavigationLink(string listId, int skip, string formId = null)
        {

            if (httpContext == null)
                return String.Empty;

            HttpRequest request = httpContext.Request;

            string url = "";
            string query = "?";
            foreach (string key in request.Query.Keys)
            {
                if (key != "_pageSkip_" + listId)
                {
                    query = query + key + "=" + request.Query[key] + "&";
                }
            }
            query = query + "_pageSkip_" + listId + "=" + skip;
            url = request.Path + query;

            // new version - uses POST
            if (!String.IsNullOrEmpty(formId))
            {
                url = String.Format("javascript:{0}.action='{1}';{0}.submit();", formId, url);
            }

            return url;

        }

        internal static void SetListIsOver(string listId, bool isOver)
        {            
            if (httpContext == null)
                return;
            httpContext.Items["__" + listId + "isListOver"] = isOver;            
        }

        internal static void SetListCount(string listId, int count) {            
            if (httpContext == null)
                return;
            httpContext.Items["__" + listId + "Count"] = count;
        }

        public static bool GetListIsOver(string listId)
        {            
            if (httpContext == null)
                return false;
            bool? isOver = httpContext.Items["__" + listId + "isListOver"] as bool?;
            if (!isOver.HasValue)
                isOver = false;
            return isOver.Value;
        }

        public static int GetListCount(string listId) {            
            if (httpContext == null)
                return 0;
            int? count = httpContext.Items["__" + listId + "Count"] as int?;
            if (!count.HasValue)
                count = 0;
            return count.Value;
        }

        /// <summary>
        /// Gets the last time a content of a given kind was published at a given location.
        /// </summary>
        /// <param name="location">The location</param>
        /// <param name="kinds">The kind</param>
        /// <returns>The date when a content was last published</returns>
        public static DateTime? GetLastPublishDate(string location, string[] kinds) {
            return siteService.GetLastPublishDate(location, kinds);
        }

        public static IEnumerable<ContentHead> GetCrossLinksFrom(string pageArea, Orders order = Orders.CrossLinkDisplayOrder, int top = 0, string[] kinds = null, bool parseContent = false, string[] pageAreaFallBacks = null)
        {            
            return siteService.GetCrossLinksFrom(pageArea, order, top, kinds, parseContent, pageAreaFallBacks);
        }

        public static IEnumerable<ContentHead> GetRelatedContents(string id = null, string location = null, string[] kinds = null, bool parseContent = false, int top = 0)
        {            
            return siteService.GetRelatedContent(id, top, location, kinds, parseContent);
        }

        public static IEnumerable<ContentHead> GetRelatedContents(string[] tags, string location = null, string[] kinds = null, bool parseContent = false, int top = 0)
        {         
            return siteService.GetRelatedContent(tags, top, location, kinds, parseContent);
        }

        public static ContentHead GetHotestContent(string[] kinds, string location, ContentRanks rankBy = ContentRanks.PageViews, DateTime? createdFrom = null, DateTime? createdTo = null) {
            return siteService.GetHotestContent(kinds, location, rankBy, createdFrom, createdTo);
        }

        public static ContentTagRank[] GetTagCloud(string location, string[] kinds)
        {         
            return siteService.GetTagCloud(location, kinds);
        }
        

    }
    
}
