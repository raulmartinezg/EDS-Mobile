namespace Movil.Models
{
    public class Data
    {
        private string zvardbcat = null;
        private int zvarbdCc = 0;
        private string zvardbbdSch = null;
        private int zvarencV;

        public string bdCat
        {
            get
            {
                if (zvardbcat == null)
                    return "SIDTUM_PROD";
                else
                    return zvardbcat;
            }
            set
            {
                zvardbcat = value;
            }
        }
        public int bdCc
        {
            get
            {
                if (zvarbdCc == 0)
                    return 22;
                else
                    return zvarbdCc;
            }
            set
            {
                zvarbdCc = value;
            }
        }
        public string bdSch
        {
            get
            {
                if (zvardbbdSch == null)
                    return "SidMovil";
                else
                    return zvardbbdSch;
            }
            set
            {
                zvardbbdSch = value;
            }
        }
        public string bdSp { get; set; }
        public int encV
        {
            get
            {
                if (zvarencV == 0)
                    return 2;
                else
                    return zvarencV;
            }
            set
            {
                zvarencV = value;
            }
        }
    }

}
