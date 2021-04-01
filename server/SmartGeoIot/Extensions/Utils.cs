using System;
using System.Globalization;
using System.Numerics;
using System.Text;

namespace SmartGeoIot.Extensions
{
    public class Utils
    {
        public static DateTime TimeStampToDateTime(long valor)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1);
            return dtDateTime.AddMilliseconds(valor).ToLocalTime();
        }
        public static DateTime TimeStampSecondsToDateTime(long valor)
        {
            DateTime dtDateTime = new DateTime(1970, 1, 1);
            return dtDateTime.AddSeconds(valor).ToLocalTime();
        }
        public static byte[] StringToHexadecimalTwoChars(String HexString)
        {
            int numberChars = HexString.Length / 2;
            byte[] bytes = new byte[numberChars];
            int step = 0;
            for (int i = 0; i < numberChars; i++)
            {
                bytes[i] = Convert.ToByte(HexString.Substring(step, 2).ToUpper(), 16);
                step += 2;
            }
            return bytes;
        }
        public static string ByteToBinary(byte value)
        {
            return Convert.ToString(value, 2).PadLeft(8, '0');
        }

        public static string SumHexValuesOfPack(string package)
        {
            var bytesOfPack = StringTwoChars(package);
            BigInteger number1 = 0;
            BigInteger number2 = 0;
            BigInteger sum = 0;

            for (int i = 0; i < bytesOfPack.Length; i+=2)
            {
                number1 = BigInteger.Parse(bytesOfPack[i], NumberStyles.HexNumber);
                number2 = BigInteger.Parse(bytesOfPack.Length-1 == i ? "00" : bytesOfPack[i+1], NumberStyles.HexNumber);
                sum += BigInteger.Add(number1, number2);
            }
            return sum.ToString("X");
        }

        public static string[] StringTwoChars(String hexString)
        {
            int numberChars = hexString.Length / 2;
            string[] stringArray = new string[numberChars];
            int step = 0;
            for (int i = 0; i < numberChars; i++)
            {
                stringArray[i] = hexString.Substring(step, 2).ToUpper();
                step += 2;
            }
            return stringArray;
        }

        public static decimal HexaToDecimal(string value)
        {
            return (Decimal)Convert.ToInt32(value, 16);
        }

        public static long HexaToLong(string value)
        {
            return (long)Convert.ToInt64(value, 16);
        }

        public static byte[] StringToHexadecimalFourChars(String HexString)
        {
            int numberChars = HexString.Length / 4;
            byte[] bytes = new byte[numberChars];
            int step = 0;
            for (int i = 0; i < numberChars; i++)
            {
                bytes[i] = Convert.ToByte(HexString.Substring(step, 2).ToUpper(), 16);
                step += 2;
            }
            return bytes;
        }

        public static string CreateBasicOauth(string login, string password)
        {
            return Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", login, password)));
        }

        public static string DecimalToHexa(string value)
        {
            return Convert.ToString(long.Parse(value), 16);
        }

        public static string ZerosForLeft(string value, int quantityNumbers)
        {
            return value.PadLeft(quantityNumbers, '0');
        }

        public static string BinaryStringToHexString(string binary)
        {
            if (string.IsNullOrEmpty(binary))
                return binary;

            StringBuilder result = new StringBuilder(binary.Length / 8 + 1);
            int mod4Len = binary.Length % 8;
            if (mod4Len != 0)
                binary = binary.PadLeft(((binary.Length / 8) + 1) * 8, '0');

            for (int i = 0; i < binary.Length; i += 8)
            {
                string eightBits = binary.Substring(i, 8);
                result.AppendFormat("{0:X2}", Convert.ToByte(eightBits, 2));
            }

            return result.ToString();
        }

        public static string GetMonthName(int enumMonth)
        {
            string[] months = new string[]
            {
                "Janeiro",
                "Fevereiro",
                "Março",
                "Abril",
                "Maio",
                "Junho",
                "Julho",
                "Agosto",
                "Setembro",
                "Outubro",
                "Novembro",
                "Dezembro"
            };
            return months[enumMonth];
        }

        public static string GetFormatDecimalToString(string value)
        {
            return "{" + $"0:0{"".PadLeft(value.Length > 1 ? value.Length-1 : value.Length, '0')}" + "}";
        }
        
    }
}