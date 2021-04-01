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

        public static ModoAlerta[] MODO_ALERTA = new ModoAlerta[]
        {
            new ModoAlerta
            {
                BAlertaMax = false,
                ModoFechado = false,
                ModoAberto = false,
                DisplayModo = "Automático",
                DisplayEstado = "Nomal",
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
                DisplayEstado = "Nomal",
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
                DisplayEstado = "Nomal",
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
    }
}