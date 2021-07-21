using SQLite;
using System;

namespace Movil.Models.DB
{
    public class LogError
    {
        [PrimaryKey]
        [Unique]
        [AutoIncrement]
        public int id { get; set; }

        public string Mensaje { get; set; }

        public DateTime Fecha { get; set; }
    }
}
