using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Models.DB;
using Newtonsoft.Json;
using Syncfusion.XForms.PopupLayout;
using Syncfusion.XForms.SignaturePad;
using System;
using System.IO;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class LlegadaViewModel : ObservableINotifyObject
    {
        private string _Nombre { get; set; }

        private string _Numero { get; set; }

        SfSignaturePad signaturePad;

        SfPopupLayout popupLayout;

        public Command ButtonCommand { private set; get; }

        public LlegadaViewModel(SfSignaturePad sfSignaturePad)
        {
            popupLayout = new SfPopupLayout();
            signaturePad = sfSignaturePad;
            ButtonCommand = new Command(async () => await ConvertSourceToBytes());
        }

        public string Nombre
        {
            get { return _Nombre; }
            set
            {
                if (_Nombre == value)
                    return;

                _Nombre = value;
                OnPropertyChanged(nameof(Nombre));
            }
        }

        public string Numero
        {
            get { return _Numero; }
            set
            {
                if (_Numero == value)
                    return;

                _Numero = value;
                OnPropertyChanged(nameof(Numero));
            }
        }

        private async Task ConvertSourceToBytes()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Cargando...");

                signaturePad.Save();

                if (signaturePad.ImageSource != null && !string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(Numero))
                {
                    StreamImageSource streamImageSource = (StreamImageSource)signaturePad.ImageSource;

                    System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
                    Task<Stream> task = streamImageSource.Stream(cancellationToken);
                    Stream stream = task.Result;

                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    Entrada entrada = new Entrada()
                    {
                        ClaveConcesionario = App.zClaveCon,
                        Empleado = Numero,
                        NombreEmpleado = Nombre,
                        Fecha = DateTime.Now,
                        Firma = bytes,
                        Sincronizado = 0
                    };

                    SQLiteDataEntrada sQLiteDataEntrada = new SQLiteDataEntrada();
                    _ = await sQLiteDataEntrada.zAddItemAsync(entrada);

                    SQLiteDataConcesionario concesionario = new SQLiteDataConcesionario();
                    Concesionario con = await concesionario.zGetItemAsync(App.zClaveCon);
                    con.Sincronizado = 0;
                    _ = await concesionario.zUpdateItemAsync(con);

                    SQLiteDataFolioViaje folioViaje = new SQLiteDataFolioViaje();
                    FolioViaje folio = await folioViaje.zGetItemAsync(App.zClaveFV);
                    folio.Sincronizado = 0;
                    _ = await folioViaje.zUpdateItemAsync(folio);

                    SQLiteDataOrdenEmbarque ordenEmbarque = new SQLiteDataOrdenEmbarque();
                    OrdenEmbarque orden = await ordenEmbarque.zGetOrdenesEmbarqueConcesionarioAsync(con.ClaveConcesionario, folio.ClaveFolioViaje);
                    orden.Sincronizado = 0;
                    _ = await ordenEmbarque.zUpdateItemAsync(orden);

                    PopExito("Entrada registrada exitosamente");
                    
                }
                else
                {
                    MostrarError("Todos los campos son obligatorios");
                }

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception err)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(err), Fecha = DateTime.Now });
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
                        Command = new Command(async () => await Shell.Current.GoToAsync("//PageEntregas")),
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
