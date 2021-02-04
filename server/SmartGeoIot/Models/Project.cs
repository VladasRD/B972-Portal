using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGeoIot.Models
{
    public class Project
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string ProjectUId { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(400)]
        public string Description { get; set; }
    }
}