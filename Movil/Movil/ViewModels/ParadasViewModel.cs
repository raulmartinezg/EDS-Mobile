using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Models.DB;
using Movil.Models.Response;
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
    public class ParadasViewModel : ObservableINotifyObject
    {
        private ObservableCollection<ConcesionarioResponse> _zpropParadasPendientes { get; set; }

        private string _Title { get; set; }

        SfPopupLayout popupLayout;

        private Command<Object> tappedCommand;

        public Command<object> TappedCommand
        {
            get { return tappedCommand; }
            set { tappedCommand = value; }
        }

        public string Title
        {
            get { return _Title; }
            set
            {
                if (_Title == value)
                    return;

                _Title = value;
                OnPropertyChanged(nameof(Title));
            }
        }

        public ObservableCollection<ConcesionarioResponse> zpropParadasPendientes
        {
            get { return _zpropParadasPendientes; }
            set
            {
                _zpropParadasPendientes = value;
                OnPropertyChanged(nameof(zpropParadasPendientes));
            }
        }

        public ParadasViewModel(string cveFV, string Tp)
        {
            try
            {
                popupLayout = new SfPopupLayout();

                TappedCommand = new Command<object>(TappedCommandMethod);
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
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
                                             " MAX(C.FechaLlegadaEstimada) [FechaLlegadaEstimada],OE.Procesado ,C.Latitud, C.Longitud  " +
                                             " FROM   OrdenEmbarque  OE "  +
                                             " INNER JOIN Concesionario C ON C.ClaveConcesionario = OE.ClaveConcesionario " +
                                             " WHERE   OE.ClaveFolioViaje = ?" +
                                             " GROUP BY OE.ClaveConcesionario,C.Latitud, C.Longitud" +
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

        public void Initialize(string cveFV, string Tp)
        {
            List<ConcesionarioResponse> concesionarios = MostrarParadas(cveFV);
            if (Tp.Equals("C"))
            {
                concesionarios = concesionarios.Where(x => x.Procesado == 1).ToList();
                zpropParadasPendientes = new ObservableCollection<ConcesionarioResponse>(concesionarios);
                Title = "Paradas Concluidas";
            }
            else
            {
                concesionarios = concesionarios.Where(x => x.Procesado == 0).ToList();
                zpropParadasPendientes = new ObservableCollection<ConcesionarioResponse>(concesionarios);
                Title = "Paradas Pendientes";
            }
        }

        private async void TappedCommandMethod(object obj)
        {
            UserDialogs.Instance.ShowLoading("Cargando...");
            await Task.Delay(300);

            try
            {
                ConcesionarioResponse concesionario = (obj as Syncfusion.ListView.XForms.ItemTappedEventArgs).ItemData as ConcesionarioResponse;
                App.zClaveCon = concesionario.ClaveConcesionario;

                await Shell.Current.GoToAsync($"//DetalleServicioView");

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();

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
    }
}

