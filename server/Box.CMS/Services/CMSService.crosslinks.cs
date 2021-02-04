using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Box.CMS.Models;
using Microsoft.EntityFrameworkCore;

namespace Box.CMS.Services {

    public partial class CMSService {

        public CrossLinkArea[] GetCrossLinkAreas()
        {
            var links = new CrossLinkArea[0];
            System.Xml.Serialization.XmlSerializer serializer = new System.Xml.Serialization.XmlSerializer(typeof(CrossLinkArea[]));
            var path = System.IO.Path.Combine(AppPath, "App_Data/CMS/crosslinks.config");
            using (var reader = System.Xml.XmlReader.Create(path))
            {
                links = serializer.Deserialize(reader) as CrossLinkArea[];
            }
            if (links == null)
                links = new CrossLinkArea[0];
            return links;
        }

        public void RemoveCrossLink(CrossLink link) {            
            // mudei um pouco aqui em relação ao BOX (.net), pois dava erro de tracking ao remover
            _context.CrossLinks.Remove(link);
            _context.SaveChanges();

            // update orders
            _context.Database.ExecuteSqlCommand(
                "UPDATE CrossLinks SET DisplayOrder = DisplayOrder - 1 WHERE PageArea = {0} AND DisplayOrder > {1}", link.PageArea, link.DisplayOrder);            
        }

        public void RemoveCrossLink(string area, string id) {            
            var link = _context.CrossLinks.SingleOrDefault(c => c.ContentUId == id && c.PageArea == area);
            if(link!=null)
                RemoveCrossLink(link);
        }

        /// <summary>
        /// Adds a news crosslink for the contect at the given area.
        /// </summary>
        /// <param name="contentUId">The content Id</param>
        /// <param name="area">The crosslink area</param>
        /// <param name="changeDisplayOrderBy">Used to change the crosslink display order</param>
        /// <returns></returns>
        public void AddCrossLink(string contentUId, string area, short changeDisplayOrderBy = 0) {

            short oldOrder = 0;

            // max crosslink order
            short maxOrder = -1;
            if (_context.CrossLinks.Any(c => c.PageArea == area)) {
                maxOrder = _context.CrossLinks.Where(c => c.PageArea == area).Select(c => c.DisplayOrder).DefaultIfEmpty().Max();
            }

            CrossLink link = _context.CrossLinks.SingleOrDefault(c => c.ContentUId == contentUId && c.PageArea == area);

            if (link==null) {
                link = new CrossLink() { ContentUId = contentUId, PageArea = area, DisplayOrder = (short)(maxOrder + 1) };
                _context.CrossLinks.Add(link);
            }
            else {
                _context.CrossLinks.Update(link);
            }

            // calcs the new crosslink order
            oldOrder = link.DisplayOrder;
            short order = (short)(link.DisplayOrder + changeDisplayOrderBy);

            // if is a order chage and it its ut of bounds, get out of here
            if (changeDisplayOrderBy < 0 && oldOrder == 0)
                return;
            if (changeDisplayOrderBy > 0 && oldOrder == maxOrder)
                return;

            // set the new order
            link.DisplayOrder = order;
            

            // change the other link display order
            CrossLink link2 = null;
            link2 = _context.CrossLinks.SingleOrDefault(c => c.ContentUId != contentUId && c.PageArea == area && c.DisplayOrder == order);
            if (link2!=null) {
                link2.DisplayOrder = oldOrder;
                _context.CrossLinks.Update(link2);
            }

            
            _context.SaveChanges();

            _log.Log($"Crosslink '{area}/{contentUId}' has it order changed.");
   
        }


        internal void ApplyCollectionValuesCrossLinks(ICollection<CrossLink> oldCollection, ICollection<CrossLink> newCollection) {
            if (oldCollection == null)
                oldCollection = new List<CrossLink>();
            if (newCollection == null)
                newCollection = new List<CrossLink>();
            var removed = oldCollection.Where(o => !newCollection.Any(n => n.PageArea == o.PageArea)).ToArray();
            var added = newCollection.Where(n => !oldCollection.Any(o => n.PageArea == o.PageArea)).ToArray();

            foreach (var r in removed)
                RemoveCrossLink(r);

            foreach (var a in added)
                AddCrossLink(a.ContentUId, a.PageArea);
        }

