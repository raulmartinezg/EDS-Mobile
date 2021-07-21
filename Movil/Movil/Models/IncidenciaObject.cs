using System;

namespace Movil.Models
{
    public class IncidenciaObject
    {
        public byte[] Imagen { get; set; }
        public string Descripcion { get; set; }
        public int Csr { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
    }
}
