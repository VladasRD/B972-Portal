using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGeoIot.Models
{
    public class B975
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string DeviceId { get; set; }
        public DateTime Date { get; set; }


        // Pacote - 1A P975U4
        [MaxLength(100)]
        public string PackA { get; set; }
        public long TimeA { get; set; }
        public bool PcPosChave { get; set; }
        public bool Jam { get; set; }
        public bool Vio { get; set; }
        public bool RasIn { get; set; }
        public bool Bloqueio { get; set; }
        public bool RasOut { get; set; }
        public string StatusDJ { get; set; }
        public bool AlertaFonteBaixa { get; set; }
        public decimal IntervaloUpLink { get; set; }
        public int ContadorCarencias { get; set; }
        public int ContadorBloqueios { get; set; }


        // Pacote - 1B P975U5
        [MaxLength(100)]
        public string PackB { get; set; }
        public long TimeB { get; set; }
        public decimal TemperaturaInterna { get; set; }
        public decimal TensaoAlimentacao { get; set; }


        // Pacote - 1C P975U6
        [MaxLength(100)]
        public string PackC { get; set; }
        public long TimeC { get; set; }
        public decimal MediaRFMinimo  { get; set; }
        public decimal MediaRFMaximo { get; set; }
        public decimal MediaLinhaBase { get; set; }
        public decimal MediaInterferencia { get; set; }
        public decimal DeteccaoInterferencia { get; set; }
        public decimal DeteccaoJammer { get; set; }
        public decimal NumeroViolacao { get; set; }



        [MaxLength(100)]
        public string Source { get; set; }

        [MaxLength(50)]
        public string Radius { get; set; }

        [MaxLength(100)]
        public double Latitude { get; set; }

        [MaxLength(100)]
        public double Longitude { get; set; }

        [MaxLength(500)]
        public string LocationCity { get; set; }

        public int Lqi { get; set; }

        [NotMapped]
        public DateTime DateGMTBrasilian
        {
            get
            {
                return this.Date;
            }
        }
    }
}