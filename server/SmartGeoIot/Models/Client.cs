using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Box.Security.Models;

namespace SmartGeoIot.Models
{
    public enum DocumentType
    {
        CPF,
        CNPJ
    }

    public enum BillingType
    {
        Mensal,
        Trimestral,
        Semestral,
        Anual
    }

    public class Client
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string ClientUId { get; set; }

        [MaxLength(500)]
        public string Name { get; set; }
        public DocumentType DocumentType { get; set; }

        [MaxLength(14)]
        public string Document { get; set; }
        public string Address { get; set; }
        
        [MaxLength(10)]
        public string AddressNumber { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Neighborhood { get; set; }
        public bool Active { get; set; }

        [MaxLength(255)]
        public string Email  { get; set; }

        [MaxLength(14)]
        public string Phone { get; set; }

        [MaxLength(8)]
        public string PostalCode { get; set; }

        [ForeignKey("ClientUId")]
        public ICollection<ClientDevice> Devices { get; set; }

        [ForeignKey("ClientUId")]
        public ICollection<ClientUser> Users { get; set; }

        public DateTime? StartBilling { get; set; }
        public int? DueDay { get; set; }
        public string Item { get; set; }
        public int? Type { get; set; }

        [Column(TypeName = "decimal(38, 2)")]
        public decimal? Value { get; set; }

        [MaxLength(11)]
        public string Cpf { get; set; }
        public DateTime? Birth { get; set; }

        [ForeignKey("ClientUId")]
        public ICollection<ClientBilling> Billings { get; set; }
        public bool EmailNotification { get; set; }
        public bool SMSNotification { get; set; }
        public bool WhatsAppNotification { get; set; }
        public bool PushNotification { get; set; }
        public string ClientFatherUId { get; set; }
        public DateTime Created { get; set; }
    }

    public class ClientDevice
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string ClientDeviceUId { get; set; }
        public string ClientUId { get; set; }
        public string Id { get; set; }
        public bool Active { get; set; }

        [ForeignKey("Id")]
        public Device AppDevice { get; set; }
    }

    public class ClientUser
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string ClientUserUId { get; set; }
        public string ClientUId { get; set; }

        [ForeignKey("AppUser")]
        public string ApplicationUserId { get; set; }

        [ForeignKey("ApplicationUserId")]
        public ApplicationUser AppUser { get; set; }
    }

    public class ClientBilling
    {
        [Key, Column(TypeName = "char(36)"), MaxLength(36)]
        public string ClientBillingUId { get; set; }
        public string ClientUId { get; set; }
        public DateTime Create { get; set; }
        public DateTime? PaymentDate { get; set; }
        public DateTime? PaymentDueDate { get; set; }
        public bool Sended { get; set; }
        public int ExternalId { get; set; }
        public string BarCode { get; set; }
        public string LinkPdf { get; set; }
        public string Status { get; set; }

        public string StatusName
        {
            get 
            {
                if (this.Status.ToLower().Equals("paid"))
                {
                    return "Pagamento efetuado";
                }
                if (this.Status.ToLower().Equals("canceled"))
                {
                    return "Cobrança cancelada pelo vendedor ou pelo pagador";
                }
                if (this.Status.ToLower().Equals("expired"))
                {
                    return "Data de pagamento expirada";
                }
                if (this.Status.ToLower().Equals("unpaid"))
                {
                    return "Não foi possível confirmar o pagamento da cobrança";
                }
                return "Aguardando pagamento";

            }
        }
    }
}