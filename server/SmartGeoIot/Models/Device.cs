using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SmartGeoIot.Extensions;
using SmartGeoIot.ViewModels;

namespace SmartGeoIot.Models
{
    public class Device
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public int SequenceNumber { get; set; }
        public long LastCom { get; set; }
        public int State { get; set; }
        public int ComState { get; set; }
        public string Pac { get; set; }
        public string LocationLat { get; set; }
        public string LocationLng { get; set; }
        public string DeviceTypeId { get; set; }
        public string GroupId { get; set; }
        public int Lqi { get; set; }
        public long ActivationTime { get; set; }
        public int TokenState { get; set; }
        public string TokenDetailMessage { get; set; }
        public long TokenEnd { get; set; }
        public string ContractId { get; set; }
        public long CreationTime { get; set; }
        public string ModemCertificateId { get; set; }
        public bool Prototype { get; set; }
        public bool AutomaticRenewal { get; set; }
        public int AutomaticRenewalStatus { get; set; }
        public string CreatedBy { get; set; }
        public long LastEditionTime { get; set; }
        public string LastEditedBy { get; set; }
        public bool Activable { get; set; }

        [NotMapped]
        public Bits Bits { get; set; }
    }

    public class DeviceRegistration
    {

        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string DeviceCustomUId { get; set; }

        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(100)]
        public string NickName { get; set; }

        // [MaxLength(50)]
        public string DeviceId { get; set; }

        [ForeignKey("DeviceId")]
        public Device Device { get; set; }


        public string PackageUId { get; set; }

        [ForeignKey("PackageUId")]
        public Package Package { get; set; }


        public string ProjectUId { get; set; }

        [ForeignKey("ProjectUId")]
        public Project Project { get; set; }

        [MaxLength(50)]
        public string DataDownloadLink { get; set; }

        [MaxLength(80)]
        public string SerialNumber { get; set; }
        
        [MaxLength(50)]
        public string Model { get; set; }
        public string Notes { get; set; }
        public DateTime? NotesCreateDate { get; set; }
        public string Ed1 { get; set; }
        public string Ed2 { get; set; }
        public string Ed3 { get; set; }
        public string Ed4 { get; set; }
        public string Sd1 { get; set; }
        public string Sd2 { get; set; }
        public string Ea10 { get; set; }
        public string Sa3 { get; set; }

        [NotMapped]
        public string Envio
        {
            get
            {
                if (this.DataDownloadLink == null)
                    return string.Empty;

                return Utils.HexaToDecimal(this.DataDownloadLink.Substring(10, 2)).ToString();
            }
        }

        [NotMapped]
        public string PeriodoTransmissao
        {
            get
            {
                if (this.DataDownloadLink == null)
                    return string.Empty;

                return Utils.HexaToDecimal(this.DataDownloadLink.Substring(4, 4)).ToString();
            }
        }

        [NotMapped]
        public bool BaseTempoUpLink
        {
            get
            {
                if (this.DataDownloadLink == null)
                    return false;

                var _chars = Utils.StringToHexadecimalTwoChars(this.DataDownloadLink);
                var _bytes = Utils.ByteToBinary(_chars[1]);
                return Convert.ToBoolean(int.Parse(_bytes.Substring(7, 1)));
            }
        }

        [NotMapped]
        public string TensaoMinima
        {
            get
            {
                if (this.DataDownloadLink == null)
                    return string.Empty;

                return Utils.HexaToDecimal(this.DataDownloadLink.Substring(8, 2)).ToString();
            }
        }
    }

    public class DeviceType
    {
        public string id { get; set; }
        public string downlinkDataString { get; set; }
    }

    public class DeviceLocation
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string DeviceLocationUId { get; set; }
        public string DeviceId { get; set; }
        public long Time { get; set; }

        [MaxLength(50)]
        public string Data { get; set; }

        [MaxLength(50)]
        public string Radius { get; set; }

        [MaxLength(100)]
        public double Latitude { get; set; }

        [MaxLength(100)]
        public double Longitude { get; set; }
    }
    
}
