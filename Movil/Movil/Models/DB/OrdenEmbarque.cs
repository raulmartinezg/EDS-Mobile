using SQLite;
using System;

namespace Movil
{
    public class OrdenEmbarque
    {
        [PrimaryKey]
        public int ClaveOrdenEmbarque { get; set; }
        public int ClaveConcesionario { get; set; }
        public int ClaveFolioViaje { get; set; }

        [MaxLength(50)]
        public string NumOrdenEmbarque { get; set; }

        public int ClaveTipoDocumento { get; set; }

        public DateTime? FechaProcesado { get; set; }

        public int Procesado { get; set; }

        public int Sincronizado { get; set; }

        public int TotalEvento { get; set; }

        public decimal TotalProcesar { get; set; }

        public int TotalProcesado { get; set; }

    }
}
