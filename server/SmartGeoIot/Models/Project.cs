using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SmartGeoIot.Models
{
    public enum ProjectCode
    {
        [EnumMember(Value = "B982-C")]
        B972,
        
        [EnumMember(Value = "B982-S")]
        B982_S,
        
        [EnumMember(Value = "Hidroleg")]
        Hidroleg,
        
        [EnumMember(Value = "DJRFleg")]
        DJRFleg,
        
        [EnumMember(Value = "AM-1leg")]
        AM_1leg,
        
        [EnumMember(Value = "FluxoRDleg")]
        FluxoRDleg,
        
        [EnumMember(Value = "B972-P")]
        B972_P,
        
        [EnumMember(Value = "B981")]
        B981,

        [EnumMember(Value = "B978")]
        B978,

        [EnumMember(Value = "B987")]
        B987
    }

    public class Project
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string ProjectUId { get; set; }

        [MaxLength(200)]
        public string Name { get; set; }

        [MaxLength(400)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }
    }

    public class ProjectDevice
    {
        [Key]
        public Guid Id { get; set; }

        [MaxLength(50)]
        public string Code { get; set; }
        public string DeviceId { get; set; }
    }
}