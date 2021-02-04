using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Collections;
using Microsoft.EntityFrameworkCore;
using Box.Common.Web;
using Box.CMS.Models;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Box.Security.Services;
using System.Security.Claims;

namespace Box.CMS.Services
{

    public partial class CMSService
    {

        private ContentKind[] kindsCache = null;

        private readonly Data.CMSContext _context;
        public Box.Common.BoxSettings Settings { get; private set;}
        public string WebPath { get; private set; }
        public string AppPath { get; private set; }
        private LogService _log { get; set; }

        

        public CMSService(
            Data.CMSContext context,
            IOptions<Box.Common.BoxSettings> boxSettings,
            LogService log,
            IHostingEnvironment env) {
            _context = context;
            Settings = boxSettings.Value;
            _log = log;
            WebPath = env.WebRootPath;
            AppPath = env.ContentRootPath;
        }
        
        /// <summary>
        /// Get contents using several filters.
        /// </summary>
        /// <param name="filter">Search filter to be used at Name, Abstract, Tags and CustomIngo fields</param>
        /// <param name="skip">Skip those first contents</param>
        /// <param name="top">Max number of contents to be returned</param>
        /// <param name="location">Returns only contents at this location</param>
        /// <param name="kinds">Returns only contents of these kinds</param>
        /// <param name="order">Returns content ordered (DESC or ASC) by one of the following Name, Date, DisplayOrder, Comments, Share, PageView, Random, RandomOnDay, CrossLinkDisplayOrder, CustomFields</param>
        /// <param name="createdFrom">Retuns only contents created after this date will be returned</param>
        /// <param name="createdTo">Returns only contents created until this date will be returned</param>
        /// <param name="includeData">Use True to include content data</param>
        /// <param name="onlyPublished">Use True for only published content</param>
        /// <param name="queryFilter">A custom content query</param>
        /// <returns></returns>
        public IEnumerable<ContentHead> GetContents(string filter, int skip, int top, string location, string[] kinds, OptionalOutTotalCount totalCount = null, Orders order = Orders.Date, DateTime? createdFrom = null, DateTime? createdTo = null, bool includeData = false, bool onlyPublished = false, System.Linq.Expressions.Expression<Func<ContentHead, bool>> queryFilter = null)
        {
            
            IQueryable<ContentHead> contents = null;

            if (!includeData)
                contents = _context.ContentHeads.Include(c => c.CrossLinks).Include(c => c.CommentsCount).Include(c => c.ShareCount).Include(c => c.PageViewCount).Include(c => c.Tags).Include(c => c.CustomInfo);
            else
                contents = _context.ContentHeads.Include(c => c.CrossLinks).Include(c => c.CommentsCount).Include(c => c.ShareCount).Include(c => c.PageViewCount).Include(c => c.Tags).Include(c => c.CustomInfo).Include(c => c.Data);

            if (createdFrom.HasValue)
                contents = contents.Where(c => c.ContentDate >= createdFrom.Value);

            if (createdTo.HasValue)
                contents = contents.Where(c => c.ContentDate <= createdTo.Value);

            if (onlyPublished)
                contents = OnlyPublishedContents(contents);

            if (queryFilter != null)
            {
                contents = contents.Where(queryFilter);
            }

            if (!String.IsNullOrEmpty(filter))
            {
                filter = filter.ToLower();
                contents = contents.Where(c =>
                    (c.CustomInfo != null && (c.CustomInfo.Text1.ToLower().StartsWith(filter) || c.CustomInfo.Text2.ToLower().StartsWith(filter) || c.CustomInfo.Text3.ToLower().StartsWith(filter) || c.CustomInfo.Text4.ToLower().StartsWith(filter))) ||
                    c.Name.ToLower().Contains(filter) ||
                    c.Abstract.ToLower().Contains(filter) ||
                    c.Tags.Any(t => t.Tag.Contains(filter)) ||
                    c.CrossLinks.Any(x => x.PageArea.Contains(filter)));
            }

            contents = OrderContents(contents, order);

            if (location != null && location.ToLower() != "null")
            {
                location = location.ToLower();
                if (!location.EndsWith("/"))
                    location = location + "/";
                contents = contents.Where(c => c.Location == location);
            }

            if (kinds != null && kinds.Any(k => k != null))
                contents = contents.Where(c => kinds.Contains(c.Kind.ToLower()));

            if (skip != 0)
                contents = contents.Skip(skip);

            if (top != 0)
                contents = contents.Take(top);

            if (totalCount != null)
            {
                totalCount.Value = contents.Count();
            }

            return contents.ToArray();
            
        }
        
