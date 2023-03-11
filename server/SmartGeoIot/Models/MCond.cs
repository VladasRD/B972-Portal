using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartGeoIot.Extensions;

namespace SmartGeoIot.Models
{
    public enum McondType
    {
        Portaria,
        Superior,
        Inferior
    }

    public class MCond
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(20)]
        public string DeviceId { get; set; }
        public long Time { get; set; }

        [MaxLength(40)]
        public string PackInf { get; set; }
        public bool InfAlarmLevelMin { get; set; }
        public bool InfAlarmLevelMax { get; set; }
        public double InfLevel { get; set; }

        [NotMapped]
        public double InfLevelLitros
        {
            get
            {
                if (this.PackInf == null)
                    return 0;
                    
                // return (this.InfLevel * this.LevelMultiplicationFactor) - this.LevelSubtractionFactor;
                return this.InfLevel;
            }
        }
        

        [MaxLength(40)]
        public string PackSup { get; set; }
        public bool SupStateBomb { get; set; }
        public bool SupAlarmLevelMin { get; set; }
        public bool SupAlarmLevelMax { get; set; }
        public double SupLevel { get; set; }
        
        [NotMapped]
        public double SupLevelLitros
        {
            get
            {
                if (this.PackSup == null)
                    return 0;

                // return (this.SupLevel * this.LevelMultiplicationFactor) - this.LevelSubtractionFactor;
                return this.SupLevel;
            }
        }
        
        
        [MaxLength(40)]
        public string PackPort { get; set; }
        public bool PortFireAlarm { get; set; }
        public bool PortIvaAlarm { get; set; }
        public bool PortFireState { get; set; }
        public bool PortIvaState { get; set; }
        public DateTime Date { get; set; }

        
        [NotMapped]
        public double PackPortSubType
        {
            get
            {
                try
                {    
                    var _value = this.PackPort.Substring(2, this.PackPort.Length - 2).Substring(12, 8);
                    var val = Utils.HexaToLong(_value);
                    return Utils.FromFloatSafe(val);
                }
                catch (System.Exception)
                {
                    return 0;
                }
            }
        }

        
        [NotMapped]
        public double PackSupSubType
        {
            get
            {
                try
                {    
                    var _value = this.PackSup.Substring(2, this.PackSup.Length - 2).Substring(12, 8);
                    var val = Utils.HexaToLong(_value);
                    return Utils.FromFloatSafe(val);
                }
                catch (System.Exception)
                {
                    return 0;
                }
            }
        }

        
        [NotMapped]
        public double PackInfSubType
        {
            get
            {
                try
                {    
                    var _value = this.PackInf.Substring(2, this.PackInf.Length - 2).Substring(12, 8);
                    var val = Utils.HexaToLong(_value);
                    return Utils.FromFloatSafe(val);
                }
                catch (System.Exception)
                {
                    return 0;
                }
            }
        }




        [NotMapped]
        public DateTime DateGMTBrasilian
        {
            get
            {
                return this.Date.AddHours(-3);
            }
        }

        [NotMapped]
        public double LevelMultiplicationFactor
        {
            get
            {
                return 1302.3256;
            }
        }

        [NotMapped]
        public double LevelSubtractionFactor
        {
            get
            {
                return 5209.302;
            }
        }

        [NotMapped]
        public string SupStateBombNameClass
        {
            get
            {
                if (this.PackSup == null)
                    return "";

                
                return (this.SupStateBomb ? "alarme-yellow" : "alarme-gray");
            }
        }

        [NotMapped]
        public string SupStateBombText
        {
            get
            {
                if (this.PackSup == null)
                    return "";

                
                return (this.SupStateBomb ? "Bomba Ligada" : "Bomba Desligada");
            }
        }

        [NotMapped]
        public string SupStateBombTextReport
        {
            get
            {
                return this.SupStateBombText.Replace("Bomba", "");
            }
        }

        [NotMapped]
        public string AlarmeSupText
        {
            get
            {
                if (this.PackSup == null)
                    return string.Empty;

                if (!this.SupAlarmLevelMin && !this.SupAlarmLevelMax)
                {
                    return "Nível Normal";
                }

                if (this.SupAlarmLevelMin && !this.SupAlarmLevelMax)
                {
                    return "Nível Baixo";
                }

                if (!this.SupAlarmLevelMin && this.SupAlarmLevelMax)
                {
                    return "Nível Alto";
                }

                return string.Empty;
            }
        }

        [NotMapped]
        public string AlarmeSupTextReport
        {
            get
            {
                return this.AlarmeSupText.Replace("Nível ", "");
            }
        }

        [NotMapped]
        public string AlarmeSupNameClass
        {
            get
            {
                if (this.PackSup == null)
                    return string.Empty;

                if (!this.SupAlarmLevelMin && !this.SupAlarmLevelMax)
                {
                    return "alarme-green";
                }

                if (this.SupAlarmLevelMin && !this.SupAlarmLevelMax)
                {
                    return "alarme-red";
                }

                if (!this.SupAlarmLevelMin && this.SupAlarmLevelMax)
                {
                    return "alarme-yellow";
                }

                return string.Empty;
            }
        }

        [NotMapped]
        public string AlarmeInfText
        {
            get
            {
                if (this.PackInf == null)
                    return string.Empty;

                if (!this.InfAlarmLevelMin && !this.InfAlarmLevelMax)
                {
                    return "Nível Normal";
                }

                if (this.InfAlarmLevelMin && !this.InfAlarmLevelMax)
                {
                    return "Nível Baixo";
                }

                if (!this.InfAlarmLevelMin && this.InfAlarmLevelMax)
                {
                    return "Nível Alto";
                }

                return string.Empty;
            }
        }

        [NotMapped]
        public string AlarmeInfTextReport
        {
            get
            {
                return this.AlarmeInfText.Replace("Nível ", "");
            }
        }

        [NotMapped]
        public string AlarmeInfNameClass
        {
            get
            {
                if (this.PackSup == null)
                    return string.Empty;

                if (!this.InfAlarmLevelMin && !this.InfAlarmLevelMax)
                {
                    return "alarme-green";
                }

                if (this.InfAlarmLevelMin && !this.InfAlarmLevelMax)
                {
                    return "alarme-red";
                }

                if (!this.InfAlarmLevelMin && this.InfAlarmLevelMax)
                {
                    return "alarme-yellow";
                }

                return string.Empty;
            }
        }

        [NotMapped]
        public string PortFireAlarmNameClass
        {
            get
            {
                if (this.PackPort == null)
                    return string.Empty;

                return (this.PortFireAlarm ? "alarme-red" : "alarme-green");
            }
        }

        [NotMapped]
        public string PortFireStateNameClass
        {
            get
            {
                if (this.PackPort == null)
                    return string.Empty;

                return (this.PortFireState ? "alarme-red" : "alarme-green");
            }
        }

        [NotMapped]
        public string PortFireStateText
        {
            get
            {
                if (this.PackPort == null)
                    return string.Empty;

                return (this.PortFireState ? "Acionado" : "Normal");
            }
        }

        [NotMapped]
        public string PortIvaAlarmNameClass
        {
            get
            {
                if (this.PackPort == null)
                    return string.Empty;

                return (this.PortIvaAlarm ? "alarme-red" : "alarme-green");
            }
        }

        [NotMapped]
        public string PortIvaStateNameClass
        {
            get
            {
                if (this.PackPort == null)
                    return string.Empty;

                return (this.PortIvaState ? "alarme-red" : "alarme-green");
            }
        }

        [NotMapped]
        public string PortIvaStateText
        {
            get
            {
                if (this.PackPort == null)
                    return string.Empty;

                return (this.PortIvaState ? "Acionado" : "Normal");
            }
        }

        [NotMapped]
        public string Latitude { get; set; }

        [NotMapped]
        public string Longitude { get; set; }
        
    }
}