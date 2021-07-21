using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Models.DB;
using Movil.Models.Response;
using Movil.Services.Routing;
using Newtonsoft.Json;
using Splat;
using Syncfusion.SfNumericUpDown.XForms;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class MissingServiceViewModel : ObservableINotifyObject
    {
        private IRoutingService _navigationService;

        SfPopupLayout popupLayout;

        private Command<Object> tappedCommand;

        public Command<object> TappedCommand
        {
            get { return tappedCommand; }
            set { tappedCommand = value; }
        }

        private ObservableCollection<ArticulosRequest> _ServicesSource { get; set; }

        public ObservableCollection<ArticulosRequest> ServicesSource
        {
            get { return _ServicesSource; }
            set
            {
                _ServicesSource = value;
                OnPropertyChanged(nameof(ServicesSource));
            }
        }

        public MissingServiceViewModel(IRoutingService navigationService = null)
        {
            _navigationService = navigationService ?? Locator.Current.GetService<IRoutingService>();

            ServicesSource = GetServices();

            TappedCommand = new Command<object>(TappedCommandMethod);

            popupLayout = new SfPopupLayout();
        }

        internal void Initialize()
        {
            try
            {
                ServicesSource = GetServices();
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public ObservableCollection<ArticulosRequest> GetServices()
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
                                   " CAST(OED.TotalProcesado AS NVARCHAR) || '/' || CAST(OED.TotalProcesar AS NVARCHAR) AS \"Entregado\", " +
                                   " OED.TotalProcesado" +
                                  " FROM OrdenEmbarque OE " +
                                  " INNER JOIN OrdenEmbarqueDetalle OED ON OED.ClaveOrdenEmbarque = OE.ClaveOrdenEmbarque " +
                                  " INNER JOIN CatalogoArticulo CA ON CA.ClaveArticulo = OED.ClaveArticulo " +
                                  " WHERE OE.ClaveFolioViaje = ?  " +
                                  " AND OE.ClaveConcesionario = ?";

                    var zresu = x.Query<ArticulosRequest>(zcmd, App.zClaveFV.ToString(), App.zClaveCon);
                    if (zresu.Count >= 1)
                    {
                        zresu = zresu.Where(z => z.TotalProcesado < z.TotalProcesar).ToList();
                        articulosRequests = new ObservableCollection<ArticulosRequest>(zresu);
                    }
                    else
                    {
                        articulosRequests = new ObservableCollection<ArticulosRequest>();
                    }x.Dispose();
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

        public async Task EgresarArticulo(string cantidad, string sku)
        {
            UserDialogs.Instance.ShowLoading("Cargando...");

            try
            {
                _ = int.TryParse(cantidad, out int _cantidad);

                if (!sku.Equals("") && _cantidad > 0)
                {
                    ArticulosRequest articulo = ServicesSource.Where(x => x.SKU.Equals(sku)).FirstOrDefault();

                    SQLiteDataOrdenEmbarqueDetalle sql = new SQLiteDataOrdenEmbarqueDetalle();
                    OrdenEmbarqueDetalle ordenEmbarqueDetalle = await sql.zGetItemAsync(articulo.ClaveOrdenEmbDet);
                    ordenEmbarqueDetalle.TotalProcesado += _cantidad;
                    ordenEmbarqueDetalle.Sincronizado = 0;
                    ordenEmbarqueDetalle.Manual = 1;
                    bool detalleActualizado = await sql.zUpdateItemAsync(ordenEmbarqueDetalle);

                    SQLiteDataOrdenEmbarque ordenEmbarque = new SQLiteDataOrdenEmbarque();
                    OrdenEmbarque orden = await ordenEmbarque.zGetItemAsync(ordenEmbarqueDetalle.ClaveOrdenEmbarque);
                    orden.Sincronizado = 0;
                    bool ordenActualizado = await ordenEmbarque.zUpdateItemAsync(orden);

                    SQLiteDataFolioViaje folioViaje = new SQLiteDataFolioViaje();
                    FolioViaje folio = await folioViaje.zGetItemAsync(orden.ClaveFolioViaje);
                    folio.Sincronizado = 0;
                    bool folioActualizado = await folioViaje.zUpdateItemAsync(folio);

                    ServicesSource = GetServices();
                }
                else
                {
                    MostrarError("Debe leer un artículo");
                }

            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();

                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });

                MostrarError(ex.Message);
            }

            UserDialogs.Instance.HideLoading();
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

        private async void TappedCommandMethod(object obj)
        {
            try
            {
                ArticulosRequest articulo = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as ArticulosRequest;
                int cantidad = articulo.TotalProcesar - articulo.TotalProcesado;
                Confirmar(cantidad.ToString(), articulo.SKU);
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public void Confirmar(string cantidad, string sku)
        {
            try
            {
                SfNumericUpDown numericUpDown = new SfNumericUpDown();

                DataTemplate templateView = new DataTemplate(() =>
                {
                    Grid grid = new Grid
                    {
                        RowDefinitions = new RowDefinitionCollection
                    {
                        new RowDefinition { Height = GridLength.Auto },
                        new RowDefinition { Height = GridLength.Auto }
                    }
                    };

                    Label popupContent = new Label
                    {
                        Text = "¿Deseas confirmar el egreso de este item?",
                        BackgroundColor = Color.White,
                        TextColor = Color.Black,
                        HorizontalTextAlignment = TextAlignment.Center,
                        VerticalTextAlignment = TextAlignment.Center
                    };

                    numericUpDown.Maximum = double.Parse(cantidad);
                    numericUpDown.ValueChangeMode = ValueChangeMode.OnKeyFocus;
                    numericUpDown.MaximumDecimalDigits = 0;
                    numericUpDown.Minimum = 1;

                    grid.Children.Add(popupContent);
                    Grid.SetRow(popupContent, 0);

                    grid.Children.Add(numericUpDown);
                    Grid.SetRow(numericUpDown, 1);

                    return grid;
                });

                DataTemplate footerTemplete = new DataTemplate(() =>
                {
                    Grid grid = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitionCollection
                        {
                            new ColumnDefinition { Width = GridLength.Star },
                            new ColumnDefinition { Width = GridLength.Star }
                        }
                    };

                    Button ok = new Button()
                    {
                        Command = new Command(async () => { await EgresarArticulo(numericUpDown.Value.ToString(), sku); Shell.Current.FlyoutIsPresented = false; popupLayout.IsOpen = false; }),
                        Text = "SI",
                        FontSize = 12,
                        BackgroundColor = Color.LightGray,
                        TextColor = Color.Black,
                        Margin = new Thickness(10)
                    };

                    Button back = new Button()
                    {
                        Command = new Command(() => popupLayout.IsOpen = false),
                        Text = "NO",
                        FontSize = 12,
                        BackgroundColor = Color.LightGray,
                        TextColor = Color.Black,
                        Margin = new Thickness(10)
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
    }
}
