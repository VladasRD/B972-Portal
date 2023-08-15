using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SmartGeoIot.Models
{
    public enum enumState
    {
        [EnumMember(Value = "Portão fechado")]
        Fechado,

        [EnumMember(Value = "Portão aberto")]
        Aberto,

        [EnumMember(Value = "Portão parado")]
        Parado,

        [EnumMember(Value = "Portão abrindo")]
        Abrindo,

        [EnumMember(Value = "Portão parando abrindo")]
        Parando_aberto,

        [EnumMember(Value = "Portão fechando")]
        Fechando,

        [EnumMember(Value = "Portão parando fechando")]
        Parando_fechado,

        [EnumMember(Value = "Portão em posição incerta")]
        Incerto
    }

    public class B979
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string DeviceId { get; set; }
        public DateTime Data { get; set; }
        public decimal? Acel { get; set; }
        public decimal? Desacel { get; set; }
        public decimal? EncoderPMA { get; set; }
        public decimal? EncoderPMF { get; set; }
        public decimal? TimerFreioOn { get; set; }
        public decimal? TimerFreioOff { get; set; }
        public decimal? Timer { get; set; }
        public decimal? TimerP2 { get; set; }
        public decimal? TOVelBaixa { get; set; }
        public decimal? TempoPMA { get; set; }
        public decimal? TempoPMF { get; set; }
        public decimal? VelBaixa { get; set; }
        public decimal? VelAltaAbrir { get; set; }
        public decimal? VelAltaFechar { get; set; }
        public decimal? Ciclos { get; set; }
        public decimal? Horimetro { get; set; }
        public decimal? Inversor { get; set; }
        public enumState? Estado { get; set; }

        [NotMapped]
        public DateTime DateGMTBrasilian
        {
            get
            {
                return this.Data.AddHours(-3);
            }
        }
    }

    public class B979ViewModel
    {
        [MaxLength(20)]
        public string DeviceId { get; set; }
        public DateTime Data { get; set; }
        public decimal? Acel { get; set; }
        public decimal? Desacel { get; set; }
        public decimal? EncoderPMA { get; set; }
        public decimal? EncoderPMF { get; set; }
        public decimal? TimerFreioOn { get; set; }
        public decimal? TimerFreioOff { get; set; }
        public decimal? Timer { get; set; }
        public decimal? TimerP2 { get; set; }
        public decimal? TOVelBaixa { get; set; }
        public decimal? TempoPMA { get; set; }
        public decimal? TempoPMF { get; set; }
        public decimal? VelBaixa { get; set; }
        public decimal? VelAltaAbrir { get; set; }
        public decimal? VelAltaFechar { get; set; }
        public decimal? Ciclos { get; set; }
        public decimal? Horimetro { get; set; }
        public decimal? Inversor { get; set; }
        public string Estado { get; set; }
    }

    public class B979RequestToDevice
    {
        [Key]
        public int Id { get; set; }
        
        [MaxLength(20)]
        public string DeviceId { get; set; }
        public decimal? Acel { get; set; }
        public decimal? Desacel { get; set; }
        public decimal? EncoderPMA { get; set; }
        public decimal? EncoderPMF { get; set; }
        public decimal? TimerFreioOn { get; set; }
        public decimal? TimerFreioOff { get; set; }
        public decimal? Timer { get; set; }
        public decimal? TimerP2 { get; set; }
        public decimal? TOVelBaixa { get; set; }
        public decimal? TempoPMA { get; set; }
        public decimal? TempoPMF { get; set; }
        public decimal? VelBaixa { get; set; }
        public decimal? VelAltaAbrir { get; set; }
        public decimal? VelAltaFechar { get; set; }
        public decimal? Inversor { get; set; }
    }

}