using System;
using System.ComponentModel.DataAnnotations.Schema;
using SmartGeoIot.Models;

namespace SmartGeoIot.ViewModels
{
    public class DashboardViewModels
    {
        public string DeviceId { get; set; }
        public string Name { get; set; }
        public string Package { get; set; }
        public string TypePackage { get; set; }
        public DateTime Date { get; set; }
        public string Country { get; set; }
        public int Lqi { get; set; }
        public Bits Bits { get; set; }
        public int SeqNumber { get; set; }
        public string Level { get; set; }
        public string Light { get; set; }
        public string Temperature { get; set; }
        public string Moisture { get; set; }
        public string OxigenioDissolvido { get; set; }
        public string Ph { get; set; }
        public string Condutividade { get; set; }
        public string PeriodoTransmissao { get; set; } // tempo de transmissão
        public int BaseT { get; set; }
        public string Alimentacao { get; set; }
        public string AlimentacaoMinima { get; set; }
        public string Envio { get; set; }
        public int EstadoDetector { get; set; }
        public string ContadorCarencias { get; set; }
        public string ContadorBloqueios { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public string Radius { get; set; }
        public string LatitudeConverted { get; set; }
        public string LongitudeConverted { get; set; }
        public string RadiusConverted { get; set; }
        public bool AlertaFonteBaixa { get; set; }
        public string TensaoMinima { get; set; }
        public string Fluor { get; set; }
        public string Cloro { get; set; }
        public string Turbidez { get; set; }
        public Rele Rele { get; set; }
        public ReleBoolean ReleBoolean { get; set; }
        public string Vazao { get; set; }
        public string TotalizacaoParcial { get; set; }
        public string Totalizacao { get; set; }
        public string TempoParcial { get; set; }
        public string EntradaAnalogica { get; set; }
        public string SaidaAnalogica { get; set; }
        public string FluxoAgua { get; set; }
        public string ConsumoAgua { get; set; }
        public string Modo { get; set; }
        public string Estado { get; set; }
        public string Valvula { get; set; }
        public string EstadoImage { get; set; }
        public string EstadoColor { get; set; }
        public string ModoImage { get; set; }

        [NotMapped]
        public long SeqNumberb { get; set; }

        public DownloadLink DownloadLink { get; set; }
        public string Calha { get; set; }
        public string CalhaImage { get; set; }
        public string CalhaAlerta { get; set; }
        public string ConsumoDia { get; set; }
        public string ConsumoSemana { get; set; }
        public string ConsumoMes { get; set; }
        public long Time { get; set; }

        [NotMapped]
        public string DateWeekName { get; set; }

        public string Flow { get; set; }
        public string Velocity { get; set; }
        public string Total { get; set; }
        public string Partial { get; set; }
        public string Flags { get; set; }
        public string Quality { get; set; }
        public string SerialNumber { get; set; }
        public string Model { get; set; }
        public string Notes { get; set; }
        public string NotesCreateDate { get; set; }
        public string Ed1 { get; set; }
        public string Ed2 { get; set; }
        public string Ed3 { get; set; }
        public string Ed4 { get; set; }
        public string Sd1 { get; set; }
        public string Sd2 { get; set; }
        public string Ea10 { get; set; }
        public string Sa3 { get; set; }
        public MCond MCond { get; set; }
    }

    public class Rele
    {
        public string Rele1 { get; set; }
        public string Rele2 { get; set; }
        public string Rele3 { get; set; }
        public string Rele4 { get; set; }
        public string Rele5 { get; set; }
    }

    public class ReleBoolean
    {
        public bool Rele1 { get; set; }
        public bool Rele2 { get; set; }
        public bool Rele3 { get; set; }
        public bool Rele4 { get; set; }
        public bool Rele5 { get; set; }
        public bool Rele6 { get; set; }
        public bool Rele7 { get; set; }
    }

    public class Bits
    {
        public bool Iluminacao { get; set; }
        public bool BombaCirculacao { get; set; }
        public bool FalhaEnergia { get; set; }
        public bool Automatico { get; set; }
        public bool SensorNivelOperacional { get; set; }
        public bool AlertaNivelMinimo { get; set; }
        public bool AlertaNivelMaximo { get; set; }
        public bool BombaOxigenacao { get; set; }
        public bool TipoEnvio { get; set; } //ModoUpLink
        public bool BaseTempoUpLink { get; set; }
        public bool AlertaSensor1 { get; set; }
        public bool AlertaSensor2 { get; set; }
        public bool EstadoAlimentacao { get; set; }
        public bool Downlink { get; set; }
        public bool EstadoPosChave { get; set; }
        public bool EstadoViolacaoPainel { get; set; }
        public bool EstadoViolacaoFalso { get; set; }
        public bool EstadoEntradaRastreador { get; set; }
        public bool EstadoBloqueio { get; set; }
        public bool EstadoSaidaRastreador { get; set; }


        // Projeto TRM-10, bits de estado 1
        public bool Bed1 { get; set; }
        public bool Bed2 { get; set; }
        public bool Bed3 { get; set; }
        public bool Bed4 { get; set; }
        public bool Bsd1 { get; set; }
        public bool Bsd2 { get; set; }


        // Projeto TRM-10, bits de estado 2
        public bool Btxev { get; set; }
        public bool BAlertaMin { get; set; }
        public bool BAlertaMax { get; set; }
        public bool ModoFechado { get; set; }
        public bool ModoAberto { get; set; }
        public bool SincHora { get; set; }
        public bool FAtualizaHora { get; set; }
        public bool FAtualizaDia { get; set; }
        public bool FAtualizaSem { get; set; }
        public bool FAtualizaMes { get; set; }

        // Clamp-ON
        public bool Vazao { get; set; }
        public bool Totalizador { get; set; }
        public bool Qualidade { get; set; }
        public bool AlertaVazao { get; set; }
        public bool AlertaTotalizador { get; set; }
        public bool AlertaQualidade { get; set; }
    }

    public enum EstatusDetector
    {
        Aguardando,
        Operacional,
        EmCarencia,
        EmCiclos,
        EmBloqueio,
        EmDormencia
    }

    public class DownloadLink
    {
        public int NumeroEnvios { get; set; }
        public int TempoTransmissao { get; set; }
        public bool TipoEnvio { get; set; }
        public string TensaoMinima { get; set; }
    }
}