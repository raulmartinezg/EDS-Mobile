using Movil.DataSQL;
using Movil.Models;
using Movil.Models.DB;
using Newtonsoft.Json;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class EncuestaViewModel : ObservableINotifyObject
    {
        public SQLiteDataEncuesta zvarsqldatEncuesta = new SQLiteDataEncuesta();

        private List<Encuesta> zvarEnc = new List<Encuesta>();
        public List<EncuestaConcesionario> zvarEnc_Conces { get; set; } = new List<EncuestaConcesionario>();
        public string DescPregunta { get; set; }
        public List<Encuesta> zcioEncuesta
        {
            get
            {
                return zvarEnc;
            }
            set
            {
                zvarEnc = value;
            }
        }
        public List<OpcionPreguntas> zcioOpcPreguntas
        {
            get;
            set;
        } = new List<OpcionPreguntas>();

        SfPopupLayout popupLayout;

        public EncuestaViewModel()
        {
            popupLayout = new SfPopupLayout();
        }

        public async Task<MensageGenerico> zMetCargarDatosEncuesta()
        {
            MensageGenerico zmsgresult;
            try
            {
                SQLiteDataEncuesta encuesta = new SQLiteDataEncuesta();

                zvarEnc = await encuesta.zGetItemsAsync();

                if (zvarEnc != null && zvarEnc.Count > 0)
                    return zmsgresult = new MensageGenerico { Mensaje = "ok", status = true };
                if (zvarEnc.Count == 0)
                    return zmsgresult = new MensageGenerico { Mensaje = "La encuesta ya fue resuelta...", status = false };
                
            }
            catch (Exception e)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(e), Fecha = DateTime.Now });

                zmsgresult = new MensageGenerico { Mensaje = e.Message, status = false };
            }
            return zmsgresult = new MensageGenerico { Mensaje = "objeto es igual a null", status = false };
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
                        Command = new Command(async () => { await Shell.Current.GoToAsync("//PageSalidas"); Shell.Current.FlyoutIsPresented = false; }),
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
