using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
using SmartGeoIot.Extensions;

namespace SmartGeoIot.Models
{
    public class Message
    {
        [Key]
        public string Id { get; set; }
        public string DeviceId { get; set; }
        public long Time { get; set; }
        public string Data { get; set; }
        public int RolloverCounter { get; set; }
        public int SeqNumber { get; set; }
        public int NbFrames { get; set; }
        public string Operator { get; set; }
        public string Country { get; set; }
        public int Lqi { get; set; }
        public DateTime? OperationDate { get; set; }

        [ForeignKey("DeviceId")]
        public Device Device { get; set; }

        [NotMapped]
        public string TypePackage
        {
            get
            {
                return Data.Substring(0, 2);
            }
        }

        [NotMapped]
        public string Package
        {
            get
            {
                return Data.Substring(2, Data.Length - 2);
            }
        }

        [NotMapped]
        public DateTime Date
        {
            get
            {
                return Utils.TimeStampToDateTime(Time).ToUniversalTime();//.AddHours(-3);
            }
        }

        [NotMapped]
        public int EstadoDetector
        {
            get
            {
                // string bits = Utils.ByteToBinary(PackageToByteArray[1]);
                // return Convert.ToInt32(bits.Substring(0, 3), 2);
                return (int)Utils.HexaToDecimal(this.Package.Substring(2, 2));
            }
        }

        [NotMapped]
        public bool AlertaFonteBaixa
        {
            get
            {
                string bits = Utils.ByteToBinary(PackageToByteArray[1]);
                return Convert.ToBoolean(int.Parse(bits.Substring(4, 1)));
            }
        }

