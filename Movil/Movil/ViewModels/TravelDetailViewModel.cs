using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Helpers;
using Movil.Models;
using Movil.Models.DB;
using Movil.Models.Response;
using Newtonsoft.Json;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class TravelDetailViewModel : ObservableINotifyObject, IQueryAttributable
    {
        public async void ApplyQueryAttributes(IDictionary<string, string> query)
        {
            try
            {
                // The query parameter requires URL decoding.
                if (query.ContainsKey("travel") && query.ContainsKey("nextTravel"))
                {
                    string travel = HttpUtility.UrlDecode(query["travel"]);
                    string nextTravel = HttpUtility.UrlDecode(query["nextTravel"]);

                    App.CurrentTravel = JsonConvert.DeserializeObject<dvNewDataSet>(travel);
                    App.NextTravel = JsonConvert.DeserializeObject<dvNewDataSet>(nextTravel);
                }

                if (App.CurrentTravel != null)
                {
                    await ZDescargarDatos(App.CurrentTravel.cfv);
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        SfPopupLayout popupLayout;

        private string _Progress { get; set; }

        public string Progress
        {
            get { return _Progress; }
            set
            {
                if (_Progress == value)
                    return;

                _Progress = value;
                OnPropertyChanged(nameof(Progress));
            }
        }

        private string _Proximo { get; set; }

        private ConcesionarioResponse _ProximoConcesionario { get; set; }

        public string Proximo
        {
            get { return _Proximo; }
            set
            {
                if (_Proximo == value)
                    return;

                _Proximo = value;
                OnPropertyChanged(nameof(Proximo));
            }
        }

        public ConcesionarioResponse ProximoConcesionario
        {
            get { return _ProximoConcesionario; }
            set
            {
                if (_ProximoConcesionario == value)
                    return;

                _ProximoConcesionario = value;
                OnPropertyChanged(nameof(ProximoConcesionario));
            }
        }

        private string _Ruta { get; set; }

        public string Ruta
        {
            get { return _Ruta; }
            set
            {
                if (_Ruta == value)
                    return;

                _Ruta = value;
                OnPropertyChanged(nameof(Ruta));
            }
        }

        private string _FolioViaje { get; set; }

        public string FolioViaje
        {
            get { return _FolioViaje; }
            set
            {
                if (_FolioViaje == value)
                    return;

                _FolioViaje = value;
                OnPropertyChanged(nameof(FolioViaje));
            }
        }

        private string _Paradas { get; set; }

        public string Paradas
        {
            get { return _Paradas; }
            set
            {
                if (_Paradas == value)
                    return;

                _Paradas = value;
                OnPropertyChanged(nameof(Paradas));
            }
        }

        private string _Pendientes { get; set; }

        public string Pendientes
        {
            get { return _Pendientes; }
            set
            {
                if (_Pendientes == value)
                    return;

                _Pendientes = value;
                OnPropertyChanged(nameof(Pendientes));
            }
        }

        private string _Concluidos { get; set; }

        public string Concluidos
        {
            get { return _Concluidos; }
            set
            {
                if (_Concluidos == value)
                    return;

                _Concluidos = value;
                OnPropertyChanged(nameof(Concluidos));
            }
        }

        private string _Unidad { get; set; }

        public string Unidad
        {
            get { return _Unidad; }
            set
            {
                if (_Unidad == value)
                    return;

                _Unidad = value;
                OnPropertyChanged(nameof(Unidad));
            }
        }

        Grid _GridProximo;

        public TravelDetailViewModel(Grid GridProximo)
        {
            popupLayout = new SfPopupLayout();

            _GridProximo = GridProximo;
        }

        public List<ConcesionarioResponse> MostrarParadas(string zpClaveFolioViaje)
        {
            List<ConcesionarioResponse> concesionarios = new List<ConcesionarioResponse>();

            try
            {
                using (var x = App.zDb)
                {
                    concesionarios = x.Query<ConcesionarioResponse>(@"SELECT  C.ClaveConcesionario, MAX(C.NumeroConcesionario)[NumeroConcesionario] ," +
                                             " MAX(C.NombreConcesionario)[NombreConcesionario] ," +
                                             " MAX(C.Direccion) [Direccion] , " +
                                             " MAX(C.FechaLlegadaEstimada) [FechaLlegadaEstimada], OE.Procesado,C.Latitud, C.Longitud  " +
                                             " FROM    FolioViaje FV " +
                                             " INNER JOIN OrdenEmbarque OE ON OE.ClaveFolioViaje = FV.ClaveFolioViaje " +
                                             " INNER JOIN Concesionario C ON C.ClaveConcesionario = OE.ClaveConcesionario " +
                                             " WHERE   FV.ClaveFolioViaje = ?" +
                                             " GROUP BY C.ClaveConcesionario, OE.Procesado,C.Latitud, C.Longitud" +
                                             " ORDER BY C.FechaLlegadaEstimada ASC; ", zpClaveFolioViaje);
                    x.Dispose();
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }

            return concesionarios;
        }

        public async Task<MensageGenerico> ZDescargarDatos(int folio)
        {
            UserDialogs.Instance.ShowLoading("Cargando...");

            MensageGenerico mensageGenerico = new MensageGenerico();

            try
            {
                ServiceWebClientApp<DescargaViaje> zServWbData = new ServiceWebClientApp<DescargaViaje>();

                #region obtener datos
                BodyRequest zitemRequest = new BodyRequest()
                {
                    data = new Data()
                    {
                        bdCat = "SIDTUM_PROD",
                        bdCc = 22,
                        bdSch = "SidMovil",
                        bdSp = "SPQRY_OrdenViajeDetalleV2",
                        encV = 2
                    },
                    filter = "[{\"property\":\"claveFolioViaje\",\"value\":\"" + folio + "\"}]"
                };
                await zServWbData.BackEndResultAsync(zitemRequest, zEnumMetodoConsumir.querytablesreturn);
                #endregion

                List<ConcesionarioResponse> concesionarios = MostrarParadas(App.zClaveFV.ToString());
                string concluidos = concesionarios.Where(x => x.Procesado == 1).Count().ToString();
                string pendientes = concesionarios.Where(x => x.Procesado == 0).Count().ToString();

                if (concesionarios.Count > 0)
                {
                    ProximoConcesionario = concesionarios.Where(x => x.Procesado == 0).ElementAt(0);

                    Proximo = ProximoConcesionario.ClaveConcesionario + " " + ProximoConcesionario.Direccion + " " + ProximoConcesionario.FechaLlegadaEstimada;

                    TapGestureRecognizer tap = new TapGestureRecognizer();
                    tap.Tapped += async (object sender, EventArgs e) =>
                    {
                        App.zClaveCon = ProximoConcesionario.ClaveConcesionario;

                        await Shell.Current.GoToAsync($"//DetalleServicioView");
                    };

                    _GridProximo.GestureRecognizers.Add(tap);
                }

                if (!zServWbData.zResponseSWApp.zSuccess)
                {
                    if (zServWbData.zResponseSWApp.zMessage == "No tiene conectividad Wi-fi." ||
                        zServWbData.zResponseSWApp.zMessage.IndexOf("Unable to resolve host") >= 0 ||
                        zServWbData.zResponseSWApp.zMessage.IndexOf("operation was canceled") >= 0 ||
                        zServWbData.zResponseSWApp.zMessage.IndexOf("recvfrom failed: ECONNRESET (Connection reset by peer)") >= 0 ||
                        zServWbData.zResponseSWApp.zMessage == "Error de conexion al intentar obtener la llave hacia el servidor")
                    {
                        SQLiteDataFolioViaje zsqlfv = new SQLiteDataFolioViaje();
                        FolioViaje zreg = await zsqlfv.zGetItemAsync(App.zClaveFV);

                        if (zreg != null)
                        {
                            FolioViaje = zreg.FolioDeViaje;
                            Unidad = zreg.Unidad;
                            Ruta = zreg.Ruta;
                            Paradas = zreg.Paradas.ToString();
                            Pendientes = pendientes;
                            Concluidos = concluidos;
                            Progress = (int.Parse(concluidos) / int.Parse(pendientes) * 100).ToString();
                        }
                        else
                        {
                            MostrarError("No hay datos para mostrar.");
                        }
                    }
                    else
                    {
                        MostrarError("Error interno.");
                    }

                    UserDialogs.Instance.HideLoading();

                    return new MensageGenerico
                    {
                        status = zServWbData.zResponseSWApp.zSuccess,
                        Mensaje = zServWbData.zResponseSWApp.zMessage
                    };
                }
                else
                {
                    //descargar datos
                    MensageGenerico zresult = null;

                    #region descarga FolioViaje
                    SQLiteDataFolioViaje zsqlfv = new SQLiteDataFolioViaje();
                    List<FolioViaje> zcioFV = new List<FolioViaje>();
                    //zResponse is null
                    if (zServWbData.zResponseSWApp.zResponse != null)
                    {
                        if (zServWbData.zResponseSWApp.zResponse.data.newDataSet.Count() == 0)
                        {
                            FolioViaje zreg = await zsqlfv.zGetItemAsync(App.zClaveFV);

                            if (zreg != null)
                            {
                                FolioViaje = zreg.FolioDeViaje;
                                Unidad = zreg.Unidad;
                                Ruta = zreg.Ruta;
                                Paradas = zreg.Paradas.ToString();
                                Pendientes = pendientes;
                                Concluidos = concluidos;
                                Progress = (int.Parse(concluidos) / int.Parse(pendientes) * 100).ToString();
                            }
                            else
                            {
                                MostrarError("No hay datos para mostrar.");
                            }

                            return new MensageGenerico
                            {
                                status = zServWbData.zResponseSWApp.zSuccess,
                                Mensaje = zServWbData.zResponseSWApp.zMessage
                            };
                        }
                    }
                    else
                    {
                        FolioViaje zreg = await zsqlfv.zGetItemAsync(App.zClaveFV);

                        if (zreg != null)
                        {
                            FolioViaje = zreg.FolioDeViaje;
                            Unidad = zreg.Unidad;
                            Ruta = zreg.Ruta;
                            Paradas = zreg.Paradas.ToString();
                            Pendientes = pendientes;
                            Concluidos = concluidos;
                            Progress = (int.Parse(concluidos) / int.Parse(pendientes) * 100).ToString();
                        }
                        else
                        {
                            MostrarError("No hay datos para mostrar.");
                        }

                        return new MensageGenerico
                        {
                            status = zServWbData.zResponseSWApp.zSuccess,
                            Mensaje = zServWbData.zResponseSWApp.zMessage
                        };
                    }

                    foreach (var zreg in zServWbData.zResponseSWApp.zResponse.data.newDataSet)
                    {
                        zcioFV.Add(new FolioViaje
                        {
                            ClaveFolioViaje = zreg.cfv,
                            ClaveOperador = zreg.cop,
                            ClaveUnidad = zreg.cun,
                            CantidadArticulo = zreg.car,
                            Completadas = zreg.com,
                            FechaPrimeraEntrega = zreg.fpe,
                            FechaProcesado = zreg.fpr,
                            FechaSalidaProgramada = zreg.fsp,
                            FolioDeViaje = zreg.fvi,
                            NumeroOrdenes = zreg.noe,
                            Paradas = zreg.par,
                            Procesado = zreg.pro,
                            Ruta = zreg.rut,
                            Sincronizado = 1,
                            Unidad = zreg.uni
                        });

                        FolioViaje = zreg.cfv.ToString();
                        Unidad = zreg.cun.ToString();
                        Ruta = zreg.rut;
                        Paradas = zreg.par.ToString();
                        Pendientes = pendientes;
                        Concluidos = concluidos;
                        if (pendientes.Equals("0"))
                        {
                            Progress = "0";
                        }
                        else
                        {
                            double progreso = (double.Parse(concluidos) / double.Parse(pendientes)) * 100;
                            Progress = progreso.ToString();
                        }
                    }
                    zresult = await zsqlfv.zAddItemsAsync(zcioFV);
                    if (zresult.status == false)
                    {
                        UserDialogs.Instance.HideLoading();
                        return zresult;
                    }
                    #endregion

                    #region descarga CatalogoArticulos
                    SQLiteDataArticulo zsqlART = new SQLiteDataArticulo();
                    List<CatalogoArticulo> zcioART = new List<CatalogoArticulo>();
                    zresult = null;
                    foreach (var zreg in zServWbData.zResponseSWApp.zResponse.data.newDataSet3)
                    {
                        zcioART.Add(new CatalogoArticulo
                        {
                            ClaveArticulo = zreg.csk,
                            Descripcion = zreg.des,
                            UnidadMedida = zreg.unm,
                            SKU = zreg.sku
                        });
                    }
                    zresult = await zsqlART.zAddItemsAsync(zcioART);
                    if (zresult.status == false)
                    {
                        UserDialogs.Instance.HideLoading();
                        return zresult;
                    }
                    #endregion

                    #region descarga Concesionarios
                    SQLiteDataConcesionario zsqlCON = new SQLiteDataConcesionario();
                    List<Concesionario> zcioCON = new List<Concesionario>();
                    zresult = null;
                    foreach (var zreg in zServWbData.zResponseSWApp.zResponse.data.newDataSet4)
                    {
                        zcioCON.Add(new Concesionario
                        {
                            ClaveConcesionario = zreg.cco,
                            CantidadArticulo = zreg.car,
                            TotalProcesar = zreg.tpr,
                            TotalProcesado = zreg.tpp,
                            Direccion = zreg.dir,
                            FechaLlegadaEstimada = zreg.fle,
                            FechaSalidaEstimada = zreg.fse,
                            Latitud = zreg.lat,
                            Longitud = zreg.lon,
                            NumeroConcesionario = zreg.nco,
                            NombreConcesionario = zreg.nnc,
                            Sincronizado = 1
                        });
                    }
                    zresult = await zsqlCON.zAddItemsAsync(zcioCON);
                    if (zresult.status == false)
                    {
                        UserDialogs.Instance.HideLoading();
                        return zresult;
                    }
                    #endregion

                    #region descarga OrdenEmbarque
                    SQLiteDataOrdenEmbarque zsqlOE = new SQLiteDataOrdenEmbarque();
                    List<OrdenEmbarque> zcioOE = new List<OrdenEmbarque>();
                    zresult = null;
                    foreach (var zreg in zServWbData.zResponseSWApp.zResponse.data.newDataSet1)
                    {
                        zcioOE.Add(new OrdenEmbarque
                        {
                            ClaveOrdenEmbarque = zreg.coe,
                            ClaveConcesionario = zreg.cco,
                            ClaveFolioViaje = zreg.cfv,
                            ClaveTipoDocumento = zreg.ctd,
                            TotalProcesar = zreg.tpr,
                            TotalEvento = zreg.tev,
                            TotalProcesado = zreg.tpp,
                            FechaProcesado = zreg.fpr,
                            NumOrdenEmbarque = zreg.oe,
                            Procesado = zreg.sin ? 1 : 0,
                            Sincronizado = 1
                        });
                    }
                    zresult = await zsqlOE.zAddItemsAsync(zcioOE);
                    if (zresult.status == false)
                    {
                        UserDialogs.Instance.HideLoading();
                        return zresult;
                    }
                    #endregion

                    #region descarga OrdenEmbarqueDetalle
                    SQLiteDataOrdenEmbarqueDetalle zsqlOED = new SQLiteDataOrdenEmbarqueDetalle();
                    List<OrdenEmbarqueDetalle> zcioOED = new List<OrdenEmbarqueDetalle>();
                    zresult = null;
                    foreach (var zreg in zServWbData.zResponseSWApp.zResponse.data.newDataSet2)
                    {
                        zcioOED.Add(new OrdenEmbarqueDetalle
                        {
                            ClaveOrdenEmbDet = zreg.cod,
                            ClaveArticulo = zreg.csk,
                            ClaveOrdenEmbarque = zreg.coe,
                            EstatusBinario = zreg.ebi,
                            Serie = zreg.ser,
                            TotalProcesar = zreg.tpr,
                            TotalProcesado = zreg.tpp,
                            Sincronizado = 1
                        });
                    }
                    zresult = await zsqlOED.zAddItemsAsync(zcioOED);
                    if (zresult.status == false)
                    {
                        UserDialogs.Instance.HideLoading();
                        return zresult;
                    }
                    #endregion

                    #region descarga Encuesta
                    SQLiteDataEncuesta zsqlENC = new SQLiteDataEncuesta();
                    List<Encuesta> zcioENC = new List<Encuesta>();
                    zresult = null;
                    foreach (var zreg in zServWbData.zResponseSWApp.zResponse.data.newDataSet5)
                    {
                        zcioENC.Add(new Encuesta
                        {
                            ClaveEncuesta = zreg.cve,
                            Version = zreg.ver,
                            TituloOPC = zreg.ca,
                            NumP = zreg.n,
                            Pregunta = zreg.d,
                            TipoOPC = zreg.t,
                            NumOPC = zreg.o
                        });
                    }
                    zresult = await zsqlENC.zAddItemsAsync(zcioENC);
                    if (zresult.status == false)
                    {
                        UserDialogs.Instance.HideLoading();
                        return zresult;
                    }
                    #endregion

                    #region opciones de encuesta
                    SQLiteDataOpcionPreguntas zsqlENCopc = new SQLiteDataOpcionPreguntas();
                    List<OpcionPreguntas> zcioENCopc = new List<OpcionPreguntas>();
                    zresult = null;
                    foreach (var zreg in zServWbData.zResponseSWApp.zResponse.data.newDataSet6)
                    {
                        zcioENCopc.Add(new OpcionPreguntas
                        {
                            ClaveOpcion = zreg.cve,
                            NumOpc = zreg.opc,
                            Descripcion = zreg.dsc,
                            Secuencia = zreg.sec,
                            NombreImagen = zreg.nimg
                        });
                    }
                    zresult = await zsqlENCopc.zAddItemsAsync(zcioENCopc);
                    if (zresult.status == false)
                    {
                        UserDialogs.Instance.HideLoading();
                        return zresult;
                    }
                    #endregion

                    mensageGenerico.status = zServWbData.zResponseSWApp.zSuccess;
                    mensageGenerico.Mensaje = zServWbData.zResponseSWApp.zMessage;
                }

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();

                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });

                mensageGenerico.status = false;
                mensageGenerico.Mensaje = ex.Message;

                MostrarError(ex.Message);
            }

            return mensageGenerico;
        }

        public void PopBack(string mensaje)
        {
            try
            {
                DataTemplate templateView = new DataTemplate(() =>
                {
                    Label popupContent = new Label
                    {
                        Text = mensaje,
                        BackgroundColor = Color.White,
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    return popupContent;
                });

                DataTemplate footerTemplete = new DataTemplate(() =>
                {
                    Grid grid = new Grid();
                    grid.ColumnDefinitions = new ColumnDefinitionCollection
                    {
                        new ColumnDefinition { Width = GridLength.Star },
                        new ColumnDefinition { Width = GridLength.Star }
                    };

                    Button ok = new Button()
                    {
                        Command = new Command(async () => { await Shell.Current.GoToAsync("/travels"); Shell.Current.FlyoutIsPresented = false; }),
                        Text = "SI",
                        FontSize = 12,
                        BackgroundColor = Color.LightGray,
                        TextColor = Color.Black,
                        Margin = new Thickness(10),

                    };

                    Button back = new Button()
                    {
                        Command = new Command(() => popupLayout.IsOpen = false),
                        Text = "NO",
                        FontSize = 12,
                        BackgroundColor = Color.LightGray,
                        TextColor = Color.Black,
                        Margin = new Thickness(10),

                    };

                    grid.Children.Add(back);
                    Grid.SetColumn(back, 0);

                    grid.Children.Add(ok);
                    Grid.SetColumn(ok, 1);

                    return grid;
                });

                popupLayout.PopupView.ContentTemplate = templateView;
                popupLayout.PopupView.HeightRequest = 200;
                popupLayout.PopupView.WidthRequest = 200;
                popupLayout.PopupView.FooterTemplate = footerTemplete;
                popupLayout.PopupView.HeaderTitle = "";
                popupLayout.StaysOpen = true;
                popupLayout.Show();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public void MostrarError(string mensaje)
        {
            try
            {
                DataTemplate templateView = new DataTemplate(() =>
                {
                    Label popupContent = new Label
                    {
                        Text = mensaje,
                        BackgroundColor = Color.Red,
                        TextColor = Color.White,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    return popupContent;
                });

                popupLayout.PopupView.ContentTemplate = templateView;
                popupLayout.PopupView.HeightRequest = 200;
                popupLayout.PopupView.WidthRequest = 200;
                popupLayout.PopupView.ShowFooter = false;
                popupLayout.PopupView.ShowHeader = true;
                popupLayout.PopupView.HeaderTitle = "";
                popupLayout.StaysOpen = true;
                popupLayout.Show();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }
    }
}