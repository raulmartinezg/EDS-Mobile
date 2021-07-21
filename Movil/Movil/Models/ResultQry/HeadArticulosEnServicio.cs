using System;

namespace Movil.Models.ResultQry
{
    public class HeadArticulosEnServicio
    {
        private DateTime? zFechaProcOE = null;
        public int ClaveArticulo { get; set; }
        public int Coincidencias { get; set; }
        public int TotalProcesar { get; set; }
        public int TotalProcesado { get; set; }
        public string SKU { get; set; }
        public int PorProcesar { get; set; }
        public string UM { get; set; }
        public DateTime? FechaProcesadoOE
        {
            get
            {
                try
                {
                    if (zFechaProcOE.Value.Year == 1)
                        zFechaProcOE = null;

                    return zFechaProcOE;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                zFechaProcOE = value;
            }
        }
        public int ClaveOrdenEmbarque { get; set; }
        public string NumOrdenEmbarque { get; set; }

    }
}
