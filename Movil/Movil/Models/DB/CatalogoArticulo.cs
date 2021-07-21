using SQLite;

namespace Movil
{
    public class CatalogoArticulo
    {
        [PrimaryKey]
        public int ClaveArticulo { get; set; }

        [MaxLength(160)]
        public string Descripcion { get; set; }

        public int UnidadMedida { get; set; }

        [MaxLength(50)]
        public string SKU { get; set; }
    }
}
