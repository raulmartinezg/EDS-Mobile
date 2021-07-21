using SQLite;
using System;

namespace Movil.Models.DB
{
    public class Salida
    {
        [PrimaryKey]
        [Unique]
        [AutoIncrement]
        public int Id { get; set; }

        public int ClaveConcesionario { get; set; }

        public string Empleado { get; set; }

        public string NombreEmpleado { get; set; }

        public byte[] Firma { get; set; }

        public DateTime Fecha { get; set; }

        public int Sincronizado { get; set; }
    }
}
