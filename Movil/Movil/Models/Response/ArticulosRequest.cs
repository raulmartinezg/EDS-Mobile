namespace Movil.Models.Response
{
    public class ArticulosRequest
    {
        public int ClaveArticulo { get; set; }

        public string Descripcion { get; set; }

        public string UnidadMedida { get; set; }

        public string SKU { get; set; }

        public int ClaveOrdenEmbDet { get; set; }

        public int TotalProcesar { get; set; }

        public int TotalProcesado { get; set; }

        public string Entregado { get; set; }
    }
}
