using Movil.Models.ResultQry;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Movil.searchs
{
    public class MSearch
    {

        /// <summary>
        /// Muestra los articulos de un servicio o parada
        /// </summary>
        /// <param name="cfv">clavefolioviaje</param>
        /// <param name="idparada">claveparada</param>
        /// <param name="procesado">0=muestra los sku no recibidos | 1=muestra los sku ya recibidos</param>
        /// <returns></returns>
        /// 
        public ObservableCollection<HeadArticulosEnServicio> zMuestraSKU_aEntregarXServicio(string cfv, string idparada, string procesado)
        {
            IList<HeadArticulosEnServicio> zresultbusq;
            using (var x = App.zDb)
            {
                string zcmd = " SELECT OED.ClaveArticulo, COUNT(OED.ClaveArticulo) AS Coincidencias, " +
                               " SUM(OED.TotalProcesar) AS TotalProcesar,SUM(OED.TotalProcesado) AS TotalProcesado, " +
                               " CASE WHEN CA.UnidadMedida = 1  THEN OED.Serie ELSE CA.SKU END AS SKU, " +
                               " SUM(OED.TotalProcesar) - SUM(OED.TotalProcesado) AS PorProcesar, " +
                               " CASE WHEN MAX(CA.UnidadMedida) = 1 THEN 'Caja'  " +
                               " WHEN MAX(CA.UnidadMedida) = 2 THEN 'Contenedor'  " +
                               " WHEN MAX(CA.UnidadMedida) = 3 THEN 'Pieza' ELSE 'N/D' END AS UM, " +
                               " OE.FechaProcesado FechaProcesadoOE, OE.ClaveOrdenEmbarque, OE.NumOrdenEmbarque " +
                               " FROM Concesionario C " +
                               " INNER JOIN OrdenEmbarque OE ON OE.ClaveConcesionario = C.ClaveConcesionario " +
                               " INNER JOIN OrdenEmbarqueDetalle OED ON OED.ClaveOrdenEmbarque = OE.ClaveOrdenEmbarque " +
                               " INNER JOIN CatalogoArticulo CA ON CA.ClaveArticulo = OED.ClaveArticulo " +
                               " WHERE OE.ClaveFolioViaje = ? " +
                               " AND C.ClaveConcesionario = ? " +
                               " AND OED.Procesado = ? " +
                               " GROUP BY OED.ClaveArticulo,CA.UnidadMedida ,OED.Serie,CA.SKU,OE.FechaProcesado,OE.ClaveOrdenEmbarque,OE.NumOrdenEmbarque ";

                zresultbusq = x.Query<HeadArticulosEnServicio>(zcmd, cfv, idparada, procesado);
            }
            return new ObservableCollection<HeadArticulosEnServicio>(zresultbusq);
        }

        //private Task<ObservableCollection<T>> ObservableCollection<T>(IList<T> zresultbusq)
        //{
        //    throw new NotImplementedException();
        //}

        /// <summary>
        /// Busca las OE donde esta  el SKU pendiente de entregar en la parada actual
        /// </summary>
        /// <param name="cfv">clavefolioviaje</param>
        /// <param name="idparada">claveparada</param>        
        /// <param name="zsku"></param>
        /// <returns></returns>
        public List<HeadArticulosEnServicio> zBuscaSKUenOE_xParada(string cfv, string idparada, string zsku)
        {
            List<HeadArticulosEnServicio> zresultbusq = null;
            using (var x = App.zDb)
            {
                string zcmd = " SELECT OED.ClaveArticulo, COUNT(OED.ClaveArticulo) AS Coincidencias, " +
                               " SUM(OED.TotalProcesar) AS TotalProcesar,SUM(OED.TotalProcesado) AS TotalProcesado, " +
                               " CASE WHEN CA.UnidadMedida = 1  THEN OED.Serie ELSE CA.SKU END AS SKU, " +
                               " SUM(OED.TotalProcesar) - SUM(OED.TotalProcesado) AS PorProcesar, " +
                               " CASE WHEN MAX(CA.UnidadMedida) = 1 THEN 'Caja'  " +
                               " WHEN MAX(CA.UnidadMedida) = 2 THEN 'Contenedor'  " +
                               " WHEN MAX(CA.UnidadMedida) = 3 THEN 'Pieza' ELSE 'N/D' END AS UM, " +
                               " OE.FechaProcesado FechaProcesadoOE, OE.ClaveOrdenEmbarque, OE.NumOrdenEmbarque " +
                               " FROM Concesionario C " +
                               " INNER JOIN OrdenEmbarque OE ON OE.ClaveConcesionario = C.ClaveConcesionario " +
                               " INNER JOIN OrdenEmbarqueDetalle OED ON OED.ClaveOrdenEmbarque = OE.ClaveOrdenEmbarque " +
                               " INNER JOIN CatalogoArticulo CA ON CA.ClaveArticulo = OED.ClaveArticulo " +
                               " WHERE OE.ClaveFolioViaje = ? " +
                               " AND C.ClaveConcesionario = ? " +
                               " AND (OED.Serie = '" + zsku + "' OR CA.SKU = '" + zsku + "') " +
                               " GROUP BY OED.ClaveArticulo,CA.UnidadMedida ,OED.Serie,CA.SKU,OE.FechaProcesado,OE.ClaveOrdenEmbarque,OE.NumOrdenEmbarque ";

                zresultbusq = x.Query<HeadArticulosEnServicio>(zcmd, cfv, idparada);
            }
            return zresultbusq;
        }

        /// <summary>
        /// Obtiene el detalle del articulo que sera registrado como entrega o rechazo.
        /// </summary>
        /// <param name="cfv">clavefolioviaje</param>
        /// <param name="cveOE">coleccion de claveordenembarque</param>
        /// <param name="zsku">serie o sku</param>
        /// <returns></returns>
        public List<DetArticuloEnServicio> zObtieneDetOE_SKU(string cfv, string zOEs, string zsku)
        {

            using (var x = App.zDb)
            {
                string zcmd = " SELECT OE.ClaveOrdenEmbarque, OED.ClaveOrdenEmbDet, OED.ClaveArticulo, " +
                                " OED.TotalProcesar, OED.TotalProcesado, " +
                                " OED.TotalProcesar-OED.TotalProcesado AS PorProcesar, " +
                                " OED.EstatusBinario, OED.Procesado, " +
                                " CASE WHEN CA.UnidadMedida = 1 THEN 'Caja' WHEN CA.UnidadMedida = 2 THEN 'Contenedor' WHEN CA.UnidadMedida = 3 THEN 'Pieza' ELSE 'N/D' END AS UM, " +
                                " OE.NumOrdenEmbarque " +
                                " FROM OrdenEmbarque OE " +
                                " INNER JOIN OrdenEmbarqueDetalle OED ON OED.ClaveOrdenEmbarque = OE.ClaveOrdenEmbarque " +
                                " INNER JOIN CatalogoArticulo CA ON CA.ClaveArticulo = OED.ClaveArticulo " +
                                " WHERE oe.ClaveFolioViaje =? " +
                                " AND OED.ClaveOrdenEmbarque in (" + zOEs + ")" +
                                " AND (OED.Serie = '" + zsku + "' OR CA.SKU = '" + zsku + "') ";
                var zresultbusq = x.Query<DetArticuloEnServicio>(zcmd, cfv);
                return zresultbusq;
            }
        }
    }
}
