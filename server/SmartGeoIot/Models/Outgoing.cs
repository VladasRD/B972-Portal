using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGeoIot.Models
{
    public class Outgoing
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string OutgoingUId { get; set; }

        [Required]
        public int Year { get; set; }

        [Required]
        public int Month { get; set; }
        public int LicensesActive { get; set; }
        public int ClientsActive { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal DevelopmentValue { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal OperationsWNDValue { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal DataCenterValue { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal OperationValue { get; set; }
    }

    public class VW_Outgoing
    {
        [Key]
        public string OutgoingUId { get; set; }
        public int Year { get; set; }
        public int Month { get; set; }
        public int LicensesActive { get; set; }
        public int ClientsActive { get; set; }
        public string Description { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal DevelopmentValue { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal OperationsWNDValue { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal DataCenterValue { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal OperationValue { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal TotalBilling { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal AverageLicensesActived { get; set; }
        
        [Column(TypeName = "decimal(38, 2)")]
        public decimal AverageForClient { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal AverageForLicenseClient { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal TotalBillingYear { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal DevelopmentValueYear { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal OperationsWNDValueYear { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal DataCenterValueYear { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal OperationValueYear { get; set; }
        public int LicensesActiveYear { get; set; }
    }

    public class OutgoingViewModel
    {
        public VW_Outgoing Outgoing { get; set; }
        public ICollection<OutigoingClient> Clients { get; set; }
    }

    public class OutigoingClient
    {
        [Key]
        public string Name { get; set; }
        public decimal Total { get; set; }
    }
}