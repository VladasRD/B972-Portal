using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGeoIot.Models
{
    public enum B972_IQ_Enum
    {
        Complete,
        Partial
    }

    public class B972
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string DeviceId { get; set; }
        public long Time { get; set; }// pack 84
        public int Position { get; set; }
        public decimal? Flow { get; set; } // vazão, pack 84

        public long? VelocityTime { get; set; } // pack 85
        public long? Pack86Time { get; set; } // pack 85
        public long? Pack87Time { get; set; } // pack 85
        public decimal? Velocity { get; set; } // velocidade, pack 85


        // pacote 86, ver se vai deixar em uma tabela relacional
        public decimal? Total { get; set; }
        public decimal? Partial { get; set; } // parcial
        public decimal? Temperature { get; set; }
        // pacote 86


        // pacote 87, ver se vai deixar em uma tabela relacional
        [MaxLength(8)]
        public string Flags { get; set; } // flags, estados do alerta, 0/1
        public decimal? Quality { get; set; } // qualidade da informação do pacote
        // pacote 87



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