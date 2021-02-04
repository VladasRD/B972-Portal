namespace Box.Common
{
    public class BoxSettings {
        public string APPLICATION_NAME { get;set; }

        public bool ENCRYPT_FILES { get;set; }
        public int CMS_THUMB_WIDTH { get;set; }
        public int CMS_THUMB_HEIGHT { get;set; }

        public string ENCRYPT_KEY { get;set; }
        public string ENCRYPT_IV { get;set; }

        public bool CMS_DEBUG {get; set; }

    }
}