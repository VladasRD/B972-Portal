namespace SmartGeoIot.Models
{
    public class SigfoxMessage
    {
        public Datum[] data { get; set; }
        public Paging paging { get; set; }

        public class Paging
        {
            public string next { get; set; }
        }

        public class Datum
        {
            public Device device { get; set; }
            public long time { get; set; }
            public string data { get; set; }
            public int rolloverCounter { get; set; }
            public int seqNumber { get; set; }
            public Rinfo[] rinfos { get; set; }
            public object[] satInfos { get; set; }
            public int nbFrames { get; set; }
            public string _operator { get; set; }
            public string country { get; set; }
            public object[] computedLocation { get; set; }
            public int lqi { get; set; }
        }

        public class Device
        {
            public string id { get; set; }
        }

        public class Rinfo
        {
            public Basestation baseStation { get; set; }
            public float delay { get; set; }
            public string lat { get; set; }
            public string lng { get; set; }
        }

        public class Basestation
        {
            public string id { get; set; }
        }
    }
}