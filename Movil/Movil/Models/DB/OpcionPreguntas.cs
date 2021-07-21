using SQLite;

namespace Movil
{
    public class OpcionPreguntas
    {
        [PrimaryKey]
        public int ClaveOpcion { get; set; }
        public int NumOpc { get; set; }

        [MaxLength(160)]
        public string Descripcion { get; set; }
        public int Secuencia { get; set; }

        [MaxLength(60)]
        public string NombreImagen { get; set; }

    }
}
