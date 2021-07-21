namespace Movil.Models.Request
{
    public class ViajesRequest
    {
        public zData data { get; set; }

        public string filter { get; set; }
    }

    public class zData
    {
        public string bdCat { get; set; }

        public int bdCc { get; set; }

        public string bdSch { get; set; }

        public string bdSp { get; set; }

        public int encV { get; set; }
    }

    public class Filter
    {
        public string property { get; set; }

        public int value { get; set; }
    }
}
