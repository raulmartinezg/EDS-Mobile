using SQLite;

namespace Movil
{
    public class Encuesta
    {
        [PrimaryKey]
        [Unique]
        [AutoIncrement]
        public int ClavePregunta { get; set; }

        public int ClaveEncuesta { get; set; }

        public int Version { get; set; }
        [MaxLength(100)]
        public string TituloOPC { get; set; }

        public int NumP { get; set; }
        [MaxLength(500)]
        public string Pregunta { get; set; }

        [MaxLength(5)]
        public string TipoOPC { get; set; }
        public int NumOPC { get; set; }
    }
}
