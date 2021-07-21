using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Models;
using Movil.Models.DB;
using Movil.Models.Response;
using Movil.Models.ResultQry;
using Movil.Services;
using Newtonsoft.Json;
using Plugin.SimpleAudioPlayer;
using Syncfusion.SfNumericUpDown.XForms;
using Syncfusion.XForms.Cards;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Input;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class DeliveriesViewModel : ObservableINotifyObject
    {
        public ObservableCollection<ArticulosRequest> CatalogoSource { get; set; }

        private SumarioEscaneo zvarSumEsc = new SumarioEscaneo();
        private SQLiteDataConcesionario DSConcesionario = new SQLiteDataConcesionario();
        private Concesionario _zvarConcesionario { get; set; }
        public Concesionario zvarConcesionario
        {
            get { return _zvarConcesionario; }
            set
            {
                if (_zvarConcesionario == value)
                    return;

                _zvarConcesionario = value;
                OnPropertyChanged(nameof(zvarConcesionario));
            }
        }

        SfPopupLayout popupLayout;

        private string _Piezas { get; set; }

        private string _Contenedores { get; set; }

        private string _Cajas { get; set; }

        private string _Total { get; set; }

        private string _SKU { get; set; }

        private string _Descripcion { get; set; }

        private string _Cantidad { get; set; }

        private string _Maximum { get; set; }

        private string _Egresar { get; set; }

        private bool _CantidadVisible { get; set; }

        private bool _EgresarVisible { get; set; }

        private string _Disponibles { get; set; }


        private IBarcodeReader _barcodeReader;

        public ICommand EgresarCommand { get; set; }

        public string Piezas
        {
            get { return _Piezas; }
            set
            {
                if (_Piezas == value)
                    return;

                _Piezas = value;
                OnPropertyChanged(nameof(Piezas));
            }
        }

        public string Cajas
        {
            get { return _Cajas; }
            set
            {
                if (_Cajas == value)
                    return;

                _Cajas = value;
                OnPropertyChanged(nameof(Cajas));
            }
        }

        public string Contenedores
        {
            get { return _Contenedores; }
            set
            {
                if (_Contenedores == value)
                    return;

                _Contenedores = value;
                OnPropertyChanged(nameof(Contenedores));
            }
        }

        public string Total
        {
            get { return _Total; }
            set
            {
                if (_Total == value)
                    return;

                _Total = value;
                OnPropertyChanged(nameof(Total));
            }
        }

        public IBarcodeReader BarcodeReader
        {
            get => _barcodeReader;
            set
            {
                _barcodeReader = value;
                OnPropertyChanged(nameof(BarcodeReader));
            }
        }

        public SumarioEscaneo zpropSumTotales
        {
            get { return zvarSumEsc; }
            set
            {
                zvarSumEsc = value;
                OnPropertyChanged(nameof(zpropSumTotales));
            }
        }

        public string SKU
        {
            get { return _SKU; }
            set
            {
                if (_SKU == value)
                    return;

                _SKU = value;
                OnPropertyChanged(nameof(SKU));
            }
        }

        public string Descripcion
        {
            get { return _Descripcion; }
            set
            {
                if (_Descripcion == value)
                    return;

                _Descripcion = value;
                OnPropertyChanged(nameof(Descripcion));
            }
        }

        public string Cantidad
        {
            get { return _Cantidad; }
            set
            {
                if (_Cantidad == value)
                    return;

                _Cantidad = value;
                OnPropertyChanged(nameof(Cantidad));
            }
        }

        public string Maximum
        {
            get { return _Maximum; }
            set
            {
                if (_Maximum == value)
                    return;

                _Maximum = value;
                OnPropertyChanged(nameof(Maximum));
            }
        }

        public string Egresar
        {
            get { return _Egresar; }
            set
            {
                if (_Egresar == value)
                    return;

                _Egresar = value;
                OnPropertyChanged(nameof(Egresar));
            }
        }

        public bool CantidadVisible
        {
            get { return _CantidadVisible; }
            set
            {
                if (_CantidadVisible == value)
                    return;

                _CantidadVisible = value;
                OnPropertyChanged(nameof(CantidadVisible));
            }
        }

        public bool EgresarVisible
        {
            get { return _EgresarVisible; }
            set
            {
                if (_EgresarVisible == value)
                    return;

                _EgresarVisible = value;
                OnPropertyChanged(nameof(EgresarVisible));
            }
        }

        public string Disponibles
        {
            get { return _Disponibles; }
            set
            {
                if (_Disponibles == value)
                    return;

                _Disponibles = value;
                OnPropertyChanged(nameof(Disponibles));
            }
        }

        SfCardView _GridSKU;

        SfNumericUpDown _numericUpDown;

        private ISimpleAudioPlayer _simpleAudioPlayer;

        public DeliveriesViewModel(SfCardView GridSKU, SfNumericUpDown numericUpDown)
        {
            UserDialogs.Instance.ShowLoading("Cargando...");

            popupLayout = new SfPopupLayout();

            _GridSKU = GridSKU;

            _numericUpDown = numericUpDown;
            _numericUpDown.ValueChangeMode = ValueChangeMode.OnKeyFocus;
            _numericUpDown.ValueChanged += Handle_ValueChanged;


            EgresarCommand = new Command(async () => await EgresarArticulo());

            CatalogoSource = GetArticulos();

            GetDeliveries();

            CantidadVisible = false;

            EgresarVisible = false;

            UserDialogs.Instance.HideLoading();
        }
        private async Task zConsultaConcecionario(int cveConcecionario)
        {
            zvarConcesionario = DSConcesionario.zGetItem(cveConcecionario);
        }

        void Handle_ValueChanged(object sender, Syncfusion.SfNumericUpDown.XForms.ValueEventArgs e)
        {
            StringBuilder sb = new StringBuilder();
            _ = sb.AppendFormat("Disponibles: {0}/{1}", e.Value.ToString(), Maximum);

            Disponibles = sb.ToString();
        }

        public async Task EgresarArticulo()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Cargando...");

                if (Egresar.Equals("Fin de la entrega"))
                {
                    await Shell.Current.GoToAsync("//CerrarEntrega");
                }
                else
                {
                    _ = int.TryParse(Cantidad, out int _cantidad);

                    if (!string.IsNullOrEmpty(SKU))
                    {
                        ArticulosRequest articulo = CatalogoSource.Where(x => x.SKU.Equals(SKU)).FirstOrDefault();

                        SQLiteDataOrdenEmbarqueDetalle sql = new SQLiteDataOrdenEmbarqueDetalle();
                        OrdenEmbarqueDetalle ordenEmbarqueDetalle = await sql.zGetItemAsync(articulo.ClaveOrdenEmbDet);
                        ordenEmbarqueDetalle.TotalProcesado += _cantidad;
                        ordenEmbarqueDetalle.Sincronizado = 0;
                        ordenEmbarqueDetalle.Manual = 0;
                        bool detalleActualizado = await sql.zUpdateItemAsync(ordenEmbarqueDetalle);

                        SQLiteDataOrdenEmbarque ordenEmbarque = new SQLiteDataOrdenEmbarque();
                        OrdenEmbarque orden = await ordenEmbarque.zGetItemAsync(ordenEmbarqueDetalle.ClaveOrdenEmbarque);
                        orden.Sincronizado = 0;
                        bool ordenActualizado = await ordenEmbarque.zUpdateItemAsync(orden);

                        SQLiteDataFolioViaje folioViaje = new SQLiteDataFolioViaje();
                        FolioViaje folio = await folioViaje.zGetItemAsync(orden.ClaveFolioViaje);
                        folio.Sincronizado = 0;
                        bool folioActualizado = await folioViaje.zUpdateItemAsync(folio);

                        CatalogoSource = GetArticulos();

                        GetDeliveries();

                        int porProcesar = articulo.TotalProcesar - (articulo.TotalProcesado + _cantidad);

                        if (porProcesar == 0)
                        {
                            if (Egresar.Equals("Fin de la entrega"))
                            {
                                EgresarVisible = true;
                                Sonido("Movil.Media.Final.wav");
                            }
                            else
                            {
                                EgresarVisible = false;
                            }
                            CantidadVisible = false;
                        }
                        else
                        {
                            SKU = articulo.SKU;
                            Descripcion = articulo.Descripcion;
                            Maximum = porProcesar.ToString();

                            StringBuilder sb = new StringBuilder();
                            _ = sb.AppendFormat("Disponibles: {0}/{1}", articulo.TotalProcesado, porProcesar.ToString());

                            Disponibles = sb.ToString();

                            CantidadVisible = true;
                            EgresarVisible = true;
                            Sonido("Movil.Media.Multiplicador.wav");
                        }
                        sql.Dispose();
                        
                    }
                    else
                    {
                        MostrarError("Debe leer un artículo");
                    }
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
                UserDialogs.Instance.HideLoading();

                MostrarError(ex.Message);
            }
            
            

            UserDialogs.Instance.HideLoading();
        }

        public ObservableCollection<ArticulosRequest> GetArticulos()
        {
            ObservableCollection<ArticulosRequest> articulosRequests;

            try
            {
                using (var x = App.zDb)
                {
                    string zcmd = "SELECT CA.ClaveArticulo," +
                                   " CA.Descripcion," +
                                   " CA.UnidadMedida," +
                                   " CASE WHEN CA.UnidadMedida = 1 THEN OED.Serie ELSE CA.SKU END AS SKU, " +
                                   " OED.TotalProcesar, " +
                                   " OED.ClaveOrdenEmbDet, " +
                                   " OED.TotalProcesado " +
                                  " FROM OrdenEmbarque OE " +
                                  " INNER JOIN OrdenEmbarqueDetalle OED ON OED.ClaveOrdenEmbarque = OE.ClaveOrdenEmbarque " +
                                  " INNER JOIN CatalogoArticulo CA ON CA.ClaveArticulo = OED.ClaveArticulo " +
                                  " WHERE OE.ClaveFolioViaje = ?  " +
                                  " AND OE.ClaveConcesionario = ?";

                    var zresu = x.Query<ArticulosRequest>(zcmd, App.zClaveFV.ToString(), App.zClaveCon);
                    if (zresu.Count >= 1)
                    {
                        articulosRequests = new ObservableCollection<ArticulosRequest>(zresu);
                        
                    }
                    else
                    {
                        articulosRequests = new ObservableCollection<ArticulosRequest>();
                        
                    }
                    x.Dispose();
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });

                articulosRequests = new ObservableCollection<ArticulosRequest>();

                MostrarError(ex.Message);
            }

            return articulosRequests;
        }

        public void GetDeliveries()
        {
            try
            {
                using (var x = App.zDb)
                {
                    string zcmd = " SELECT SQ.ClaveConcesionario, " +
                                   " CAST(SUM(SQ.TotalProcesado) AS NVARCHAR)  || '/' || CAST(SUM(SQ.TotalProcesar) AS NVARCHAR) AS \"Total\", " +
                                   " CAST(SUM(SQ.CajaFalta) AS NVARCHAR) || '/' || CAST(SUM(SQ.CajaTotal) AS NVARCHAR) AS \"Caja\", " +
                                   " CAST(SUM(SQ.ContFalta) AS NVARCHAR) || '/' || CAST(SUM(SQ.ContTotal)AS NVARCHAR)  AS \"Contenedor\", " +
                                   " CAST(SUM(SQ.PiezaFalta) AS NVARCHAR) || '/' || CAST(SUM(SQ.PiezaTotal)AS NVARCHAR) AS \"Piezas\", " +
                                   " SUM(SQ.TotalProcesar)- SUM(SQ.TotalProcesado) AS \"PorProcesar\" " +
                                   " FROM    (" +
                                   " SELECT OE.ClaveConcesionario," +
                                   " SUM(OED.TotalProcesar) AS \"TotalProcesar\", " +
                                   " SUM(OED.TotalProcesado) AS \"TotalProcesado\"," +
                                   " CASE WHEN CA.UnidadMedida = 1 THEN SUM(OED.TotalProcesar) ELSE 0 END AS \"CajaTotal\", " +
                                   " CASE WHEN CA.UnidadMedida = 1 THEN SUM(OED.TotalProcesado) ELSE 0 END AS \"CajaFalta\", " +
                                   " CASE WHEN CA.UnidadMedida = 2 THEN SUM(OED.TotalProcesar) ELSE 0 END AS \"ContTotal\", " +
                                   " CASE WHEN CA.UnidadMedida = 2 THEN SUM(OED.TotalProcesado) ELSE 0 END AS \"ContFalta\", " +
                                   " CASE WHEN CA.UnidadMedida = 3 THEN SUM(OED.TotalProcesar) ELSE 0 END AS \"PiezaTotal\", " +
                                   " CASE WHEN CA.UnidadMedida = 3 THEN SUM(OED.TotalProcesado) ELSE 0 END AS \"PiezaFalta\" " +
                                  " FROM   OrdenEmbarque OE " +
                                  " INNER JOIN OrdenEmbarqueDetalle OED ON OED.ClaveOrdenEmbarque = OE.ClaveOrdenEmbarque " +
                                  " INNER JOIN CatalogoArticulo CA ON CA.ClaveArticulo = OED.ClaveArticulo " +
                                  " WHERE OE.ClaveFolioViaje = ?  " +
                                  " AND OE.ClaveConcesionario = ?" +
                                  " GROUP BY      OE.ClaveConcesionario, CA.UnidadMedida " +
                                  " ) [SQ] GROUP BY SQ.ClaveConcesionario ";

                    var zresu = x.Query<SumarioEscaneo>(zcmd, App.zClaveFV.ToString(), App.zClaveCon);
                    if (zresu.Count >= 1)
                    {
                        zpropSumTotales = zresu[0];
                        Piezas = "Piezas " + zpropSumTotales.Piezas;
                        Contenedores = "Contenedores " + zpropSumTotales.Contenedor;
                        Cajas = "Cajas " + zpropSumTotales.Caja;
                        Total = "Total " + zpropSumTotales.Total;

                        Cantidad = "1";

                        if (zpropSumTotales.PorProcesar == 0)
                        {
                            SKU = "";
                            Descripcion = "";
                            Disponibles = "";
                            EgresarVisible = true;
                            Egresar = "Fin de la entrega";
                        }
                        else
                        {
                            Egresar = "Egreso";
                        }
                    }x.Dispose();
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });

                MostrarError(ex.Message);
            }
        }

        public ObservableCollection<ArticuloObject> GetServices()
        {
            ObservableCollection<ArticuloObject> articulosRequests;

            try
            {
                using (var x = App.zDb)
                {
                    string zcmd = "SELECT CA.ClaveArticulo csk, " +
                                   " OED.TotalProcesar tpr, " +
                                   " OED.ClaveOrdenEmbDet cod, " +
                                   " OED.ClaveOrdenEmbarque coe, " +
                                   " OED.EstatusBinario ebi, " +
                                   " OED.TotalProcesado tpp " +
                                  " FROM OrdenEmbarque OE " +
                                  " INNER JOIN OrdenEmbarqueDetalle OED ON OED.ClaveOrdenEmbarque = OE.ClaveOrdenEmbarque " +
                                  " INNER JOIN CatalogoArticulo CA ON CA.ClaveArticulo = OED.ClaveArticulo " +
                                  " WHERE OE.ClaveFolioViaje = ?  " +
                                  " AND OE.ClaveConcesionario = ?";

                    var zresu = x.Query<ArticuloObject>(zcmd, App.zClaveFV.ToString(), App.zClaveCon);
                    if (zresu.Count >= 1)
                    {
                        zresu = zresu.Where(z => z.tpr == z.tpp).ToList();
                        articulosRequests = new ObservableCollection<ArticuloObject>(zresu);
                    }
                    else
                    {
                        articulosRequests = new ObservableCollection<ArticuloObject>();
                    }x.Dispose();
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });

                articulosRequests = new ObservableCollection<ArticuloObject>();
                MostrarError(ex.Message);
            }

            return articulosRequests;
        }

        public async Task Initialize()
        {
            

            try
            {
                await zConsultaConcecionario(App.zClaveCon);

                await Stop();

                BarcodeReader = DependencyService.Get<IBarcodeReader>();

                if (null != BarcodeReader)
                {
                    BarcodeReader.BarcodeDataReady += BarcodeReader_BarcodeDataReady;

                    await BarcodeReader.Initialize();
                    _ = await BarcodeReader.StartBarcodeReader();
                }

                CatalogoSource = GetArticulos();
                GetDeliveries();
               
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public async Task Stop()
        {
            try
            {
                if (null != BarcodeReader)
                {
                    BarcodeReader.BarcodeDataReady -= BarcodeReader_BarcodeDataReady;
                    bool exito = await BarcodeReader.StopBarcodeReader();
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public void PopExito(string mensaje)
        {
            try
            {
                DataTemplate templateView = new DataTemplate(() =>
                {
                    Label popupContent = new Label
                    {
                        Text = mensaje,
                        BackgroundColor = Color.LightGreen,
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    return popupContent;
                });

                DataTemplate footerTemplete = new DataTemplate(() =>
                {
                    Grid grid = new Grid();
                    grid.Children.Add(new Button()
                    {
                        Command = new Command(async () => await Shell.Current.GoToAsync("//DetalleServicioView")),
                        Text = "Ok",
                        BackgroundColor = Color.LightGray,
                        TextColor = Color.Black,
                        Margin = new Thickness(10)
                    });
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

        public async Task PopError(string mensaje)
        {
            try
            {
                DataTemplate templateView = new DataTemplate(() =>
                {
                    Label popupContent = new Label
                    {
                        Text = mensaje,
                        BackgroundColor = Color.IndianRed,
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    };
                    return popupContent;
                });

                DataTemplate footerTemplete = new DataTemplate(() =>
                {
                    Grid grid = new Grid();
                    grid.Children.Add(new Button()
                    {
                        Command = new Command(async () => await Shell.Current.GoToAsync("//CerrarEntrega")),
                        Text = "Ok",
                        BackgroundColor = Color.LightGray,
                        TextColor = Color.Black,
                        Margin = new Thickness(10)
                    });
                    return grid;
                });

                popupLayout.PopupView.ContentTemplate = templateView;
                popupLayout.PopupView.HeightRequest = 200;
                popupLayout.PopupView.WidthRequest = 200;
                popupLayout.PopupView.FooterTemplate = footerTemplete;
                popupLayout.PopupView.ShowHeader = false;
                popupLayout.StaysOpen = true;
                popupLayout.Show();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
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
                        Command = new Command(async () => { await Shell.Current.GoToAsync("//CerrarEntrega"); Shell.Current.FlyoutIsPresented = false; }),
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

        private void BarcodeReader_BarcodeDataReady(object sender, BarcodeDataArgs e)
        {
            try
            {
                Device.BeginInvokeOnMainThread(async () =>
                {
                    ArticulosRequest articulo = CatalogoSource.Where(x => x.SKU.Equals(e.Data)).FirstOrDefault();

                    if (articulo != null)
                    {
                        Timer timer = new Timer
                        {
                            Interval = 1,
                            Enabled = false
                        };

                        timer.Elapsed += CambiaColor;
                        timer.Start();

                        int porProcesar = articulo.TotalProcesar - articulo.TotalProcesado;

                        if (porProcesar == 0)
                        {
                            Sonido("Movil.Media.Egresado.wav");
                            MostrarError("Artículo ya egresado");
                        }
                        else
                        {
                            if (articulo.TotalProcesado < articulo.TotalProcesar)
                            {
                                SKU = articulo.SKU;
                                Descripcion = articulo.Descripcion;
                                Maximum = porProcesar.ToString();

                                StringBuilder sb = new StringBuilder();
                                _ = sb.AppendFormat("Disponibles: {0}/{1}", articulo.TotalProcesado == 0 ? 1 : articulo.TotalProcesado, porProcesar.ToString());

                                Disponibles = sb.ToString();

                                await EgresarArticulo();
                            }
                            else
                            {
                                Sonido("Movil.Media.Maximo.wav");
                                MostrarError("Número maximo de artículos alcanzado");
                            }
                        }
                        timer.Stop();
                        timer.Elapsed -= CambiaColor;
                    }
                    else
                    {
                        Sonido("Movil.Media.Error.wav");
                        MostrarError("Artículo no encontrado");
                    }
                });
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public void CambiaColor(object sender, ElapsedEventArgs e)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                if (_GridSKU.BackgroundColor == Color.GreenYellow)
                {
                    _GridSKU.BackgroundColor = Color.IndianRed;
                }
                else
                {
                    _GridSKU.BackgroundColor = Color.GreenYellow;
                }
            });
        }

        public void Sonido(string archivo)
        {
            _simpleAudioPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            Stream beepStream = GetType().Assembly.GetManifestResourceStream(archivo);
            bool isSuccess = _simpleAudioPlayer.Load(beepStream);
            _simpleAudioPlayer.Play();
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
