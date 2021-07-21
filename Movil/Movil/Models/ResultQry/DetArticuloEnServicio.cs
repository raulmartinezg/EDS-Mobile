namespace Movil.Models.ResultQry
{
    public class DetArticuloEnServicio
    {
        public int ClaveOrdenEmbarque { get; set; }
        public int ClaveOrdenEmbDet { get; set; }

        public int ClaveArticulo { get; set; }

        public int TotalProcesar { get; set; }

        public int TotalProcesado { get; set; }

        public int PorProcesar { get; set; }

        public int EstatusBinario { get; set; }

        public int Procesado { get; set; }

        public string UM { get; set; }

        public string NumOrdenEmbarque { get; set; }
    }
}
