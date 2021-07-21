using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Helpers;
using Movil.Models.DB;
using Movil.Models.Request;
using Movil.Models.Response;
using Movil.Services.Routing;
using Newtonsoft.Json;
using Splat;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class TravelsViewModel : ObservableINotifyObject
    {
        private IRoutingService _navigationService;

        public SQLiteDataFolioViaje DSFolioViaje;

        private bool isRefreshing;

        SfPopupLayout popupLayout;

        public ObservableCollection<dvNewDataSet> _ViajesSource { get; set; }

        public ICommand BackCommand { get; set; }

        public ICommand RefreshCommand { get; set; }

        private Command<Object> tappedCommand;

        public Command<object> TappedCommand
        {
            get { return tappedCommand; }
            set { tappedCommand = value; }
        }

        public bool IsRefreshing
        {
            get
            {
                return isRefreshing;
            }
            set
            {
                isRefreshing = value;
                OnPropertyChanged(nameof(IsRefreshing));
            }
        }

        public NotifyTaskCompletion<ObservableCollection<dvNewDataSet>> ViajesSource { get; private set; }

        public TravelsViewModel(IRoutingService navigationService = null)
        {
            _navigationService = navigationService ?? Locator.Current.GetService<IRoutingService>();

            ViajesSource = new NotifyTaskCompletion<ObservableCollection<dvNewDataSet>>(GetTravels());

            TappedCommand = new Command<object>(TappedCommandMethod);

            BackCommand = new Command(async () => await BackCommandMethod());

            RefreshCommand = new Command(Refresh);

            popupLayout = new SfPopupLayout();
        }

        async private void Refresh(object obj)
        {
            try
            {
                IsRefreshing = true;
                UserDialogs.Instance.ShowLoading("Cargando...");
                await Task.Delay(300);

                await GetTravels();

                UserDialogs.Instance.HideLoading();
                IsRefreshing = false;
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public async Task BackCommandMethod()
        {
            try
            {
                await _navigationService.NavigateTo("///Bienvenida");
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public async Task<ObservableCollection<dvNewDataSet>> GetTravels()
        {
            UserDialogs.Instance.ShowLoading("Cargando...");
            await Task.Delay(300);

            try
            {
                ViajesRequest viajesRequest = new ViajesRequest
                {
                    data = new zData()
                    {
                        bdCc = 22,
                        bdSp = "SPQRY_OrdenViajeV2",
                        bdCat = "SIDTUM_PROD",
                        bdSch = "SidMovil",
                        encV = 2
                    },
                    filter = JsonConvert.SerializeObject(new List<Models.Request.Filter>() { new Models.Request.Filter() { property = "claveOperador", value = App.zClaveOp } })
                };

                ServiceWebClient.zURL = Helpers.CDGlobales.zURLViajes;

                DescargaViaje resp = await ServiceWebClient.GetTravels(viajesRequest);

                if (resp.success)
                {
                    if (resp.data.newDataSet.Count() == 0)
                    {
                        SQLiteDataFolioViaje sQLiteData = new SQLiteDataFolioViaje();

                        List<FolioViaje> viajes = (await sQLiteData.zGetFoliosOperadorAsync(App.zClaveOp)).ToList();
                        if (viajes.Count > 0)
                        {
                            App.zClaveFV = viajes.ElementAt(0).ClaveFolioViaje;

                            _ViajesSource = new ObservableCollection<dvNewDataSet>();

                            foreach (FolioViaje viaje in viajes)
                            {
                                _ViajesSource.Add(new dvNewDataSet()
                                {
                                    cfv = viaje.ClaveFolioViaje,
                                    cop = viaje.ClaveOperador,
                                    cun = viaje.ClaveUnidad,
                                    car = viaje.CantidadArticulo,
                                    com = viaje.Completadas,
                                    fpe = viaje.FechaPrimeraEntrega,
                                    fpr = viaje.FechaProcesado,
                                    fsp = viaje.FechaSalidaProgramada,
                                    fvi = viaje.FolioDeViaje,
                                    noe = viaje.NumeroOrdenes,
                                    par = viaje.Paradas,
                                    pro = viaje.Procesado,
                                    rut = viaje.Ruta,
                                    sin = viaje.Sincronizado == 1,
                                    uni = viaje.Unidad
                                });
                            }
                        }
                        else
                        {
                            MostrarError("No hay datos para mostrar.");
                        }
                    }
                    else
                    {
                        _ViajesSource = new ObservableCollection<dvNewDataSet>(resp.data.newDataSet);
                        App.zClaveFV = resp.data.newDataSet[0].cfv;
                    }
                }
                else
                {
                    if (resp.message == "No tiene conectividad Wi-fi." ||
                        resp.message.IndexOf("Unable to resolve host") >= 0 ||
                        resp.message.IndexOf("operation was canceled") >= 0 ||
                        resp.message.IndexOf("recvfrom failed: ECONNRESET (Connection reset by peer)") >= 0 ||
                        resp.message == "Error al intentar establecer conexion con el servidor.")
                    {
                        SQLiteDataFolioViaje sQLiteData = new SQLiteDataFolioViaje();

                        List<FolioViaje> viajes = (await sQLiteData.zGetFoliosOperadorAsync(App.zClaveOp)).ToList();
                        if (viajes.Count > 0)
                        {
                            App.zClaveFV = viajes.ElementAt(0).ClaveFolioViaje;

                            _ViajesSource = new ObservableCollection<dvNewDataSet>();

                            foreach (FolioViaje viaje in viajes)
                            {
                                _ViajesSource.Add(new dvNewDataSet()
                                {
                                    cfv = viaje.ClaveFolioViaje,
                                    cop = viaje.ClaveOperador,
                                    cun = viaje.ClaveUnidad,
                                    car = viaje.CantidadArticulo,
                                    com = viaje.Completadas,
                                    fpe = viaje.FechaPrimeraEntrega,
                                    fpr = viaje.FechaProcesado,
                                    fsp = viaje.FechaSalidaProgramada,
                                    fvi = viaje.FolioDeViaje,
                                    noe = viaje.NumeroOrdenes,
                                    par = viaje.Paradas,
                                    pro = viaje.Procesado,
                                    rut = viaje.Ruta,
                                    sin = viaje.Sincronizado == 1,
                                    uni = viaje.Unidad
                                });
                            }
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
                }

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();

                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });

                MostrarError(ex.Message);
            }

            return _ViajesSource;
        }

        public DescargaViaje GetDummieData(DescargaViaje resp)
        {
            try
            {
                string jsonFileName = "dummies.ObtieneViajes.json";
                var assembly = typeof(TravelsViewModel).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");
                using (var reader = new StreamReader(stream))
                {
                    var jsonString = reader.ReadToEnd();

                    //Converting JSON Array Objects into generic list    
                    resp = JsonConvert.DeserializeObject<DescargaViaje>(jsonString);
                    reader.Dispose();
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }

            return resp;
        }

        private async void TappedCommandMethod(object obj)
        {
            UserDialogs.Instance.ShowLoading("Cargando...");
            await Task.Delay(300);

            try
            {
                dvNewDataSet travel = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as dvNewDataSet;
                List<dvNewDataSet> dvs = _ViajesSource.ToList();

                int index = dvs.IndexOf(travel);
                dvNewDataSet nextTravel = new dvNewDataSet();

                if (dvs.Count > index + 1)
                {
                    nextTravel = dvs.ElementAt(index + 1);
                }

                await _navigationService.NavigateTo($"//traveldetail?travel={JsonConvert.SerializeObject(travel)}&nextTravel={JsonConvert.SerializeObject(nextTravel)}");

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();

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
                        Command = new Command(async () => { await Shell.Current.GoToAsync("///Bienvenida"); }),
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