        public IEnumerable<ContentHead> GetCrossLinksFrom(string pageArea, Orders order = Orders.CrossLinkDisplayOrder, int top = 0, string[] kinds = null, bool includeData = false, string[] pageAreaFallbacks = null, bool onlyPublished = true)
        {
            
            IQueryable<ContentHead> contents = null;

            if (!includeData)
                contents = _context.ContentHeads.Include("CommentsCount").Include("ShareCount").Include("PageViewCount").Include("Tags").Include("CustomInfo");
            else
                contents = _context.ContentHeads.Include("CommentsCount").Include("ShareCount").Include("PageViewCount").Include("Tags").Include("CustomInfo").Include("Data");

            contents = contents.Where(c => c.CrossLinks.Any(x => x.PageArea == pageArea));

            if (onlyPublished)
                contents = OnlyPublishedContents(contents);

            contents = OrderContents(contents, order, pageArea);

            if (kinds != null)
                contents = contents.Where(c => kinds.Contains(c.Kind.ToLower()));

            if (top != 0)
                contents = contents.Take(top);

            var array = contents.ToArray();

            // if ther is no content, and there is any fallback, try it
            if (!array.Any() && pageAreaFallbacks != null && pageAreaFallbacks.Length >= 1)
            {
                string fallBackArea = pageAreaFallbacks.First();
                string[] othersFall = pageAreaFallbacks.Where(a => a != fallBackArea).ToArray();
                return GetCrossLinksFrom(fallBackArea, order, top, kinds, includeData, othersFall);
            }

            return array;
            
        }


    public IEnumerable<ContentHead> GetRelatedContent(string id, int top, string location, string[] kinds, bool includeData = false) {
        var content = GetContent(id);
        if (content == null)
            return new List<ContentHead>();
        string[] tags = content.Tags.Select(t => t.Tag).ToArray();
        var contents = GetRelatedContent(tags, top, location, kinds, includeData);

        // remove it selft from related contents
        if (contents != null)
            contents = contents.Where(c => c.ContentUId != id);

        if(top==0)
            return contents;

        return contents.Take(top);
    }

        public IEnumerable<ContentHead> GetRelatedContent(string[] tags, int top, string location, string[] kinds, bool includeData = false)
        {
            
            IQueryable<ContentHead> contents = null;

            if (!includeData)
                contents = _context.ContentHeads.Include("CrossLinks").Include("CommentsCount").Include("ShareCount").Include("Tags").Include("PageViewCount").Include("CustomInfo");
            else
                contents = _context.ContentHeads.Include("CrossLinks").Include("CommentsCount").Include("ShareCount").Include("Tags").Include("PageViewCount").Include("CustomInfo").Include("Data");

            // only published
            contents = OnlyPublishedContents(contents);

            // remove empty entries, and go lower
            tags = tags.Where(t => !String.IsNullOrEmpty(t)).ToArray();
            for (int i = 0; i < tags.Length; i++)
                tags[i] = tags[i].ToLower();

            // get related using tags
            contents = contents.Where(c => c.Tags.Any(t => tags.Contains(t.Tag)));

            contents = OrderContents(contents, Orders.Date);

            if (location != null)
            {
                location = location.ToLower();
                if (!location.EndsWith("/"))
                    location = location + "/";
                contents = contents.Where(c => c.Location == location);
            }

            if (kinds != null)
                contents = contents.Where(c => kinds.Contains(c.Kind.ToLower()));

            if (top != 0)
                contents = contents.Take(top);

            return contents.ToArray();
            
        }

        public ContentHead GetHotestContent(string[] kinds, string location, ContentRanks rankBy = ContentRanks.PageViews, DateTime? createdFrom = null, DateTime? createdTo = null)
        {
            Orders order = Orders.PageViewDESC;
            switch (rankBy)
            {
                case ContentRanks.Comments:
                    order =  Orders.CommentsDESC;
                    break;
                case ContentRanks.Shares:
                    order = Orders.ShareDESC;
                    break;
                case ContentRanks.Date:
                    order = Orders.Date;
                    break;
            }
            return GetContents(null, 0, 1, location, kinds, null, order, createdFrom, createdTo, false, true).FirstOrDefault();
        }


    }

    

}
