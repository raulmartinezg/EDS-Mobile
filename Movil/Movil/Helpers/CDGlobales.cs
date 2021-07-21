using System;

namespace Movil.Helpers
{
    public class CDGlobales
    {
        public static bool BoolAlternaColorCaptura;

        public const string zURLLogin = "https://portal.tum.com.mx/DatumRestMasKota/api/loginsp";

        public const string zURLViajes = "https://portal.tum.com.mx/DatumRestMasKota/api/querytablesreturn";

        public const string zURLMetGen_Request = "https://portal.tum.com.mx/DatumRestMasKota/api/";

        public const string zURLOrdenEmbarqueDetalle = "http://200.52.73.110:8085/NSNAPI/api/OrdenEmbarqueDetalle";

        public const string zURLIncidencia = "http://200.52.73.110:8085/NSNAPI/api/Incidencia";

        public const string zURLMaster = "http://200.52.73.110:8085/NSNAPI/api/Master";

        public static string ZExhorto { get; set; }

        public static int ZEdoEstBin(int estatus, int posicion, Boolean valor)
        {
            var valorActual = estatus & (int)Math.Pow(2, posicion);

            if (valor && valorActual == 0)
            {
                return estatus + (int)Math.Pow(2, posicion);
            }
            if (!valor && valorActual > 0)
            {
                return estatus - (int)Math.Pow(2, posicion);
            }
            return estatus;
        }
    }
}
