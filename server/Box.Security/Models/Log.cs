using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Box.Security.Models
{
    public class Log
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public Guid LogUId { get; set; }

        public DateTime When { get; set; }

        [MaxLength(255)]
        public string SignedUser { get; set; }

        public string ActionDescription { get; set; }

        public string Url { get; set; }

        [MaxLength(20)]
        public string UserIp { get; set; }

        public string ErrorDescription { get; set; }

        public string Parameters { get; set; }

        public short LogType { get; set; }
    }
}
