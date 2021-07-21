using SQLite;
using System;
namespace Movil
{
    public class OrdenEmbarqueDetalle
    {

        [PrimaryKey]
        public int ClaveOrdenEmbDet { get; set; }

        public int ClaveArticulo { get; set; }

        public int ClaveOrdenEmbarque { get; set; }

        public int EstatusBinario { get; set; }

        public DateTime FechaProcesado { get; set; }

        [MaxLength(1000)]
        public string Observaciones { get; set; }

        public int Procesado { get; set; }

        [MaxLength(100)]
        public string Serie { get; set; }

        public int Sincronizado { get; set; }

        public int TotalEvento { get; set; }

        public int TotalProcesar { get; set; }

        public int TotalProcesado { get; set; }

        public int Manual { get; set; }
    }
}
