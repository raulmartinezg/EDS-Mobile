using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Models;
using Movil.Models.DB;
using Movil.Models.Response;
using Movil.Models.ResultQry;
using Newtonsoft.Json;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Syncfusion.XForms.PopupLayout;
using Syncfusion.XForms.SignaturePad;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Movil.ViewModels
{
    public class SalidasViewModel : ObservableINotifyObject
    {
        private string _Nombre { get; set; }

        private string _Numero { get; set; }

        private bool _CamaraEnabled { get; set; }

        private bool _GaleriaEnabled { get; set; }

        private byte[] _ImageData { get; set; }

        private ImageSource _ImageSource { get; set; }

        SfSignaturePad signaturePad;

        SfPopupLayout popupLayout;

        public Command ButtonCommand { private set; get; }

        public ICommand CameraCommand { get; set; }

        public ICommand GalleryCommand { get; set; }

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

        public SalidasViewModel(SfSignaturePad sfSignaturePad)
        {
            popupLayout = new SfPopupLayout();
            signaturePad = sfSignaturePad;
            ButtonCommand = new Command(async () => await ConvertSourceToBytes());
            CameraCommand = new Command(async () => await Camera());
            GalleryCommand = new Command(async () => await Gallery());
        }

        public async Task Camera()
        {
            try
            {
                _ = await CrossMedia.Current.Initialize();

                if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                {
                    MostrarError(":( No camera available.");
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

        private async Task ConvertSourceToBytes()
        {
            try
            {
                UserDialogs.Instance.ShowLoading("Cargando...");

                signaturePad.Save();

                if (signaturePad.ImageSource != null && !string.IsNullOrEmpty(Nombre) && !string.IsNullOrEmpty(Numero) && ImageData != null)
                {
                    StreamImageSource streamImageSource = (StreamImageSource)signaturePad.ImageSource;

                    System.Threading.CancellationToken cancellationToken = System.Threading.CancellationToken.None;
                    Task<Stream> task = streamImageSource.Stream(cancellationToken);
                    Stream stream = task.Result;

                    byte[] bytes = new byte[stream.Length];
                    stream.Read(bytes, 0, bytes.Length);

                    Salida salida = new Salida()
                    {
                        ClaveConcesionario = App.zClaveCon,
                        Empleado = Numero,
                        NombreEmpleado = Nombre,
                        Fecha = DateTime.Now,
                        Firma = bytes,
                        Sincronizado = 0
                    };

                    SQLiteDataSalida sQLiteDataSalida = new SQLiteDataSalida();
                    await sQLiteDataSalida.zAddItemAsync(salida);
                    RegistroObject registroSalida = new RegistroObject()
                    {
                        claveConcesionario = salida.ClaveConcesionario,
                        empleado = salida.Empleado,
                        nombreEmpleado = salida.NombreEmpleado,
                        fecha = salida.Fecha,
                        firma = salida.Firma,
                        imagen = ImageData
                    };

                    SQLiteDataConcesionario concesionario = new SQLiteDataConcesionario();
                    Concesionario con = await concesionario.zGetItemAsync(App.zClaveCon);
                    con.Sincronizado = 0;
                    _ = await concesionario.zUpdateItemAsync(con);

                    SQLiteDataFolioViaje folioViaje = new SQLiteDataFolioViaje();
                    FolioViaje folio = await folioViaje.zGetItemAsync(App.zClaveFV);
                    folio.Sincronizado = 0;
                    _ = await folioViaje.zUpdateItemAsync(folio);

                    SQLiteDataOrdenEmbarque ordenEmbarque = new SQLiteDataOrdenEmbarque();
                    OrdenEmbarque orden = await ordenEmbarque.zGetOrdenesEmbarqueConcesionarioAsync(con.ClaveConcesionario,folio.ClaveFolioViaje);
                    orden.Sincronizado = 0;
                    orden.Procesado = 1;
                    _ = await ordenEmbarque.zUpdateItemAsync(orden);

                    #region
                    SQLiteDataEntrada sQLiteDataEntrada = new SQLiteDataEntrada();
                    Entrada entrada = await sQLiteDataEntrada.zGetItemClaveConcesionarioAsync(App.zClaveCon);
                    RegistroObject registroEntrada = new RegistroObject();

                    if (entrada != null)
                    {
                        registroEntrada = new RegistroObject()
                        {
                            claveConcesionario = entrada.ClaveConcesionario,
                            empleado = entrada.Empleado,
                            nombreEmpleado = entrada.NombreEmpleado,
                            fecha = entrada.Fecha,
                            firma = entrada.Firma
                        };
                    }

                    SQLiteDataEncuestaConcesionario sQLiteDataEncuestaConcesionario = new SQLiteDataEncuestaConcesionario();
                    List<EncuestaConcesionario> encuestaConcesionarios = await sQLiteDataEncuestaConcesionario.zGetAllItemsAsync(App.zClaveFV, App.zClaveCon);
                    List<EncuestaObject> encuestas = new List<EncuestaObject>();

                    foreach (EncuestaConcesionario encuestaConcesionario in encuestaConcesionarios)
                    {
                        encuestas.Add(new EncuestaObject()
                        {
                            ClaveEncCon = encuestaConcesionario.ClaveEncCon,
                            ClaveFolioViaje = encuestaConcesionario.ClaveFolioViaje,
                            ClaveConcesionario = encuestaConcesionario.ClaveConcesionario,
                            ClavePregunta = encuestaConcesionario.ClavePregunta,
                            Respuesta = encuestaConcesionario.Respuesta,
                            Fecha = encuestaConcesionario.Fecha,
                            Observaciones = encuestaConcesionario.Observaciones,
                            NumEmpleado = encuestaConcesionario.NumEmpleado,
                            ClaveEstatus = encuestaConcesionario.ClaveEstatus,
                            Sincronizado = encuestaConcesionario.Sincronizado
                        });
                    }

                    List<ArticuloObject> articulos = BuscaArticulos();

                    SQLiteDataIncidencia sQLiteDataIncidencia = new SQLiteDataIncidencia();
                    List<Incidencia> dbIncidencias = (await sQLiteDataIncidencia.zGetItemsAsync()).Where(x => (x.Csr == App.zClaveFV || x.Csr == App.zClaveCon) && x.Sincronizado == 0).ToList();
                    List<IncidenciaObject> incidencias = new List<IncidenciaObject>();

                    foreach (Incidencia incidencia in dbIncidencias)
                    {
                        incidencias.Add(new IncidenciaObject()
                        {
                            Imagen = incidencia.Imagen,
                            Csr = incidencia.Csr,
                            Descripcion = incidencia.Descripcion,
                            Fecha = incidencia.Fecha,
                            Tipo = incidencia.Tipo
                        });
                    }

                    MasterObject master = new MasterObject()
                    {
                        salida = registroSalida,
                        entrada = registroEntrada,
                        encuestas = encuestas,
                        articulos = articulos,
                        incidencias = incidencias,
                        sincronizado = true
                    };
                    #endregion

                    MasterResponse masterResponse = await ServiceWebClient.Master(master);

                    if (masterResponse.success)
                    {
                        con.Sincronizado = 1;
                        _ = await concesionario.zUpdateItemAsync(con);

                        orden.Sincronizado = 1;
                        _ = await ordenEmbarque.zUpdateItemAsync(orden);

                        if (entrada != null)
                        {
                            if (entrada.ClaveConcesionario > 0)
                            {
                                entrada.Sincronizado = 1;
                                _ = await sQLiteDataEntrada.zUpdateItemAsync(entrada);
                            }
                        }

                        foreach (EncuestaConcesionario encuestaConcesionario in encuestaConcesionarios)
                        {
                            encuestaConcesionario.Sincronizado = 1;
                            _ = await sQLiteDataEncuestaConcesionario.zUpdateItemAsync(encuestaConcesionario);
                        }

                        SQLiteDataOrdenEmbarqueDetalle sQLiteDataOrdenEmbarqueDetalle = new SQLiteDataOrdenEmbarqueDetalle();

                        foreach (ArticuloObject articuloObject in articulos)
                        {
                            OrdenEmbarqueDetalle ordenEmbarqueDetalle = await sQLiteDataOrdenEmbarqueDetalle.zGetItemAsync(articuloObject.cod);
                            ordenEmbarqueDetalle.Sincronizado = 1;
                            _ = await sQLiteDataOrdenEmbarqueDetalle.zUpdateItemAsync(ordenEmbarqueDetalle);
                        }

                        foreach (Incidencia inc in dbIncidencias)
                        {
                            inc.Sincronizado = 1;
                            _ = await sQLiteDataIncidencia.zUpdateItemAsync(inc);
                        }

                        PopExito("Salida registrada con exito");
                    }
                    else
                    {
                        MostrarError("Error de red");
                    }
                }
                else
                {
                    MostrarError("Todos los campos son obligatorios");
                }

                UserDialogs.Instance.HideLoading();
            }
            catch (Exception err)
            {
                UserDialogs.Instance.HideLoading();
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(err), Fecha = DateTime.Now });
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
                                   " OED.TotalProcesado tpp, " +
                                   " OED.Manual " +
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

        public async Task GetDeliveries()
        {
            try
            {
                using (var x = App.zDb)
                {
                    string zcmd = " SELECT SQ.ClaveConcesionario, " +
                                   " CAST(SUM(SQ.TotalProcesado) AS NVARCHAR)  || '/' || CAST(SUM(SQ.TotalProcesar) AS NVARCHAR) AS \"Total\", " +
                                   " CAST(SUM(SQ.CajaFalta) AS NVARCHAR) || '/' || CAST(SUM(SQ.CajaTotal) AS NVARCHAR) AS \"Caja\", " +
                                   " CAST(SUM(SQ.ContFalta) AS NVARCHAR) || '/' || CAST(SUM(SQ.ContTotal)AS NVARCHAR) AS \"Contenedor\", " +
                                   " CAST(SUM(SQ.PiezaFalta) AS NVARCHAR) || '/' || CAST(SUM(SQ.PiezaTotal)AS NVARCHAR) AS \"Piezas\", " +
                                   " SUM(SQ.TotalProcesar)- SUM(SQ.TotalProcesado) AS \"PorProcesar\" " +
                                   " FROM (" +
                                   " SELECT OE.ClaveConcesionario," +
                                   " SUM(OED.TotalProcesar) AS \"TotalProcesar\", " +
                                   " SUM(OED.TotalProcesado) AS \"TotalProcesado\"," +
                                   " CASE WHEN CA.UnidadMedida = 1 THEN SUM(OED.TotalProcesar) ELSE 0 END AS \"CajaTotal\", " +
                                   " CASE WHEN CA.UnidadMedida = 1 THEN SUM(OED.TotalProcesado) ELSE 0 END AS \"CajaFalta\", " +
                                   " CASE WHEN CA.UnidadMedida = 2 THEN SUM(OED.TotalProcesar) ELSE 0 END AS \"ContTotal\", " +
                                   " CASE WHEN CA.UnidadMedida = 2 THEN SUM(OED.TotalProcesado) ELSE 0 END AS \"ContFalta\", " +
                                   " CASE WHEN CA.UnidadMedida = 3 THEN SUM(OED.TotalProcesar) ELSE 0 END AS \"PiezaTotal\", " +
                                   " CASE WHEN CA.UnidadMedida = 3 THEN SUM(OED.TotalProcesado) ELSE 0 END AS \"PiezaFalta\" " +
                                  " FROM OrdenEmbarque OE " +
                                  " INNER JOIN OrdenEmbarqueDetalle OED ON OED.ClaveOrdenEmbarque = OE.ClaveOrdenEmbarque " +
                                  " INNER JOIN CatalogoArticulo CA ON CA.ClaveArticulo = OED.ClaveArticulo " +
                                  " WHERE OE.ClaveFolioViaje = ?  " +
                                  " AND OE.ClaveConcesionario = ?" +
                                  " GROUP BY OE.ClaveConcesionario, CA.UnidadMedida " +
                                  " ) [SQ] GROUP BY SQ.ClaveConcesionario ";

                    var zresu = x.Query<SumarioEscaneo>(zcmd, App.zClaveFV.ToString(), App.zClaveCon);
                    if (zresu.Count >= 1)
                    {
                        if (zresu[0].PorProcesar > 0)
                        {
                            await Task.Delay(300);

                            await PopError("Debes egresar todos los artículos", "//PageEntregas");
                        }
                    }x.Dispose();
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
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

        public async Task PopError(string mensaje, string ruta)
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
                        Command = new Command(async () => await Shell.Current.GoToAsync(ruta)),
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
                        Command = new Command(async () => await Shell.Current.GoToAsync("//CerrarServicio")),
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
