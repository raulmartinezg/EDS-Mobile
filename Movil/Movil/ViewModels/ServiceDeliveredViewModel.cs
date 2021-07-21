using Movil.DataSQL;
using Movil.Models.DB;
using Movil.Models.Response;
using Movil.Services.Routing;
using Newtonsoft.Json;
using Splat;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class ServiceDeliveredViewModel : ObservableINotifyObject
    {
        private IRoutingService _navigationService;

        SfPopupLayout popupLayout;

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

        public ServiceDeliveredViewModel(IRoutingService navigationService = null)
        {
            _navigationService = navigationService ?? Locator.Current.GetService<IRoutingService>();

            ServicesSource = GetServices();

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
                        zresu = zresu.Where(z => z.TotalProcesar == z.TotalProcesado).ToList();
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
