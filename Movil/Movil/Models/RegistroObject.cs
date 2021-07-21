using System;

namespace Movil.Models
{
    public class RegistroObject
    {
        public int claveConcesionario { get; set; }

        public string empleado { get; set; }

        public string nombreEmpleado { get; set; }

        public byte[] firma { get; set; }

        public DateTime fecha { get; set; }

        public byte[] imagen { get; set; }
    }
}
