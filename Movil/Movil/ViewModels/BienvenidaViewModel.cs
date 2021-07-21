using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Models.DB;
using Movil.Services.Routing;
using Newtonsoft.Json;
using Splat;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class BienvenidaViewModel : ObservableINotifyObject
    {
        private IRoutingService _navigationService;

        SfPopupLayout popupLayout;

        private SQLiteDataOperador DSOperador = new SQLiteDataOperador();
        private string zvarOperador;

        public string zOperadorBienvenida
        {
            get
            { return zvarOperador; }
            set
            {
                zvarOperador = value;
            }
        }

        public string zExhorto { get; set; }

        public ICommand GoTravels { get; set; }

        public BienvenidaViewModel(IRoutingService navigationService = null)
        {
            _navigationService = navigationService ?? Locator.Current.GetService<IRoutingService>();

            popupLayout = new SfPopupLayout();

            zBienvenidoOperador();

            GoTravels = new Command(() => Travels_Clicked());
        }

        private void zBienvenidoOperador()
        {
            try
            {
                var zopa = DSOperador.zGetItem(App.zClaveOp);
                zOperadorBienvenida = "Hola! " + zopa.FcNombre + Environment.NewLine + "[ " + zopa.FnOperador + " ]";

                if (!string.IsNullOrEmpty(zopa.FcExhorto))
                {
                    zExhorto = zopa.FcExhorto;
                }
                else
                {
                    zExhorto = Helpers.CDGlobales.ZExhorto;
                }
            }
            catch (Exception ex)
            {
                UserDialogs.Instance.HideLoading();

                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new Models.DB.LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }

            UserDialogs.Instance.HideLoading();
        }

        private async void Travels_Clicked()
        {
            try
            {
                await _navigationService.NavigateTo("travels");
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new Models.DB.LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
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
                        Command = new Command(async () => await Shell.Current.GoToAsync("//CerrarSesion")),
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
