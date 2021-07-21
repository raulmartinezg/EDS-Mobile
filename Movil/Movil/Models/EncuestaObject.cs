using System;

namespace Movil.Models
{
    public class EncuestaObject
    {
        public int ClaveEncCon { get; set; }
        public int ClaveFolioViaje { get; set; }
        public int ClaveConcesionario { get; set; }
        public int ClavePregunta { get; set; }
        public string Respuesta { get; set; }
        public DateTime Fecha { get; set; }
        public string Observaciones { get; set; }
        public string NumEmpleado { get; set; }
        public int ClaveEstatus { get; set; }
        public int Sincronizado { get; set; }
    }
}
