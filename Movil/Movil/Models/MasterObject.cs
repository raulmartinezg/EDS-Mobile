using System.Collections.Generic;

namespace Movil.Models
{
    public class MasterObject
    {
        public List<IncidenciaObject> incidencias { get; set; }

        public List<ArticuloObject> articulos { get; set; }

        public List<EncuestaObject> encuestas { get; set; }

        public RegistroObject entrada { get; set; }

        public RegistroObject salida { get; set; }

        public bool sincronizado { get; set; }
    }
}
