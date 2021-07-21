using System;
using System.Collections.ObjectModel;

namespace Movil.Models.Response
{
    public class ResultViajes
    {
        public bool success { get; set; }
        public vData data { get; set; }
        public int total { get; set; }
    }

    public class vNewDataSet
    {
        public int cfv { get; set; }
        public int cop { get; set; }
        public int cun { get; set; }
        public string fvi { get; set; }
        public string fvr { get; set; }
        public string rut { get; set; }
        public DateTime? fsp { get; set; }
        public int cin { get; set; }
        public string ori { get; set; }
        public int recTotal { get; set; }
    }

    public class vData
    {
        public ObservableCollection<vNewDataSet> newDataSet { get; set; }
    }

}
