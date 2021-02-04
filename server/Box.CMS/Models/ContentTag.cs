using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Box.CMS.Models
{
    public class ContentTag {

        [Column(TypeName="char(36)"), MaxLength(36)]
        public string ContentUId { get; set; }

        [MaxLength(100)]
        public string Tag { get; set; }

        public override bool Equals(object obj) {
            ContentTag tag = obj as ContentTag;
            if(tag==null)
                return false;
            return (ContentUId == tag.ContentUId && Tag == tag.Tag);
        }

        public override int GetHashCode() {
            return (ContentUId + Tag).GetHashCode();
        }
    }

    public class ContentTagRank {

        public string Tag { get; set; }
        public int Rank { get; set; }
    }

}
