using System;
using System.ComponentModel.DataAnnotations;

namespace SmartGeoIot.Models
{
    public class ResetTotalPartial
    {
        [Key]
        [MaxLength(50)]
        public string DeviceId { get; set; }
        public decimal LastValue { get; set; }
        public DateTime Date { get; set; }

        [MaxLength(100)]
        public string EmailUser { get; set; }
    }
}