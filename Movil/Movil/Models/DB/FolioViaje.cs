using SQLite;
using System;

namespace Movil

{
    public class FolioViaje
    {

        [PrimaryKey]
        public int ClaveFolioViaje { get; set; }
        public int ClaveOperador { get; set; }
        [MaxLength(100)]
        public string FolioDeViaje { get; set; }

        public int ClaveUnidad { get; set; }

        public int CantidadArticulo { get; set; }

        public int Completadas { get; set; }

        public DateTime? FechaPrimeraEntrega { get; set; }

        public DateTime? FechaProcesado { get; set; }

        public DateTime? FechaSalidaProgramada { get; set; }

        public DateTime? FechaUltimaEntrega { get; set; }

        public int NumeroOrdenes { get; set; }

        public int Paradas { get; set; }

        public int Procesado { get; set; }
        [MaxLength(150)]
        public string Ruta { get; set; }

        public int Sincronizado { get; set; }

        [MaxLength(10)]
        public string Unidad { get; set; }

    }
}
