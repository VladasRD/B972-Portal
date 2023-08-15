using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Box.Security.Models;

namespace SmartGeoIot.Models
{
    public class ServiceDesk
    {
        [Key]
        public int ServiceDeskId { get; set; }

        [MaxLength(20)]
        public string DeviceId { get; set; }
        public DateTime CreateDate { get; set; }
        public DateTime? FinishDate { get; set; }

        [ForeignKey("ServiceDeskId")]
        public ICollection<ServiceDeskRecord> Records { get; set; }

        [NotMapped]
        public string Status
        {
            get
            {
                return this.FinishDate.HasValue ? "Finalizado" : "Aberto";
            }
        }

        [NotMapped]
        public bool IsOpened
        {
            get
            {
                return !this.FinishDate.HasValue;
            }
        }

        [NotMapped]
        public DateTime CreateDateBrasilian
        {
            get
            {
                return this.CreateDate.AddHours(-3);
            }
        }

        [NotMapped]
        public DateTime? FinishDateBrasilian
        {
            get
            {
                return this.FinishDate.HasValue ? this.FinishDate.Value.AddHours(-3) : (DateTime?) null;
            }
        }
    }

    public class ServiceDeskRecord
    {
        [Key]
        public int ServiceDeskRecordId { get; set; }
        public int ServiceDeskId { get; set; }

        [MaxLength(100)]
        public string Package { get; set; }
        public long? PackageTimestamp { get; set; }
        public string Description { get; set; }
        public DateTime CreateDate { get; set; }

        [NotMapped]
        public DateTime CreateDateBrasilian
        {
            get
            {
                return this.CreateDate.AddHours(-3);
            }
        }
    }
}