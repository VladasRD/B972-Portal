using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Box.CMS.Models {

    public enum ContentRanks {
        PageViews,
        Shares,
        Comments,
        Date
    }

    public enum Periods {
        AnyTime,
        LastHour,
        LastDay,
        Last5Days,
        Last30Days,
        Last150Days,
        Last360Days
    }

    public enum Orders {
        Name,
        Date,
        DateASC,
        DisplayOrder,
        Comments,
        CommentsDESC,
        Share,
        ShareDESC,
        PageView,
        PageViewDESC,
        Random,
        CrossLinkDisplayOrder,
        RandomOnDay,
        CustomNumber1,
        CustomNumber1DESC,
        CustomNumber2,
        CustomNumber2DESC,
        CustomNumber3,
        CustomNumber3DESC,
        CustomNumber4,
        CustomNumber4DESC,
        CustomText1,
        CustomText1DESC,
        CustomText2,
        CustomText2DESC,
        CustomText3,
        CustomText3DESC,
        CustomText4,
        CustomText4DESC,
        CustomDate1,
        CustomDate1DESC,
        CustomDate2,
        CustomDate2DESC,
        CustomDate3,
        CustomDate3DESC,
        CustomDate4,
        CustomDate4DESC
    }

    public class ContentHead {

        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string ContentUId { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(250)]
        public string CanonicalName { get; set; }       

        [MaxLength(25)]
        public string Kind { get; set; }

        [MaxLength(300)]
        public string Abstract { get; set; }

        [MaxLength(87)]
        public string ThumbFilePath { get; set; }

        [MaxLength(500)]
        public string Location { get; set; }

        public ContentData Data { get; set; }


        public DateTime CreateDate { get; set; }
        public DateTime ContentDate { get; set; }

        public short DisplayOrder { get; set; }

        public DateTime? PublishUntil { get; set; }
        public DateTime? PublishAfter { get; set; }

        public int Version { get; set; }

        public ICollection<ContentTag> Tags { get; set; }

        public ICollection<CrossLink> CrossLinks { get; set; }

        public ContentCommentCount CommentsCount { get; set; }

        public ContentShareCount ShareCount { get; set; }

        public ContentPageViewCount PageViewCount { get; set; }

        public ContentCustomInfo CustomInfo { get; set; }

        [MaxLength(500)]
        public string ExternalLinkUrl { get; set; }

        [NotMapped]
        // [Newtonsoft.Json.JsonIgnore]
        public dynamic CONTENT { get; set; }

        public string[] TagsToArray() {
            if(Tags==null)
                return new string[0];
            return Tags.Select(t => t.Tag).ToArray<string>();
        }

        [NotMapped]
        // [Newtonsoft.Json.JsonIgnore]
        public int OrderIndex { get; set; }
    }

    public class ContentData {
        
        [Key, MaxLength(36)]
        public string ContentUId { get; set; }

        public string JSON { get; set; }
    }

}
