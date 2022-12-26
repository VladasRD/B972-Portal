using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SmartGeoIot.Extensions
{
    public class Consts
    {
        public class ModoAlerta
        {
            public bool BAlertaMax { get; set; }
            public bool ModoFechado { get; set; }
            public bool ModoAberto { get; set; }
            public string DisplayModo { get; set; }
            public string DisplayEstado { get; set; }
            public string DisplayValvula { get; set; }
            public string EstadoImage { get; set; }
            public string ModoImage { get; set; }
            public string EstadoColor { get; set; }
        }

        public class DiametroCalha
        {
            public int Calha { get; set; } // 1-Palmer-Bowlus D, 2-Palmer-Bowlus D/2, 3-Parshall
            public string Diametro { get; set; }
            public bool B0 { get; set; }
            public bool B1 { get; set; }
            public bool B2 { get; set; }
            public bool B3 { get; set; }
        }

        public class AlertaCalha
        {
            public string Alerta { get; set; }
            public bool B4 { get; set; }
            public bool B5 { get; set; }
            public bool B6 { get; set; }
            public bool B7 { get; set; }
        }

        public static AlertaCalha[] ALERTA_CALHA = new AlertaCalha[]
        {
            new AlertaCalha
            {
                Alerta = "Normal",
                B4 = false,
                B5 = false,
                B6 = false,
                B7 = false
            },
            new AlertaCalha
            {
                Alerta = "Vazão acima do crítico máximo",
                B4 = false,
                B5 = false,
                B6 = true,
                B7 = true
            },
            new AlertaCalha
            {
                Alerta = "Vazão acima do máximo",
                B4 = false,
                B5 = false,
                B6 = true,
                B7 = false
            },
            new AlertaCalha
            {
                Alerta = "Vazão abaixo do mínimo",
                B4 = false,
                B5 = true,
                B6 = false,
                B7 = false
            },
            new AlertaCalha
            {
                Alerta = "Vazão abaixo do crítico mínimo",
                B4 = true,
                B5 = true,
                B6 = false,
                B7 = false
            }
        };

        public static DiametroCalha[] DIAMETRO_CALHA = new DiametroCalha[]
        {
            new DiametroCalha
            {
                Calha = 1,
                Diametro = "4",
                B0 = true,
                B1 = false,
                B2 = false,
                B3 = false
            },
            new DiametroCalha
            {
                Calha = 1,
                Diametro = "6",
                B0 = false,
                B1 = true,
                B2 = false,
                B3 = false
            },
            new DiametroCalha
            {
                Calha = 1,
                Diametro = "8",
                B0 = true,
                B1 = true,
                B2 = false,
                B3 = false
            },
            new DiametroCalha
            {
                Calha = 1,
                Diametro = "10",
                B0 = false,
                B1 = false,
                B2 = true,
                B3 = false
            },
            new DiametroCalha
            {
                Calha = 1,
                Diametro = "12",
                B0 = true,
                B1 = false,
                B2 = true,
                B3 = false
            },
            new DiametroCalha
            {
                Calha = 1,
                Diametro = "15",
                B0 = false,
                B1 = true,
                B2 = true,
                B3 = false
            },
            new DiametroCalha
            {
                Calha = 1,
                Diametro = "18",
                B0 = true,
                B1 = true,
                B2 = true,
                B3 = false
            },
            new DiametroCalha
            {
                Calha = 1,
                Diametro = "21",
                B0 = false,
                B1 = false,
                B2 = false,
                B3 = true
            },
            new DiametroCalha
            {
                Calha = 1,
                Diametro = "24",
                B0 = true,
                B1 = false,
                B2 = false,
                B3 = true
            },
            new DiametroCalha
            {
                Calha = 3,
                Diametro = "1",
                B0 = true,
                B1 = false,
                B2 = false,
                B3 = false
            },
            new DiametroCalha
            {
                Calha = 3,
                Diametro = "2",
                B0 = false,
                B1 = true,
                B2 = false,
                B3 = false
            },
            new DiametroCalha
            {
                Calha = 3,
                Diametro = "3",
                B0 = true,
                B1 = true,
                B2 = false,
                B3 = false
            }
        };

        public static ModoAlerta[] MODO_ALERTA = new ModoAlerta[]
        {
            new ModoAlerta
            {
                BAlertaMax = false,
                ModoFechado = false,
                ModoAberto = false,
                DisplayModo = "Automático",
                DisplayEstado = "Normal",
                DisplayValvula = "Aberta",
                EstadoImage = "valvula_aberta",
                ModoImage = "modo_automatico",
                EstadoColor = "estado-green"
            },
            new ModoAlerta
            {
                BAlertaMax = true,
                ModoFechado = false,
                ModoAberto = false,
                DisplayModo = "Automático",
                DisplayEstado = "Alerta",
                DisplayValvula = "Fechada",
                EstadoImage = "valvula_fechada",
                ModoImage = "modo_automatico",
                EstadoColor = "estado-red"
            },
            new ModoAlerta
            {
                BAlertaMax = false,
                ModoFechado = true,
                ModoAberto = false,
                DisplayModo = "Fechado",
                DisplayEstado = "Normal",
                DisplayValvula = "Fechada",
                EstadoImage = "valvula_fechada",
                ModoImage = "modo_fechado",
                EstadoColor = "estado-green"
            },
            new ModoAlerta
            {
                BAlertaMax = true,
                ModoFechado = true,
                ModoAberto = false,
                DisplayModo = "Fechado",
                DisplayEstado = "Alerta",
                DisplayValvula = "Fechada",
                EstadoImage = "valvula_fechada",
                ModoImage = "modo_fechado",
                EstadoColor = "estado-red"
            },
            new ModoAlerta
            {
                BAlertaMax = false,
                ModoFechado = false,
                ModoAberto = true,
                DisplayModo = "Aberto",
                DisplayEstado = "Normal",
                DisplayValvula = "Aberta",
                EstadoImage = "valvula_aberta",
                ModoImage = "modo_aberto",
                EstadoColor = "estado-green"
            },
            new ModoAlerta
            {
                BAlertaMax = true,
                ModoFechado = false,
                ModoAberto = true,
                DisplayModo = "Aberto",
                DisplayEstado = "Alerta",
                DisplayValvula = "Aberta",
                EstadoImage = "valvula_aberta",
                ModoImage = "modo_aberto",
                EstadoColor = "estado-red"
            },
            new ModoAlerta
            {
                BAlertaMax = false,
                ModoFechado = true,
                ModoAberto = true,
                DisplayModo = "Inválido",
                DisplayEstado = "Inválido",
                DisplayValvula = "Inválido",
                EstadoImage = "valvula_fechada",
                ModoImage = "modo_fechado",
                EstadoColor = "estado-red"
            }
        };

        public static ModoAlerta GetDisplayTRM10(bool b0, bool b1, bool b2)
        {
            try
            {
                return MODO_ALERTA.SingleOrDefault(s => s.BAlertaMax == b0 && s.ModoFechado == b1 && s.ModoAberto == b2);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static DiametroCalha GetDiametroCalha(int calha, bool b0, bool b1, bool b2, bool b3)
        {
            try
            {
                return DIAMETRO_CALHA.SingleOrDefault(s => s.B0 == b0 && s.B1 == b1 && s.B2 == b2 && s.B3 == b3 && s.Calha == calha);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static AlertaCalha GetAlertaCalha(bool b4, bool b5, bool b6, bool b7)
        {
            try
            {
                return ALERTA_CALHA.SingleOrDefault(s => s.B4 == b4 && s.B5 == b5 && s.B6 == b6 && s.B7 == b7);
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static string GetFirmwareName(string codeName)
        {
            try
            {
                return FIRMWARE_DEVICE_NAME.SingleOrDefault(s => s.Key.Equals(codeName)).Value;
            }
            catch (System.Exception)
            {
                return null;
            }
        }

        public static Dictionary<string, string> FIRMWARE_DEVICE_NAME = new Dictionary<string, string>
        {
            {
                "0001",
                "G965"
            },
            {
                "0002",
                "SensNivel"
            },
            {
                "0003",
                "B963_BP01"
            },
            {
                "0004",
                "B965"
            },
            {
                "0005",
                "TermTA"
            },
            {
                "0006",
                "B963_TK42"
            },
            {
                "0007",
                "B957_DTV"
            },
            {
                "0008",
                "B950_laco"
            },
            {
                "0009",
                "B968_NB"
            },
            {
                "000A",
                "B969_TA"
            },
            {
                "000B",
                "G93A"
            },
            {
                "000C",
                "G100"
            },
            {
                "000D",
                "TRM10"
            },
            {
                "000E",
                "nanoRD"
            },
            {
                "000F",
                "TRM_Hidro"
            },
            {
                "0010",
                "DJS6"
            },
            {
                "0011",
                "TRM_HidroB"
            },
            {
                "0012",
                "MDM10"
            },
            {
                "0013",
                "ColorV3"
            },
            {
                "0014",
                "CTRV1"
            },
            {
                "0015",
                "CTRU1"
            },
            {
                "0016",
                "TRM_HidroL"
            },
            {
                "0017",
                "AguaMon"
            },
            {
                "0018",
                "DJRF"
            },
            {
                "0019",
                "F107"
            },
            {
                "001A",
                "F977"
            },
            {
                "001B",
                "CamTerm"
            },
            {
                "001C",
                "F100"
            },
            {
                "001D",
                "F104"
            },
            {
                "001E",
                "F105"
            },
            {
                "001F",
                "TQA"
            },
            {
                "0020",
                "SP300"
            },
            {
                "0021",
                "F414"
            },
            {
                "0022",
                "pT104"
            },
            {
                "0023",
                "pT110"
            },
            {
                "0024",
                "T97"
            },
            {
                "0025",
                "GSM1"
            },
            {
                "0026",
                "PrimeL"
            },
            {
                "0027",
                "B984"
            },
            {
                "0028",
                "TRM11"
            },
            {
                "0029",
                "ClpOn"
            },
            {
                "002A",
                "AquaS"
            },
            {
                "002B",
                "INVC104"
            },
            {
                "002C",
                "F98"
            },
            {
                "002D",
                "F109"
            },
            {
                "002E",
                "B988A"
            }
        };


    }
}