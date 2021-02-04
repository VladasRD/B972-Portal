using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Box.CMS.Models
{

    public class CrossLink {

        [Column(TypeName="char(36)"), MaxLength(36)]
        public string ContentUId { get; set; }

        [MaxLength(50)]
        public string PageArea { get; set; }

        public short DisplayOrder { get; set; }

    }

    public class CrossLinkArea {
        public string Area { get; set; }
        public string Description { get; set; }
        public int MaxLinks { get; set; }
    }

}