        private IQueryable<ContentHead> OrderContents(IQueryable<ContentHead> contents, Orders order, string pageArea = null)
        {
            if (order == Orders.Name)
                return contents.OrderBy(c => c.Name).ThenByDescending(c => c.ContentDate);
            if (order == Orders.DateASC)
                return contents.OrderBy(c => c.ContentDate);
            if (order == Orders.DisplayOrder)
                return contents.OrderBy(c => c.DisplayOrder).ThenByDescending(c => c.ContentDate);
            if (order == Orders.Comments)
                return contents.OrderBy(c => c.CommentsCount.Count);
            if (order == Orders.CommentsDESC)
                return contents.OrderByDescending(c => c.CommentsCount.Count);
            if (order == Orders.Share)
                return contents.OrderBy(c => c.ShareCount.Count);
            if (order == Orders.ShareDESC)
                return contents.OrderByDescending(c => c.ShareCount.Count);
            if (order == Orders.PageView)
                return contents.OrderBy(c => c.PageViewCount.Count);
            if (order == Orders.PageViewDESC)
                return contents.OrderByDescending(c => c.PageViewCount.Count);

            if (order == Orders.Random)
            {
                var rand = new Random();
                int r = rand.Next();
                return contents.OrderByDescending(c =>
                    (~(((c.CreateDate.Second + c.CreateDate.Minute + c.CreateDate.Hour + c.CreateDate.Millisecond + c.ContentDate.Day) & r))
                    & ((c.CreateDate.Second + c.CreateDate.Minute + c.CreateDate.Hour + c.CreateDate.Millisecond + c.ContentDate.Day) | r)));
            }

            if (order == Orders.CrossLinkDisplayOrder && pageArea != null)
                return contents.OrderByDescending(c => c.CrossLinks.Where(x => x.PageArea == pageArea).FirstOrDefault().DisplayOrder).ThenByDescending(c => c.ContentDate);

            if (order == Orders.RandomOnDay)
            {

                int dw = (int)System.DateTime.Today.DayOfWeek * 100;
                int dd = System.DateTime.Today.Day * 50;
                int r = (System.DateTime.Today.DayOfYear * 15) + dw + dd;
                return contents.OrderByDescending(c =>
                    (~(((c.CreateDate.Second + c.CreateDate.Minute + c.CreateDate.Hour + c.CreateDate.Millisecond + c.ContentDate.Day) & r))
                    & ((c.CreateDate.Second + c.CreateDate.Minute + c.CreateDate.Hour + c.CreateDate.Millisecond + c.ContentDate.Day) | r)));

            }

            // CUSTOM ORDERS
            if (order == Orders.CustomNumber1)
            {
                return contents.OrderBy(c => c.CustomInfo.Number1).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomNumber2)
            {
                return contents.OrderBy(c => c.CustomInfo.Number2).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomNumber3)
            {
                return contents.OrderBy(c => c.CustomInfo.Number3).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomNumber4)
            {
                return contents.OrderBy(c => c.CustomInfo.Number4).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomNumber1DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Number1).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomNumber2DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Number2).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomNumber3DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Number3).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomNumber4DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Number4).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomText1)
            {
                return contents.OrderBy(c => c.CustomInfo.Text1).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomText2)
            {
                return contents.OrderBy(c => c.CustomInfo.Text2).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomText3)
            {
                return contents.OrderBy(c => c.CustomInfo.Text3).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomText4)
            {
                return contents.OrderBy(c => c.CustomInfo.Text4).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomText1DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Text1).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomText2DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Text2).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomText3DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Text3).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomText4DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Text4).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomDate1)
            {
                return contents.OrderBy(c => c.CustomInfo.Date1).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomDate2)
            {
                return contents.OrderBy(c => c.CustomInfo.Date2).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomDate3)
            {
                return contents.OrderBy(c => c.CustomInfo.Date3).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomDate4)
            {
                return contents.OrderBy(c => c.CustomInfo.Date4).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomDate1DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Date1).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomDate2DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Date2).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomDate3DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Date3).ThenBy(c => c.DisplayOrder);
            }
            if (order == Orders.CustomDate4DESC)
            {
                return contents.OrderByDescending(c => c.CustomInfo.Date4).ThenBy(c => c.DisplayOrder);
            }

            return contents.OrderByDescending(c => c.ContentDate);
        }
        private IQueryable<ContentHead> OnlyPublishedContents(IQueryable<ContentHead> contents)
        {
            DateTime now = DateTime.Now.ToUniversalTime();
            return contents.Where(c => (c.PublishAfter.HasValue && c.PublishAfter <= now)
                && (!c.PublishUntil.HasValue || c.PublishUntil >= now));
        }

        public ContentHead GetContent(string id, bool onlyPublished = false)
        {          
            IQueryable<ContentHead> content = _context.ContentHeads.Include("Data").Include("CrossLinks").Include("Tags").Include("CommentsCount").Include("CustomInfo").Where(c => c.ContentUId == id);
            if (onlyPublished)
                content = OnlyPublishedContents(content);
            return content.SingleOrDefault();          
        }

        public ContentHead SaveContent(ContentHead content)
        {            
            ContentHead oldContent = GetContent(content.ContentUId);
            List<CrossLink> removedLinks = new List<CrossLink>();
            List<CrossLink> addedLinks = content.CrossLinks.ToList();

            if (oldContent == null)
            {
                _context.ContentHeads.Add(content);
            }
            else
            {
                _context.ContentHeads.Attach(oldContent);
                _context.Entry<ContentHead>(oldContent).CurrentValues.SetValues(content);
                _context.Entry<ContentData>(oldContent.Data).CurrentValues.SetValues(content.Data);
                if (oldContent.CustomInfo != null)
                    _context.Entry<ContentCustomInfo>(oldContent.CustomInfo).CurrentValues.SetValues(content.CustomInfo);
            }

            
            ApplyCollectionValuesCrossLinks(oldContent != null ? oldContent.CrossLinks : null, content.CrossLinks);
            //_context.ApplyCollectionValues<CrossLink>(oldContent != null ? oldContent.CrossLinks : null, content.CrossLinks, (c1, c2) => { return c1.PageArea == c2.PageArea; });

            _context.ApplyCollectionValues<ContentTag>(oldContent != null ? oldContent.Tags : null, content.Tags, (t1, t2) => { return t1.Tag == t2.Tag; });

            _context.SaveChanges();
            
            _log.Log($"Content '{content.Name}' ({content.Kind}) was created/updated.");

            return content;
        }

        public void VerifyAuthorizationToEditContent(ClaimsPrincipal user, string kind)
        {            
            if (!CanEditContent(user, GetContentKind(kind)))
                throw new System.Security.SecurityException("Not authorized to edit content");            
        }

        
        public bool CanEditContent(ClaimsPrincipal user, ContentKind kind)
        {                  
            if (kind == null)
                return true;

            // if CMS ADM, can edit anything, get out of here
            if (user.IsInRole("CONTENT.WRITE"))
                return true;

            foreach (string role in kind.RequiredRolesToEdit)
            {
                if (user.IsInRole(role))
                {
                    return true;
                }
            }
            return false;
        }

        public string CanonicalName(string text)      
        {
            StringBuilder sbReturn = new StringBuilder();
            var arrayText = text.Normalize(NormalizationForm.FormD).ToCharArray();

            foreach (char letter in arrayText)
            {
                if (System.Globalization.CharUnicodeInfo.GetUnicodeCategory(letter) != System.Globalization.UnicodeCategory.NonSpacingMark)
                    sbReturn.Append(letter);
            }
            string s1 = sbReturn.ToString();

            //apenas letras, números, espaços e hífen

            s1 = s1.Replace("/", "-");

            Regex rgx = new Regex("[^a-zA-Z0-9 -]");
            s1 = rgx.Replace(s1, "");

            s1 = s1.ToLower().Replace(" ", "-")
                .Replace(".", "")
                .Replace("?", "")
                .Replace("'", "")
                .Replace("\"", "")
                .Replace("&", "")
                .Replace("*", "")
                .Replace(":", "-")
                .Replace("%", "")
                .Replace(",", "-")
                .Replace("ª", "");

            while (s1.IndexOf("--") > 0)
            {
                s1 = s1.Replace("--", "-");
            }

            return s1;
        }

        private void LoadKindsCache()
        {
            this.kindsCache = GetContentKinds();
        }

        public ContentKind[] GetContentKinds()
        {
            var kinds = new ContentKind[0];
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(ContentKind[]));
            var path = System.IO.Path.Combine(AppPath, "App_Data/CMS/kinds.config");
            using (var reader = System.Xml.XmlReader.Create(path))
            {
                kinds = serializer.Deserialize(reader) as ContentKind[];
            }
            if (kinds == null)
                kinds = new ContentKind[0];

            foreach (ContentKind k in kinds)
            {
                if (k.Browsable == null)
                    k.Browsable = true;                
                if (k.FriendlyPluralName == null)
                    k.FriendlyPluralName = k.FriendlyName + "s";                
                if (k.RequiredRolesToEdit == null || k.RequiredRolesToEdit.Length == 0)
                    k.RequiredRolesToEdit = new string[] { "CONTENT.ADM" };
                if (k.Tags == null)
                    k.Tags = new string[0];
            }
            return kinds;
        }

        public ContentKind[] ContentKinds
        {
            get
            {
                if (kindsCache == null)
                    LoadKindsCache();
                return kindsCache;
            }
        }

        public ContentKind GetContentKind(string kind, bool withAllLocations = false)
        {
            if (kindsCache == null)
                LoadKindsCache();

            var contentKind = kindsCache.SingleOrDefault(k => k.Kind.ToLower() == kind.ToLower());

            if (contentKind == null)
                return null;

            // adds old locations
            if(withAllLocations) {
                string[] plocations = GetPublishedLocations(kind);
                contentKind.Locations = contentKind.Locations.Union(plocations).ToArray();
            }

            return contentKind;
        }

        public string[] GetPublishedLocations(string kind)
        {
            return _context.ContentHeads.Where(c => c.Kind.ToLower() == kind.ToLower()).Select(c => c.Location).Distinct().ToArray();            
        }

        public ContentHead GetContentHeadByUrlAndKind(string url, string kind, bool onlyPublished)
        {
            url = url.ToLower();            
            IQueryable<ContentHead> content = _context.ContentHeads.Where(c => c.Location + c.CanonicalName == url);
            if (kind != null)
                content = content.Where(c => c.Kind == kind);
            if (onlyPublished)
                content = OnlyPublishedContents(content);
            return content.FirstOrDefault();
        }

        public void ParseContentData(ContentHead content)
        {
            content.CONTENT = JObject.Parse(content.Data.JSON);
        }

        
        public DateTime? GetLastPublishDate(string location, string[] kinds)
        {
            
            IQueryable<ContentHead> contents = _context.ContentHeads;

            contents = OnlyPublishedContents(contents);

            if (!string.IsNullOrEmpty(location))
            {
                location = location.ToLower();
                if (!location.EndsWith("/"))
                    location = location + "/";
                contents = contents.Where(c => c.Location == location);
            }

            if (kinds != null)
                contents = contents.Where(c => kinds.Contains(c.Kind.ToLower()));

            return contents.Max(c => (DateTime?)c.CreateDate);
            
        }

        public void RemoveContent(string contentUId)
        {         
            ContentHead content = _context.ContentHeads.SingleOrDefault(c => c.ContentUId == contentUId);
            if (content == null)
                return;
            _context.ContentHeads.Remove(content);
            _context.SaveChanges();         
        }



    }



}
