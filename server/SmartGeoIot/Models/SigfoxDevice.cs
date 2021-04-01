namespace SmartGeoIot.Models
{
    public class SigfoxDevice
    {
        public Datum[] data { get; set; }
        public Paging paging { get; set; }

        public class Paging
        {
            public string next { get; set; }
        }

        public class Datum
        {
            public string id { get; set; }
            public string name { get; set; }
            public int sequenceNumber { get; set; }
            public long lastCom { get; set; }
            public int state { get; set; }
            public int comState { get; set; }
            public string pac { get; set; }
            public Location location { get; set; }
            public Devicetype deviceType { get; set; }
            public Group group { get; set; }
            public int lqi { get; set; }
            public long activationTime { get; set; }
            public Token token { get; set; }
            public Contract contract { get; set; }
            public long creationTime { get; set; }
            public Modemcertificate modemCertificate { get; set; }
            public bool prototype { get; set; }
            public bool automaticRenewal { get; set; }
            public int automaticRenewalStatus { get; set; }
            public string createdBy { get; set; }
            public long lastEditionTime { get; set; }
            public string lastEditedBy { get; set; }
            public bool activable { get; set; }
        }

        public class Location
        {
            public string lat { get; set; }
            public string lng { get; set; }
        }

        public class Devicetype
        {
            public string id { get; set; }
        }

        public class Group
        {
            public string id { get; set; }
        }

        public class Token
        {
            public int state { get; set; }
            public string detailMessage { get; set; }
            public long end { get; set; }
        }

        public class Contract
        {
            public string id { get; set; }
        }

        public class Modemcertificate
        {
            public string id { get; set; }
        }
    }
}