using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGeoIot.Models
{
    public class B982_S
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string DeviceId { get; set; }
        public long Time { get; set; }// pack 84

        [MaxLength(30)]
        public string OriginPack { get; set; }

        public decimal Flow { get; set; } // vazão, pack 83/84
        public decimal Total { get; set; }
        public decimal Partial { get; set; } // parcial
        
        [MaxLength(100)]
        public string Calha { get; set; }

        [MaxLength(200)]
        public string CalhaAlerta { get; set; }


        [MaxLength(100)]
        public string RSSI { get; set; }

        [MaxLength(100)]
        public string Source { get; set; }
        public int Lqi { get; set; } // qualidade do registro
        public B972_IQ_Enum? Iq { get; set; }
        public DateTime Date { get; set; }

        [NotMapped]
        public DateTime DateGMTBrasilian
        {
            get
            {
                return this.Date.AddHours(-3);
            }
        }
        
    }
}