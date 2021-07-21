using SQLite;
using System;

namespace Movil.Models.DB
{
    public class Incidencia
    {
        [PrimaryKey]
        [Unique]
        [AutoIncrement]
        public int id { get; set; }
        public byte[] Imagen { get; set; }
        public string Descripcion { get; set; }
        public int Csr { get; set; }
        public DateTime Fecha { get; set; }
        public string Tipo { get; set; }
        public int Sincronizado { get; set; }
    }
}
