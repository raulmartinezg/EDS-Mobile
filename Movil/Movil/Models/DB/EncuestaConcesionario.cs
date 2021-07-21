using SQLite;
using System;

namespace Movil
{
    public class EncuestaConcesionario
    {
        [PrimaryKey]
        [AutoIncrement]
        public int ClaveEncCon { get; set; }
        public int ClaveFolioViaje { get; set; }
        public int ClaveConcesionario { get; set; }
        public int ClavePregunta { get; set; }
        [MaxLength(50)]
        public string Respuesta { get; set; }
        public DateTime Fecha { get; set; }
        [MaxLength(1000)]
        public string Observaciones { get; set; }
        [MaxLength(50)]
        public string NumEmpleado { get; set; }
        public int ClaveEstatus { get; set; }
        public int Sincronizado { get; set; }
    }
}
