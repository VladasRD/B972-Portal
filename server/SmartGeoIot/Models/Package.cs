using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Models
{
    public enum PackagesEnum
    {
        Hidroponia = 22,
        Aguamon = 21,
        Aguamon2 = 81,
        DJRF = 12,
        TRM = 23,
        TSP = 83,
        // P982U2 = 84,
        B972_83 = 83,
        B972_84 = 84,
        B972_85 = 85,
        B972_86 = 86,
        B972_87 = 87,
        TQA = 81,
        TQA_S = 82,
        B978 = 21,
        B987 = 21,
        B987_P = 21
    }

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