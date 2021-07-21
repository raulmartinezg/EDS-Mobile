using Movil.DataSQL;
using Movil.Models;
using Movil.Models.DB;
using Movil.ViewModels;
using Newtonsoft.Json;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Movil.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EncuestaView : ContentPage
    {
        EncuestaViewModel zVMEncuesta = new EncuestaViewModel();

        private int PreguntaActual;
        private int zcvePregunta;
        private string zRespPregunta = string.Empty;
        Button btnCliente;

        SfPopupLayout popupLayout;

        public EncuestaView()
        {
            InitializeComponent();
            popupLayout = new SfPopupLayout();
            BindingContext = zVMEncuesta;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            try
            {
                SQLiteDataEncuestaConcesionario encuestaConcesionario = new SQLiteDataEncuestaConcesionario();

                try
                {
                    List<EncuestaConcesionario> encuestaConcesionarios = await encuestaConcesionario.zGetAllItemsAsync(App.zClaveFV, App.zClaveCon);
                    PreguntaActual = 0;
                    if (zVMEncuesta.zcioEncuesta.Count() > 0)
                    {
                        if (encuestaConcesionarios.Count() == zVMEncuesta.zcioEncuesta.Count())
                        {
                            EncuestaTerminada();
                        }
                        else
                        {
                            await zMetMostrarPregunta(PreguntaActual);
                        }
                    }
                    else
                    {
                        await zMetMostrarPregunta(PreguntaActual);
                    }
                }
                catch (Exception ex)
                {
                    SQLiteDataLogError sQLite = new SQLiteDataLogError();
                    await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public async Task zMetMostrarPregunta(int zPregunta)
        {
            try
            {
                MensageGenerico zmsgr = await zVMEncuesta.zMetCargarDatosEncuesta();
                if (zmsgr.status)
                {
                    zcvePregunta = zVMEncuesta.zcioEncuesta.ElementAt(zPregunta).ClavePregunta;
                    zVMEncuesta.DescPregunta = zVMEncuesta.zcioEncuesta.ElementAt(zPregunta).Pregunta;
                    zlblDescripcionPrengunta.Text = zVMEncuesta.DescPregunta;
                    Device.BeginInvokeOnMainThread(() =>
                    {
                        zMetMostrarOpciones(zVMEncuesta.zcioEncuesta.ElementAt(zPregunta).NumOPC);
                    });
                }
                else
                {
                    Label lblEtiqueta = new Label
                    {
                        Text = zmsgr.Mensaje,
                        FontSize = 18,
                        TextColor = Color.FromHex("004D40"),
                        FontAttributes = FontAttributes.Bold,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };
                    zgrdbotones.Children.Add(lblEtiqueta, 2, 1);
                    zbtnSiguiente.IsVisible = false;
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public async void zMetMostrarOpciones(int zNumOpc)
        {
            try
            {
                int zrenglon = -1;
                zRespPregunta = "";

                for (int i = zgrdbotones.Children.Count - 1; i >= 0; --i)
                {
                    zgrdbotones.Children.RemoveAt(i);
                }

                if (zVMEncuesta.zcioEncuesta != null && zVMEncuesta.zcioEncuesta.Count() > 0)
                {
                    SQLiteDataOpcionPreguntas zdataopcpregunta = new SQLiteDataOpcionPreguntas();
                    zVMEncuesta.zcioOpcPreguntas = await zdataopcpregunta.zGetListItemsAsync(zNumOpc);

                    if (zVMEncuesta.zcioOpcPreguntas.Count() > 0)
                    {
                        foreach (var opc in zVMEncuesta.zcioOpcPreguntas)
                        {
                            zrenglon++;

                            Label lblEtiqueta = new Label
                            {
                                Text = opc.Descripcion.ToString(),
                                FontSize = 16,
                                FontAttributes = FontAttributes.Bold,
                                VerticalOptions = LayoutOptions.CenterAndExpand
                            };
                            zgrdbotones.Children.Add(lblEtiqueta, 2, zrenglon);

                            btnCliente = new Button
                            {
                                Text = opc.Secuencia.ToString(),
                                ImageSource = opc.NombreImagen,
                                BackgroundColor = Color.FromHex("003A79"),
                                TextColor = Color.White,
                                WidthRequest = Convert.ToDouble(70),
                                HeightRequest = Convert.ToDouble(70),
                                CornerRadius = 35,
                                HorizontalOptions = LayoutOptions.Center,
                                VerticalOptions = LayoutOptions.Center
                            };
                            btnCliente.Clicked += BtnCliente_Click;

                            zgrdbotones.Children.Add(btnCliente, 1, zrenglon);
                        }
                    }

                    zbtnSiguiente.IsVisible = true;

                    if ((zVMEncuesta.zcioEncuesta.Count() - 1) == PreguntaActual)
                    {
                        zbtnSiguiente.Text = "Terminar";
                    }
                    else
                    {
                        zbtnSiguiente.Text = "Siguiente";
                    }
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        public async void BtnCliente_Click(object sender, EventArgs args)
        {
            try
            {
                if (btnCliente != null)
                {
                    ViewExtensions.CancelAnimations(btnCliente);
                    await ViewExtensions.ScaleTo(btnCliente, 1, 150);
                }
                btnCliente = ((Button)(sender));
                zRespPregunta = (((Button)(sender)).Text);

                await ViewExtensions.ScaleTo(btnCliente, 1.5, 150);
                await ViewExtensions.RelRotateTo(btnCliente, 360, 150);
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        private async void zbtnSiguiente_Clicked(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(zRespPregunta))
                {
                    zVMEncuesta.zvarEnc_Conces.Add(
                    new EncuestaConcesionario()
                    {
                        ClaveFolioViaje = App.zClaveFV,
                        ClaveConcesionario = Convert.ToInt32(App.zClaveCon),
                        ClavePregunta = zcvePregunta,
                        Respuesta = zRespPregunta,
                        Fecha = DateTime.Now,
                        Observaciones = "",
                        NumEmpleado = App.zUsuario,
                        ClaveEstatus = 0,
                        Sincronizado = 0
                    });

                    if (zbtnSiguiente.Text == "Terminar")
                    {
                        SQLiteDataEncuestaConcesionario zcioEncuestaConces = new SQLiteDataEncuestaConcesionario();
                        MensageGenerico zmsg = await zcioEncuestaConces.zAddItemsAsync(zVMEncuesta.zvarEnc_Conces);

                        if (zmsg.status)
                        {
                            SQLiteDataConcesionario concesionario = new SQLiteDataConcesionario();
                            Concesionario con = await concesionario.zGetItemAsync(App.zClaveCon);
                            con.Sincronizado = 0;
                            await concesionario.zUpdateItemAsync(con);

                            SQLiteDataFolioViaje folioViaje = new SQLiteDataFolioViaje();
                            FolioViaje folio = await folioViaje.zGetItemAsync(App.zClaveFV);
                            folio.Sincronizado = 0;
                            await folioViaje.zUpdateItemAsync(folio);

                            SQLiteDataOrdenEmbarque ordenEmbarque = new SQLiteDataOrdenEmbarque();
                            OrdenEmbarque orden = await ordenEmbarque.zGetOrdenesEmbarqueConcesionarioAsync(con.ClaveConcesionario,folio.ClaveFolioViaje);
                            orden.Sincronizado = 0;
                            await ordenEmbarque.zUpdateItemAsync(orden);

                            EncuestaTerminada();
                        }
                        else
                            MostrarError(zmsg.Mensaje);
                    }
                    else
                    {
                        PreguntaActual++;
                        zMetMostrarPregunta(PreguntaActual);
                    }
                }
                else
                {
                    MostrarError("Porfavor agregue su respuesta en la pregunta actual.");
                }
            }
            catch (Exception err)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(err), Fecha = DateTime.Now });
                MostrarError(err.Message);
            }
            finally
            {
                zRespPregunta = "";
            }
        }

        public void EncuestaTerminada()
        {
            for (int i = zgrdbotones.Children.Count - 1; i >= 0; --i)
            {
                zgrdbotones.Children.RemoveAt(i);
            }
            zlblDescripcionPrengunta.Text = "";

            Button btnEnd = new Button
            {
                Text = "Terminar",
                TextColor = Color.White,
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Color.DarkSlateBlue,
                Command = new Command(async () => { await Shell.Current.GoToAsync("//PageSalidas"); Shell.Current.FlyoutIsPresented = false; })
            };

            Label lblEtiqueta = new Label
            {
                Text = "Su opinion nos interesa." + Environment.NewLine + "GRACIAS POR SU TIEMPO!! ",
                FontSize = 18,
                TextColor = Color.FromHex("004D40"),
                FontAttributes = FontAttributes.Bold,
                VerticalOptions = LayoutOptions.CenterAndExpand
            };
            zgrdbotones.Children.Add(btnEnd, 1, 1);
            zbtnSiguiente.IsVisible = false;
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
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }
        }

        protected override bool OnBackButtonPressed()
        {
            var vm = (EncuestaViewModel)BindingContext;
            vm.PopBack("¿Desea cerrar servicio?");
            return true;
        }
    }
}