        [NotMapped]
        public ViewModels.Bits Bits
        {
            get
            {
                if (this.TypePackage.Equals("10"))
                {
                    return new ViewModels.Bits()
                    {
                        Downlink = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(2, 1))),
                        EstadoAlimentacao = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(3, 1))),
                        AlertaSensor2 = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(4, 1))),
                        AlertaSensor1 = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(5, 1))),
                        BaseTempoUpLink = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(6, 1))),
                        TipoEnvio = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(7, 1)))
                    };
                }

                if (this.TypePackage.Equals("12"))
                {
                    return new ViewModels.Bits()
                    {
                        TipoEnvio = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(7, 1))),
                        BaseTempoUpLink = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(6, 1))),
                        EstadoPosChave = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(5, 1))),
                        EstadoViolacaoPainel = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(4, 1))),
                        EstadoViolacaoFalso = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(3, 1))),
                        EstadoEntradaRastreador = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(2, 1))),
                        EstadoBloqueio = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(1, 1))),
                        EstadoSaidaRastreador = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(0, 1)))
                    };
                }

                 if (this.TypePackage.Equals("21"))
                 {
                     //00010101
                     var _bitsByteToBinary2 = GetBitsByteToBinary(1);

                     return new ViewModels.Bits()
                     {
                        Bed1 = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(7, 1))),
                        Bed2 = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(6, 1))),
                        Bed3 = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(5, 1))),
                        Bed4 = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(4, 1))),
                        Bsd1 = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(3, 1))),
                        Bsd2 = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(2, 1))),



                        Btxev = Convert.ToBoolean(int.Parse(_bitsByteToBinary2.Substring(7, 1))),
                        BAlertaMin = Convert.ToBoolean(int.Parse(_bitsByteToBinary2.Substring(6, 1))),
                        BAlertaMax = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(5, 1)))
                     };
                 }

                 if (this.TypePackage.Equals("23"))
                 {
                      return new ViewModels.Bits()
                      {
                            BAlertaMax = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(7, 1))),
                            ModoFechado = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(6, 1))),
                            ModoAberto = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(5, 1)))
                      };
                 }

                return new ViewModels.Bits()
                {
                    Iluminacao = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(0, 1))),
                    BombaCirculacao = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(1, 1))),
                    FalhaEnergia = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(2, 1))),
                    Automatico = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(3, 1))),
                    SensorNivelOperacional = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(4, 1))),
                    AlertaNivelMinimo = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(5, 1))),
                    AlertaNivelMaximo = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(6, 1))),
                    BombaOxigenacao = Convert.ToBoolean(int.Parse(BitsByteToBinary.Substring(7, 1)))
                };
            }
        }

        [NotMapped]
        public string DataCompilacao
        {
            get
            {
                return $"{PackageToByteArray[0]} de {Utils.GetMonthName(PackageToByteArray[1])} de 20{PackageToByteArray[2]}";
            }
        }

        [NotMapped]
        public string SerieDispositivo
        {
            get
            {
                return PackageToByteArray[3].ToString();
            }
        }

        [NotMapped]
        public string VersaoHardware
        {
            get
            {
                return PackageToByteArray[4].ToString();
            }
        }

        [NotMapped]
        public string RevisaoHardware
        {
            get
            {
                return PackageToByteArray[5].ToString();
            }
        }

        [NotMapped]
        public string VersaoFirmware
        {
            get
            {
                return PackageToByteArray[6].ToString();
            }
        }

        [NotMapped]
        public string RevisaoFirmware
        {
            get
            {
                return PackageToByteArray[7].ToString();
            }
        }

        [NotMapped]
        public string Proc
        {
            get
            {
                string proc = this.Package.Substring(0, 2);
                if (proc == "04")
                    return "F67K22";

                if (proc == "05")
                    return "F26K20";

                if (proc == "14")
                    return "LF26K42";

                if (proc == "15")
                    return "F47K42";

                if (proc == "16")
                    return "F26K42";

                return proc;
            }
        }

        [NotMapped]
        public string Placa
        {
            get
            {
                return $"C{Utils.HexaToDecimal(this.Package.Substring(2, 2))}";
            }
        }

        [NotMapped]
        public string VPlaca
        {
            get
            {
                string vPlaca = Utils.HexaToDecimal(this.Package.Substring(4, 2)).ToString();
                return $"{vPlaca.Substring(0, 1)}.{vPlaca.Substring(1, 1)}";
            }
        }

        [NotMapped]
        public string NAplic
        {
            get
            {
                string nAplic = this.Package.Substring(6, 4);
                if (nAplic == "0017")
                    return "AguaMon";

                if (nAplic == "0018")
                    return "DJRF";

                return nAplic;
            }
        }

        [NotMapped]
        public string Level
        {
            get
            {
                return PackageToByteArray[1].ToString();
            }
        }

        [NotMapped]
        public string Light
        {
            get
            {
                var luz = Utils.HexaToDecimal(Data.Substring(2, Data.Length - 2).Substring(8, 4));
                return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:00.0}", luz);
            }
        }

        [NotMapped]
        public string Temperature
        {
            get
            {
                if (this.TypePackage.Equals("10"))
                    return $"{PackageToByteArray[6]},{PackageToByteArray[7]}";

                if (this.TypePackage.Equals("13"))
                    return Utils.HexaToDecimal(this.Package.Substring(0, 4)).ToString();

                if (this.TypePackage.Equals("81"))
                {
                    var _temperature = Utils.HexaToDecimal(this.Package.Substring(0, 4)).ToString();
                    if (_temperature.Length > 2)
                    {
                        string _secondValue = _temperature.Substring(2, 1).Length > 1 ? _temperature.Substring(2, 2) : $"{_temperature.Substring(2, 1)}0";
                        return $"{_temperature.Substring(0, 2)},{_secondValue}";
                    }
                    else
                        return _temperature;
                }

                return $"{PackageToByteArray[2]},{PackageToByteArray[3]}";
            }
        }

        [NotMapped]
        public bool TemperatureIsZero
        {
            get
            {
                if (this.TypePackage.Equals("10"))
                    return false;

                if (this.TypePackage.Equals("13") || this.TypePackage.Equals("81"))
                    return Utils.HexaToDecimal(this.Package.Substring(0, 4)) == 0;

                return false;
            }
        }

        [NotMapped]
        public string Envio
        {
            get
            {
                return $"{PackageToByteArray[5]}";
            }
        }

        [NotMapped]
        public string Moisture
        {
            get
            {
                return $"{PackageToByteArray[6]},{PackageToByteArray[7]}";
            }
        }

        [NotMapped]
        public string OxigenioDissolvido
        {
            get
            {
                return Utils.HexaToDecimal(Package.Substring(16, 4)).ToString();
            }
        }

        [NotMapped]
        public string Ph
        {
            get
            {
                string tempPh = null;
                if (!TypePackage.Equals("23") || !TypePackage.Equals("81"))
                    return string.Empty;

                if (TypePackage.Equals("81"))
                    tempPh = Utils.HexaToDecimal(Package.Substring(4, 4)).ToString();
                else
                    tempPh = Utils.HexaToDecimal(Package.Substring(0, 4)).ToString();
                
                if (string.IsNullOrWhiteSpace(tempPh) || tempPh.Equals("0"))
                    return "0";

                if (TypePackage.Equals("81"))
                {
                    string _secondValue = tempPh.Substring(tempPh.Length - 2, 1).Length > 1 ? tempPh.Substring(tempPh.Length - 2, 2) : $"{tempPh.Substring(tempPh.Length - 2, 1)}0";
                    return $"{tempPh.Substring(0, tempPh.Length - 2)},{_secondValue}";
                }

                return $"{tempPh.Substring(0, tempPh.Length - 2)},{tempPh.Substring(tempPh.Length - 2, 2)}";
            }
        }

        [NotMapped]
        public string Condutividade
        {
            get
            {
                if (TypePackage.Equals("23"))
                {
                    decimal condutividade = Utils.HexaToDecimal(Package.Substring(4, 4));
                    return string.Format(CultureInfo.GetCultureInfo("pt-BR"), "{0:N}", condutividade);
                }
                else
                    return string.Empty;
            }
        }

        [NotMapped]
        public string PeriodoTransmissao
        {
            get
            {
                if (this.TypePackage.Equals("23"))
                    return Utils.HexaToDecimal(this.Package.Substring(8, 4)).ToString();
                else if (this.TypePackage.Equals("10"))
                    return Utils.HexaToDecimal(this.Package.Substring(2, 4)).ToString();
                else if (this.TypePackage.Equals("12"))
                    return Utils.HexaToDecimal(this.Package.Substring(4, 4)).ToString();
                else
                    return string.Empty;
            }
        }

        [NotMapped]
        public string ContadorCarencias
        {
            get
            {
                return Utils.HexaToDecimal(this.Package.Substring(8, 8)).ToString();
            }
        }

        [NotMapped]
        public string ContadorBloqueios
        {
            get
            {
                return Utils.HexaToDecimal(this.Package.Substring(16, 4)).ToString();
            }
        }

        [NotMapped]
        public string Alimentacao
        {
            get
            {
                if (this.TypePackage.Equals("10"))
                    return (Utils.HexaToDecimal(Package.Substring(6, 2)) / 10).ToString();
                else if (this.TypePackage.Equals("13"))
                    return (Utils.HexaToDecimal(Package.Substring(4, 2)) / 10).ToString();
                else
                    return string.Empty;
            }
        }

        [NotMapped]
        public string AlimentacaoH
        {
            get
            {
                if (this.TypePackage.Equals("13"))
                    return (Utils.HexaToDecimal(Package.Substring(4, 2)) / 10).ToString();
                else
                    return string.Empty;
            }
        }

        [NotMapped]
        public string AlimentacaoL
        {
            get
            {
                if (this.TypePackage.Equals("13"))
                    return (Utils.HexaToDecimal(Package.Substring(6, 2)) / 10).ToString();
                else
                    return string.Empty;
            }
        }

        [NotMapped]
        public string AlimentacaoMinima
        {
            get
            {
                if (this.TypePackage.Equals("10"))
                    return (Utils.HexaToDecimal(Package.Substring(8, 2)) / 10).ToString();
                else
                    return string.Empty;
            }
        }

        [NotMapped]
        public int BaseT
        {
            get
            {
                int baseT = 0;
                if ((int)PackageToByteArray[9] == 1)
                    baseT = 1;

                return baseT;
            }
        }

        //somente para consulta
        [NotMapped]
        public byte[] PackageToByteArray
        {
            get
            {
                return Utils.StringToHexadecimalTwoChars(Package);
            }
        }

        //somente para consulta
        [NotMapped]
        public string BitsByteToBinary
        {
            get
            {
                return Utils.ByteToBinary(PackageToByteArray[0]);
            }
        }

        public string GetBitsByteToBinary(int bit)
        {
            return Utils.ByteToBinary(PackageToByteArray[bit]);
        }

        [NotMapped]
        public string Fluor
        {
            get
            {
                decimal _fluor = Utils.HexaToDecimal(this.Package.Substring(8, 4));
                string fluor = String.Format(Utils.GetFormatDecimalToString(_fluor.ToString()), _fluor);
                string _secondValue = fluor.Substring(fluor.Length - 1, 1).Length > 1 ? fluor.Substring(fluor.Length - 1, 2) : $"{fluor.Substring(fluor.Length - 1, 1)}0";
                return $"{fluor.Substring(0, fluor.Length - 1)},{_secondValue}";
            }
        }

        [NotMapped]
        public string Cloro
        {
            get
            {
                decimal _cloro = Utils.HexaToDecimal(this.Package.Substring(12, 4));
                string cloro = String.Format(Utils.GetFormatDecimalToString(_cloro.ToString()), _cloro);
                string _secondValue = cloro.Substring(cloro.Length - 2, 1).Length > 1 ? cloro.Substring(cloro.Length - 2, 2) : $"{cloro.Substring(cloro.Length - 2, 1)}0";
                return $"{cloro.Substring(0, cloro.Length - 2)},{_secondValue}";
            }
        }

        [NotMapped]
        public string Turbidez
        {
            get
            {
                decimal _turbidez = Utils.HexaToDecimal(this.Package.Substring(16, 4));
                string turbidez = String.Format(Utils.GetFormatDecimalToString(_turbidez.ToString()), _turbidez);
                string _secondValue = turbidez.Substring(turbidez.Length - 1, 1).Length > 1 ? turbidez.Substring(turbidez.Length - 1, 2) : $"{turbidez.Substring(turbidez.Length - 1, 1)}0";
                return $"{turbidez.Substring(0, turbidez.Length - 1)},{_secondValue}";
            }
        }

        [NotMapped]
        public string Vazao
        {
            get
            {
                var _vazao = Utils.HexaToDecimal(this.Package.Substring(0, 4)).ToString();
                return $"{_vazao.Substring(0, _vazao.Length - 1)},{_vazao.Substring(_vazao.Length - 1, 1)}";
            }
        }

        [NotMapped]
        public string TotalizacaoParcial
        {
            get
            {
                return Utils.HexaToDecimal(this.Package.Substring(4, 4)).ToString();
            }
        }

        [NotMapped]
        public string Totalizacao
        {
            get
            {
                var _totalizacao = Utils.HexaToDecimal(this.Package.Substring(8, 8)).ToString();
                return $"{_totalizacao.Substring(0, 1)}.{_totalizacao.Substring(1, _totalizacao.Length - 1)}";
            }
        }

        [NotMapped]
        public string TempoParcial
        {
            get
            {
                return Utils.HexaToDecimal(this.Package.Substring(16, 4)).ToString();
            }
        }

        [NotMapped]
        public long EntradaAnalogica
        {
            get
            {
                return Utils.HexaToLong(this.Package.Substring(4, 8));
            }
        }

        [NotMapped]
        public long SaidaAnalogica
        {
            get
            {
                return Utils.HexaToLong(this.Package.Substring(12, 8));
            }
        }

        [NotMapped]
        public long FluxoAgua
        {
            get
            {
                var _value = this.Package.Substring(2, 8);
                return Utils.HexaToLong($"{_value.ToString().Substring(6,2)}{_value.ToString().Substring(4,2)}{_value.ToString().Substring(2,2)}{_value.ToString().Substring(0,2)}");
            }
        }

        [NotMapped]
        public long ConsumoAgua
        {
            get
            {
                var _value = this.Package.Substring(10, 8);
                return Utils.HexaToLong($"{_value.ToString().Substring(6,2)}{_value.ToString().Substring(4,2)}{_value.ToString().Substring(2,2)}{_value.ToString().Substring(0,2)}");
            }
        }


    }
}