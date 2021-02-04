using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Models
{
    public class Package
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string PackageUId { get; set; }

        [MaxLength(50)]
        public string Type { get; set; }
        public int Byte { get; set; }
        public int Bit { get; set; }

        [MaxLength(100)]
        public string Name { get; set; }

        [MaxLength(200)]
        public string Description { get; set; }
    }
}