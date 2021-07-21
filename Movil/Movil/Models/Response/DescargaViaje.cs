using System;
using System.Collections.Generic;

namespace Movil.Models.Response
{
    public class DescargaViaje
    {
        public bool success { get; set; }
        public dvData data { get; set; }
        public int total { get; set; }

        public string message { get; set; }
    }

    public class dvData
    {
        public List<dvNewDataSet> newDataSet { get; set; }
        public List<dvNewDataSet1> newDataSet1 { get; set; }
        public List<dvNewDataSet2> newDataSet2 { get; set; }
        public List<dvNewDataSet3> newDataSet3 { get; set; }
        public List<dvNewDataSet4> newDataSet4 { get; set; }
        public List<dvNewDataSet5> newDataSet5 { get; set; }

        public List<dvNewDataSet6> newDataSet6 { get; set; }
    }


    #region estructura de tablas del viaje seleccionado
    /// <summary>
    /// Datos para almacenar el tabla FolioViaje
    /// </summary>
    public class dvNewDataSet
    {

        public Int32 cfv { get; set; }
        public int ces { get; set; }
        public int cop { get; set; }
        public int cun { get; set; }
        public int car { get; set; }
        public int com { get; set; }

        public DateTime? fpe
        {
            get;
            set;
        }
        public DateTime? fpr
        {
            get;
            set;
        }
        public DateTime? fsp
        {
            get;
            set;
        }
        public DateTime? fsi
        {
            get;
            set;
        }
        public string fvi { get; set; }
        public int noe { get; set; }
        public int par { get; set; }
        public int pro { get; set; }
        public string rut { get; set; }
        public bool sin { get; set; }
        public string uni { get; set; }
    }

    /// <summary>
    /// Datos para almacenar la tabla OrdenEmbarque
    /// </summary>

    public class dvNewDataSet1
    {
        public int coe { get; set; }
        public int cco { get; set; }
        public int ces { get; set; }
        public int cfv { get; set; }
        public int ctd { get; set; }
        public DateTime? fpr
        {
            get;
            set;
        }

        public DateTime? fsi
        {
            get;
            set;

        }
        public string oe { get; set; }
        public bool pro { get; set; }
        public bool sin { get; set; }
        public int tev { get; set; }
        public decimal tpr { get; set; }
        public int tpp { get; set; }
    }

    /// <summary>
    /// Datos para almacenar la tabla OrdenEmbarqueDetalle
    /// </summary>
    public class dvNewDataSet2
    {
        public int cod { get; set; }
        public int csk { get; set; }
        public int coe { get; set; }
        public int ces { get; set; }
        public int ebi { get; set; }
        public string ser { get; set; }
        public int tpr { get; set; }
        public int tpp { get; set; }
    }

    /// <summary>
    /// Datos para almacenar la tabla CatalogoArticulo
    /// </summary>

    public class dvNewDataSet3
    {
        public int csk { get; set; }
        public string des { get; set; }
        public int unm { get; set; }
        public string sku { get; set; }
    }

    /// <summary>
    /// Datos para almacenar la tabla Concesionario
    /// </summary>

    public class dvNewDataSet4
    {
        #region
        private DateTime? zdtpfle = null;
        private DateTime? zdtpfse = null;
        #endregion
        public int cco { get; set; }
        public string nnc { get; set; }
        public int car { get; set; }
        public string dir { get; set; }
        public DateTime? fle
        {
            get
            {
                try
                {
                    if (zdtpfle.Value.Year == 1)
                        zdtpfle = null;
                    return zdtpfle;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                zdtpfle = value;
            }
        }
        public DateTime? fse
        {
            get
            {
                try
                {
                    if (zdtpfse.Value.Year == 1)
                        zdtpfse = null;
                    return zdtpfse;
                }
                catch
                {
                    return null;
                }
            }
            set
            {
                zdtpfse = value;
            }
        }
        public decimal lat { get; set; }
        public decimal lon { get; set; }
        public int nco { get; set; }
        public int ces { get; set; }
        public int tpr { get; set; }
        public int tpp { get; set; }
        public int pro { get; set; }
    }
    /// <summary>
    /// Datos para llenar la tabla encuesta
    /// </summary>    
    public class dvNewDataSet5
    {
        public int cve { get; set; }
        public int ver { get; set; }
        public string ca { get; set; }
        public int n { get; set; }
        public string d { get; set; }
        public string t { get; set; }
        public int o { get; set; }
    }


    public class dvNewDataSet6
    {
        public int cve { get; set; }
        public int opc { get; set; }
        public string dsc { get; set; }
        public int sec { get; set; }
        public string nimg { get; set; }
    }

    #endregion

}

