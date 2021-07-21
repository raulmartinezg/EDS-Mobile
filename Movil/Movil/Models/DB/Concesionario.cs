using SQLite;
using System;

namespace Movil
{
    public class Concesionario
    {
        [PrimaryKey]
        public int ClaveConcesionario { get; set; }
        public int CantidadArticulo { get; set; }
        public int TotalProcesar { get; set; }
        public int TotalProcesado { get; set; }
        [MaxLength(1000)]
        public string Direccion { get; set; }
        public DateTime FechaInicioCD { get; set; }
        public DateTime FechaFinCD { get; set; }
        public DateTime? FechaLlegadaEstimada { get; set; }
        public DateTime? FechaSalidaEstimada { get; set; }
        public decimal Latitud { get; set; }
        public decimal Longitud { get; set; }
        public int NumeroConcesionario { get; set; }
        [MaxLength(100)]
        public string NombreConcesionario { get; set; }
        public int Sincronizado { get; set; }
    }
}
