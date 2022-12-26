using System.ComponentModel.DataAnnotations.Schema;

namespace SmartGeoIot.Models
{
    public class CallBackDataAdvanced
    {
        public string device { get; set; }
        public long timestamp { get; set; }
        public string data { get; set; }
        public string type { get; set; }
        public int seqNumber { get; set; }
        public CallBackMessage[] messages { get; set; }

        [NotMapped]
        public string TypePackage
        {
            get
            {
                return this.data != null ? this.data.Substring(0, 2) : "";
            }
        }
        

        [NotMapped]
        public string Package
        {
            get
            {
                return this.data != null ? this.data.Substring(2, this.data.Length - 2) : "";
            }
        }
    }
    
    public class CallBackMessage
    {
        public string key { get; set; }
        public object value { get; set; }
    }

    public class CallBackMessageValue
    {
        public string lat { get; set; }
        public string lng { get; set; }
        public string radius { get; set; }
        public string source { get; set; }
        public string status { get; set; }
    }
}