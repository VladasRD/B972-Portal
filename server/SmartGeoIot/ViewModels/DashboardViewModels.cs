using System;

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
        public string Vazao { get; set; }
        public string TotalizacaoParcial { get; set; }
        public string Totalizacao { get; set; }
        public string TempoParcial { get; set; }
    }

    public class Rele
    {
        public string Rele1 { get; set; }
        public string Rele2 { get; set; }
        public string Rele3 { get; set; }
        public string Rele4 { get; set; }
        public string Rele5 { get; set; }
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
}