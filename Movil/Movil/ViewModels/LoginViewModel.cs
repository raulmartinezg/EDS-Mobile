using Acr.UserDialogs;
using Movil.DataSQL;
using Movil.Helpers;
using Movil.Models;
using Movil.Models.DB;
using Movil.Models.Response;
using Movil.Services.Routing;
using Newtonsoft.Json;
using Splat;
using Syncfusion.XForms.PopupLayout;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Movil
{
    public class LoginViewModel : ObservableINotifyObject
    {
        private IRoutingService _navigationService;

        public SQLiteDataOperador DSOperador;

        public UserLoginRequest zusrlogReq { get; set; } = new UserLoginRequest();

        public ICommand ExecuteLogin { get; set; }

        SfPopupLayout popupLayout;

        public LoginViewModel(IRoutingService navigationService = null)
        {
            _navigationService = navigationService ?? Locator.Current.GetService<IRoutingService>();
            ExecuteLogin = new Command(async () => await zInicioSesionUsuario());
            popupLayout = new SfPopupLayout();
        }

        public async Task<bool> zInicioSesionUsuario()
        {
            UserDialogs.Instance.ShowLoading("Cargando...");
            await Task.Delay(300);

            try
            {
                await Sincronizar();

                if (!int.TryParse(zusrlogReq.filter.usr, out int i) ||
                    zusrlogReq.filter.usr.Length == 0 ||
                    zusrlogReq.filter.pwd.Length == 0)
                {
                    UserDialogs.Instance.HideLoading();
                    return false;
                }

                DSOperador = new SQLiteDataOperador();
                App.zUsuario = zusrlogReq.filter.usr;
                App.zPassword = zusrlogReq.filter.pwd;

                Operador zOperador = new Operador();
                Operador zOperadorAnt = new Operador();

                ServiceWebClient.zURL = CDGlobales.zURLLogin;
                zusrlogReq.data.bdCc = 22;
                zusrlogReq.data.bdSp = "SPQRY_ValidaCredencialV2";
                zusrlogReq.data.bdCat = "SIDTUM_PROD";
                zusrlogReq.data.bdSch = "SidMovil";
                zusrlogReq.data.encV = 2;

                UserLoginResponse resp = await ServiceWebClient.LoginValidateResult(zusrlogReq);

                if (resp.success)
                {
                    App.zToken = resp.token;
                    zOperador = new Operador
                    {
                        FiOperador = resp.data.cus,
                        FiEstatus = resp.data.estatus,
                        FnOperador = zusrlogReq.filter.usr,
                        FcNombre = resp.data.nom,
                        FcExhorto = resp.data.exhorto,
                        FcPassword = zusrlogReq.filter.pwd,
                        FdIngreso = DateTime.Now
                    };

                    CDGlobales.ZExhorto = zOperador.FcExhorto;
                    zOperadorAnt = await DSOperador.zGetItemAsync(zusrlogReq.filter.usr);
                    if (zOperadorAnt != null)
                    {
                        _ = await DSOperador.zUpdateItemAsync(zOperador);
                    }
                    else
                    {
                        if (!await DSOperador.zAddItemAsync(zOperador))
                        {
                            MostrarError("Error al registrar usuario");
                        }

                    }
                    if (zOperador.FcPassword != zusrlogReq.filter.pwd)
                    {
                        UserDialogs.Instance.HideLoading();
                        MostrarError("Password incorrecto!");
                    }
                    else
                    {
                        UserDialogs.Instance.HideLoading();
                        App.zClaveOp = zOperador.FiOperador;
                        await _navigationService.NavigateTo("///Bienvenida");
                        return true;
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
                        zOperador = await DSOperador.zGetItemAsync(zusrlogReq.filter.usr);

                        if (zOperador != null)
                        {
                            if (zOperador.FcPassword != zusrlogReq.filter.pwd)
                            {
                                UserDialogs.Instance.HideLoading();
                                MostrarError("Password incorrecto!");
                            }
                            else
                            {
                                UserDialogs.Instance.HideLoading();
                                MostrarError("Modo OffLine activo");
                                App.zClaveOp = zOperador.FiOperador;
                                await _navigationService.NavigateTo("///Bienvenida");
                                return true;
                            }
                        }
                        else
                        {
                            MostrarError("Usuario no encontrado!");
                        }
                    }
                    else
                    {
                        UserDialogs.Instance.HideLoading();
                        MostrarError(resp.message);
                    }
                    UserDialogs.Instance.HideLoading();
                }
                return false;
            }
            catch (Exception err)
            {
                UserDialogs.Instance.HideLoading();

                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                _ = sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(err), Fecha = DateTime.Now });

                MostrarError(err.Message);

                return false;
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

        public async Task Sincronizar()
        {
            try
            {
                SQLiteDataFolioViaje sQLiteDataFolioViaje = new SQLiteDataFolioViaje();
                List<FolioViaje> folioViajes = (await sQLiteDataFolioViaje.zGetItemsAsync()).ToList();

                foreach (FolioViaje folioViaje in folioViajes)
                {
                    bool sinc = false;

                    if (folioViaje.Sincronizado == 0)
                    {
                        SQLiteDataIncidencia sQLiteDataIncidencia = new SQLiteDataIncidencia();
                        List<Incidencia> dbIncidencias = (await sQLiteDataIncidencia.zGetItemsAsync()).Where(x => x.Csr == folioViaje.ClaveFolioViaje && x.Sincronizado == 0).ToList();
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

                        if (incidencias.Count() > 0)
                        {
                            sinc = true;
                        }

                        SQLiteDataOrdenEmbarque sQLiteDataOrdenEmbarque = new SQLiteDataOrdenEmbarque();
                        List<OrdenEmbarque> ordenEmbarques = (await sQLiteDataOrdenEmbarque.zGetOrnesEmbarqueAsync(folioViaje.ClaveFolioViaje)).ToList();

                        foreach (OrdenEmbarque ordenEmbarque in ordenEmbarques)
                        {
                            if (ordenEmbarque.Sincronizado == 0)
                            {
                                SQLiteDataEncuestaConcesionario sQLiteDataEncuestaConcesionario = new SQLiteDataEncuestaConcesionario();
                                List<EncuestaConcesionario> encuestaConcesionarios = await sQLiteDataEncuestaConcesionario.zGetAllItemsAsync(folioViaje.ClaveFolioViaje, ordenEmbarque.ClaveConcesionario);
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

                                if (encuestas.Count() > 1)
                                {
                                    sinc = true;
                                }

                                dbIncidencias = (await sQLiteDataIncidencia.zGetItemsAsync()).Where(x => x.Csr == ordenEmbarque.ClaveConcesionario && x.Sincronizado == 0).ToList();

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

                                if (incidencias.Count() > 0)
                                {
                                    sinc = true;
                                }

                                List<ArticuloObject> articulos = BuscaArticulos(folioViaje.ClaveFolioViaje, ordenEmbarque.ClaveConcesionario);

                                if (articulos.Count() > 0)
                                {
                                    sinc = true;
                                }

                                SQLiteDataEntrada sQLiteDataEntrada = new SQLiteDataEntrada();
                                Entrada entrada = await sQLiteDataEntrada.zGetItemClaveConcesionarioAsync(ordenEmbarque.ClaveConcesionario);
                                RegistroObject registroEntrada = new RegistroObject();

                                if (entrada != null)
                                {
                                    if (entrada.ClaveConcesionario > 0)
                                    {
                                        registroEntrada = new RegistroObject()
                                        {
                                            claveConcesionario = entrada.ClaveConcesionario,
                                            empleado = entrada.Empleado,
                                            nombreEmpleado = entrada.NombreEmpleado,
                                            fecha = entrada.Fecha,
                                            firma = entrada.Firma
                                        };

                                        sinc = true;
                                    }
                                    else
                                    {
                                        registroEntrada = null;
                                    }
                                }

                                SQLiteDataSalida sQLiteDataSalida = new SQLiteDataSalida();
                                Salida Salida = await sQLiteDataSalida.zGetItemClaveConcesionarioAsync(ordenEmbarque.ClaveConcesionario);
                                RegistroObject registroSalida = new RegistroObject();

                                if (Salida != null)
                                {
                                    if (Salida.ClaveConcesionario > 0)
                                    {
                                        registroSalida = new RegistroObject()
                                        {
                                            claveConcesionario = Salida.ClaveConcesionario,
                                            empleado = Salida.Empleado,
                                            nombreEmpleado = Salida.NombreEmpleado,
                                            fecha = Salida.Fecha,
                                            firma = Salida.Firma
                                        };

                                        sinc = true;
                                    }
                                    else
                                    {
                                        registroSalida = null;
                                    }
                                }

                                if (sinc)
                                {
                                    MasterObject master = new MasterObject()
                                    {
                                        salida = registroSalida,
                                        entrada = registroEntrada,
                                        encuestas = encuestas,
                                        articulos = articulos,
                                        incidencias = incidencias,
                                        sincronizado = true
                                    };

                                    MasterResponse masterResponse = await ServiceWebClient.Master(master);

                                    if (masterResponse.success)
                                    {
                                        SQLiteDataConcesionario concesionario = new SQLiteDataConcesionario();
                                        Concesionario con = await concesionario.zGetItemAsync(ordenEmbarque.ClaveConcesionario);
                                        con.Sincronizado = 1;
                                        await concesionario.zUpdateItemAsync(con);

                                        ordenEmbarque.Sincronizado = 1;
                                        await sQLiteDataOrdenEmbarque.zUpdateItemAsync(ordenEmbarque);

                                        entrada.Sincronizado = 1;
                                        _ = await sQLiteDataEntrada.zUpdateItemAsync(entrada);

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
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception err)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                await sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(err), Fecha = DateTime.Now });
            }
        }

        public List<ArticuloObject> BuscaArticulos(int zClaveFV, int zClaveCon)
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
                                   " OED.TotalProcesado tpp " +
                                  " FROM OrdenEmbarque OE " +
                                  " INNER JOIN OrdenEmbarqueDetalle OED ON OED.ClaveOrdenEmbarque = OE.ClaveOrdenEmbarque " +
                                  " INNER JOIN CatalogoArticulo CA ON CA.ClaveArticulo = OED.ClaveArticulo " +
                                  " WHERE OE.ClaveFolioViaje = ?  " +
                                  " AND OE.ClaveConcesionario = ?";

                    var zresu = x.Query<ArticuloObject>(zcmd, zClaveFV, zClaveCon);
                    if (zresu.Count >= 1)
                    {
                        zresu = zresu.Where(z => z.tpr == z.tpp).ToList();
                        articulosRequests = new List<ArticuloObject>(zresu);
                    }
                    else
                    {
                        articulosRequests = new List<ArticuloObject>();
                    }
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });

                articulosRequests = new List<ArticuloObject>();
                MostrarError(ex.Message);
            }

            return articulosRequests;
        }

        public UserLoginResponse GetDummieData(UserLoginResponse resp)
        {
            try
            {
                string jsonFileName = "dummies.LoginService.json";
                var assembly = typeof(LoginViewModel).GetTypeInfo().Assembly;
                Stream stream = assembly.GetManifestResourceStream($"{assembly.GetName().Name}.{jsonFileName}");
                using (var reader = new StreamReader(stream))
                {
                    var jsonString = reader.ReadToEnd();

                    //Converting JSON Array Objects into generic list    
                    resp = JsonConvert.DeserializeObject<UserLoginResponse>(jsonString);
                }
            }
            catch (Exception ex)
            {
                SQLiteDataLogError sQLite = new SQLiteDataLogError();
                sQLite.zAddItemAsync(new LogError() { Mensaje = JsonConvert.SerializeObject(ex), Fecha = DateTime.Now });
            }

            return resp;
        }
    }
}
