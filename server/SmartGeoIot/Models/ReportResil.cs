using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGeoIot.Models
{
    public enum ReportResilType
    {
        Analitico,
        Hora,
        Dia,
        Semana,
        Mes
    }
    public class ReportResil
    {
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public long Time { get; set; }
        public int Day { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int Hour { get; set; }
        public int Minute { get; set; }
        public decimal? ConsumoHora { get; set; }
        public decimal? ConsumoDia { get; set; }
        public decimal? ConsumoSemana { get; set; }
        public decimal? ConsumoMes { get; set; }
        public decimal? Fluxo { get; set; }
        public string Modo { get; set; }
        public string Estado { get; set; }
        public string Valvula { get; set; }
        public DateTime Date { get; set; }
        public bool FAtualizaHora { get; set; }
        public bool FAtualizaDia { get; set; }
        public bool FAtualizaSem { get; set; }
        public bool FAtualizaMes { get; set; }
    }
}