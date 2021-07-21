using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Models;
using Movil.Models.DB;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Syncfusion.XForms.PopupLayout;
using System;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class IncidentesViewModel : ObservableINotifyObject
    {
        #region Attributes
        private byte[] _ImageData { get; set; }

        private ImageSource _ImageSource { get; set; }

        private string _Descripcion { get; set; }

        private bool _CamaraEnabled { get; set; }

        private bool _GaleriaEnabled { get; set; }
        #endregion

        #region Properties
        public IncidenciaObject Incidencia { get; set; }

        SfPopupLayout popupLayout;

        public byte[] ImageData
        {
            get { return _ImageData; }
            set
            {
                if (_ImageData == value)
                    return;

                _ImageData = value;
                OnPropertyChanged(nameof(ImageData));
            }
        }

        public ImageSource ImageSource
        {
            get { return _ImageSource; }
            set
            {
                if (_ImageSource == value)
                    return;

                _ImageSource = value;
                OnPropertyChanged(nameof(ImageSource));
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

        public bool CamaraEnabled
        {
            get { return _CamaraEnabled; }
            set
            {
                if (_CamaraEnabled == value)
                    return;

                _CamaraEnabled = value;
                OnPropertyChanged(nameof(CamaraEnabled));
            }
        }

        public bool GaleriaEnabled
        {
            get { return _GaleriaEnabled; }
            set
            {
                if (_GaleriaEnabled == value)
                    return;

                _GaleriaEnabled = value;
                OnPropertyChanged(nameof(GaleriaEnabled));
            }
        }
        #endregion

        #region Constructor
        public IncidentesViewModel()
        {
            popupLayout = new SfPopupLayout();
            Incidencia = new IncidenciaObject();
            CamaraEnabled = true;
            GaleriaEnabled = true;

            SendCommand = new Command(async () => await Send());
            CameraCommand = new Command(async () => await Camera());
            GalleryCommand = new Command(async () => await Gallery());
        }

        #endregion

        #region Commands
        public ICommand SendCommand { get; set; }

        public ICommand CameraCommand { get; set; }

        public ICommand GalleryCommand { get; set; }
        #endregion

        #region Functions
        private async Task Send()
        {
            UserDialogs.Instance.ShowLoading("Cargando...");

            try
            {
                SQLiteDataIncidencia incidencia = new SQLiteDataIncidencia();

                bool exito = await incidencia.zAddItemAsync(new Incidencia()
                {
                    Fecha = DateTime.Now,
                    Csr = App.zClaveFV,
                    Imagen = ImageData,
                    Descripcion = Descripcion,
                    Tipo = "FolioViaje",
                    Sincronizado = 0
                });

                SQLiteDataFolioViaje folioViaje = new SQLiteDataFolioViaje();
                FolioViaje folio = await folioViaje.zGetItemAsync(App.zClaveFV);
                folio.Sincronizado = 0;
                _ = await folioViaje.zUpdateItemAsync(folio);

                if (exito)
                {
                    PopExito("Se guardo con exito");
                }
                else
                {
                    MostrarError("Erro en el servicio");
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }

            UserDialogs.Instance.HideLoading();
        }

        public async Task Camera()
        {
            try
            {
                _ = await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    MostrarError("Camara no disponible.");
                    return;
                }

                await ChargePermissions();

                var file = await CrossMedia.Current.TakePhotoAsync(new StoreCameraMediaOptions
                {
                    Directory = "EDS",
                    Name = "incidencia.jpg"
                });

                if (file == null)
                    return;

                ImageData = ImageSourceToBytes(file);

                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });

            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
            
        }

        public async Task Gallery()
        {
            try
            {
                await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsPickPhotoSupported)
                {
                    MostrarError("Camara no disponible.");
                    return;
                }

                await ChargePermissions();

                var file = await CrossMedia.Current.PickPhotoAsync(new PickMediaOptions
                {
                    PhotoSize = PhotoSize.Medium
                });

                if (file == null)
                    return;

                ImageData = ImageSourceToBytes(file);

                ImageSource = ImageSource.FromStream(() =>
                {
                    var stream = file.GetStream();
                    return stream;
                });
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public async Task ChargePermissions()
        {
            var statusStorage = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            if (statusStorage != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                {
                    MostrarError("Se necesitan permisos de almacenamiento");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                statusStorage = results[Permission.Storage];

                if (statusStorage != PermissionStatus.Granted)
                {
                    MostrarError("Permisos denegados");
                    return;
                }
                
            }

            var statusCamera = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            if (statusCamera != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Camera))
                {
                    MostrarError("Se necesitan permisos de almacenamiento");
                }

                var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Camera);
                statusCamera = results[Permission.Camera];

                if (statusCamera != PermissionStatus.Granted)
                {
                    MostrarError("Permisos denegados");
                    return;
                }
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
                        Command = new Command(async () => await Shell.Current.GoToAsync("//traveldetail")),
                        Text = "Ok",
                        BackgroundColor = Color.LightGray,
                        TextColor = Color.Black,
                        Margin = new Thickness(5)
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

        public byte[] ImageSourceToBytes(MediaFile imageSource)
        {
            byte[] imageArray = null;

            try
            {
                if (imageSource != null)
                {
                    using (MemoryStream ms = new MemoryStream())
                    {
                        var stream = imageSource.GetStream();
                        stream.CopyTo(ms);
                        imageArray = ms.ToArray();
                    }
                } 
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }

            return imageArray;
        }
        #endregion
    }
}
