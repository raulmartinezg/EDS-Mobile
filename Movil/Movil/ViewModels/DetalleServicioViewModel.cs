using Movil.DataSQL;
using Movil.Models;
using Movil.Models.DB;
using Newtonsoft.Json;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class DetalleServicioViewModel : ObservableINotifyObject
    {
        private SQLiteDataConcesionario DSConcesionario = new SQLiteDataConcesionario();

        private Concesionario _zvarConcesionario { get; set; }

        public ObservableCollection<OrdenEmbarque> zListOrdenEmbarque { get; set; }

        public string zExhorto { get; set; }

        SfPopupLayout popupLayout;

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

        private ImageSource _EntregasImagen { get; set; }

        private ImageSource _EntradaImagen { get; set; }

        private ImageSource _IncidentesImagen { get; set; }

        private ImageSource _EncuestaImagen { get; set; }

        private string _EntregasNumero { get; set; }

        private string _EntradaNumero { get; set; }

        private string _IncidentesNumero { get; set; }

        private string _EncuestaNumero { get; set; }

        public ImageSource EntregasImagen
        {
            get { return _EntregasImagen; }
            set
            {
                if (_EntregasImagen == value)
                    return;

                _EntregasImagen = value;
                OnPropertyChanged(nameof(EntregasImagen));
            }
        }

        public ImageSource EntradaImagen
        {
            get { return _EntradaImagen; }
            set
            {
                if (_EntradaImagen == value)
                    return;

                _EntradaImagen = value;
                OnPropertyChanged(nameof(EntradaImagen));
            }
        }

        public ImageSource IncidentesImagen
        {
            get { return _IncidentesImagen; }
            set
            {
                if (_IncidentesImagen == value)
                    return;

                _IncidentesImagen = value;
                OnPropertyChanged(nameof(IncidentesImagen));
            }
        }

        public ImageSource EncuestaImagen
        {
            get { return _EncuestaImagen; }
            set
            {
                if (_EncuestaImagen == value)
                    return;

                _EncuestaImagen = value;
                OnPropertyChanged(nameof(EncuestaImagen));
            }
        }

        public string EntregasNumero
        {
            get { return _EntregasNumero; }
            set
            {
                if (_EntregasNumero == value)
                    return;

                _EntregasNumero = value;
                OnPropertyChanged(nameof(EntregasNumero));
            }
        }

        public string EntradaNumero
        {
            get { return _EntradaNumero; }
            set
            {
                if (_EntradaNumero == value)
                    return;

                _EntradaNumero = value;
                OnPropertyChanged(nameof(EntradaNumero));
            }
        }

        public string IncidentesNumero
        {
            get { return _IncidentesNumero; }
            set
            {
                if (_IncidentesNumero == value)
                    return;

                _IncidentesNumero = value;
                OnPropertyChanged(nameof(IncidentesNumero));
            }
        }

        public string EncuestaNumero
        {
            get { return _EncuestaNumero; }
            set
            {
                if (_EncuestaNumero == value)
                    return;

                _EncuestaNumero = value;
                OnPropertyChanged(nameof(EncuestaNumero));
            }
        }

        public DetalleServicioViewModel()
        {
            popupLayout = new SfPopupLayout();
        }

        public async Task Initialize()
        {
            await zConsultaConcecionario(App.zClaveCon);
        }

        private async Task zConsultaConcecionario(int cveConcecionario)
        {
            try
            {
                zvarConcesionario = DSConcesionario.zGetItem(cveConcecionario);

                SQLiteDataEncuestaConcesionario sQLiteDataEncuestaConcesionario = new SQLiteDataEncuestaConcesionario();
                List<EncuestaConcesionario> encuestaConcesionarios = await sQLiteDataEncuestaConcesionario.zGetAllItemsAsync(App.zClaveFV, App.zClaveCon);

                EncuestaNumero = encuestaConcesionarios.Count() > 0 ? "1" : "0";
                EncuestaImagen = encuestaConcesionarios.Count() > 0 ? ImageSource.FromFile("accept.png") : ImageSource.FromFile("cancelar1.ico");

                SQLiteDataIncidencia sQLiteDataIncidencia = new SQLiteDataIncidencia();
                List<Incidencia> dbIncidencias = (await sQLiteDataIncidencia.zGetItemsAsync()).Where(x => x.Csr == App.zClaveFV || x.Csr == App.zClaveCon).ToList();

                IncidentesNumero = dbIncidencias.Count() > 0 ? dbIncidencias.Count().ToString() : "0";
                IncidentesImagen = dbIncidencias.Count() > 0 ? ImageSource.FromFile("accept.png") : ImageSource.FromFile("cancelar1.ico");

                SQLiteDataEntrada sQLiteDataEntrada = new SQLiteDataEntrada();
                Entrada entrada = await sQLiteDataEntrada.zGetItemClaveConcesionarioAsync(App.zClaveCon);

                EntradaNumero = entrada != null ? "1" : "0";
                EntradaImagen = entrada != null ? ImageSource.FromFile("accept.png") : ImageSource.FromFile("cancelar1.ico");

                List<ArticuloObject> articulos = BuscaArticulos();

                EntregasNumero = articulos.Count() > 0 ? articulos.Count().ToString() : "0";
                EntregasImagen = articulos.Count() > 0 ? ImageSource.FromFile("accept.png") : ImageSource.FromFile("cancelar1.ico");
                
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public List<ArticuloObject> BuscaArticulos()
        {
            List<ArticuloObject> articulosRequests;

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

                    var zresu = x.Query<ArticuloObject>(zcmd, App.zClaveFV, App.zClaveCon);
                    if (zresu.Count >= 1)
                    {
                        zresu = zresu.Where(z => z.tpr == z.tpp).ToList();
                        articulosRequests = new List<ArticuloObject>(zresu);
                    }
                    else
                    {
                        articulosRequests = new List<ArticuloObject>();
                    }x.Dispose();
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });

                articulosRequests = new List<ArticuloObject>();

                MostrarError(ex.Message);
            }

            return articulosRequests;
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
                        Command = new Command(async () => { await Shell.Current.GoToAsync("//CerrarServicio"); Shell.Current.FlyoutIsPresented = false; }),
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
    }
}
