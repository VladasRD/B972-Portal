namespace SmartGeoIot.ViewModels
{
    public class GerenciaNetViewModels
    {
        public class ChargeResponse
        {
            public int code { get; set; }
            public Data data { get; set; }
        }

        public class Data
        {
            public string barcode { get; set; }
            public string link { get; set; }
            public Pdf pdf { get; set; }
            public string expire_at { get; set; }
            public int charge_id { get; set; }
            public string status { get; set; }
            public int total { get; set; }
            public string payment { get; set; }
        }

        public class Pdf
        {
            public string charge { get; set; }
        }
    }

    public class DetailCharge
    {
        public class DetailChargeResponde
        {
            public int code { get; set; }
            public Data data { get; set; }
        }

        public class Data
        {
            public int charge_id { get; set; }
            public int total { get; set; }
            public string status { get; set; }
            public string custom_id { get; set; }
            public string created_at { get; set; }
            public string notification_url { get; set; }
            public Item[] items { get; set; }
            public History[] history { get; set; }
            public Customer customer { get; set; }
            public Payment payment { get; set; }
        }
        public class Customer
        {
            public string name { get; set; }
            public string cpf { get; set; }
            public string birth { get; set; }
            public string email { get; set; }
            public string phone_number { get; set; }
            public Juridical_Person juridical_person { get; set; }
        }

        public class Juridical_Person
        {
            public string corporate_name { get; set; }
            public string cnpj { get; set; }
        }

        public class Payment
        {
            public string method { get; set; }
            public string created_at { get; set; }
            public object message { get; set; }
            public Banking_Billet banking_billet { get; set; }
        }

        public class Banking_Billet
        {
            public string barcode { get; set; }
            public string link { get; set; }
            public Pdf pdf { get; set; }
            public string expire_at { get; set; }
            public Configurations configurations { get; set; }
        }

        public class Pdf
        {
            public string charge { get; set; }
        }

        public class Configurations
        {
            public int interest { get; set; }
            public int fine { get; set; }
        }

        public class Item
        {
            public string name { get; set; }
            public int value { get; set; }
            public int amount { get; set; }
        }

        public class History
        {
            public string message { get; set; }
            public string created_at { get; set; }
        }
    }

}