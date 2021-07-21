using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Movil.Helpers
{
    public enum zEnumMetodoConsumir : int
    {
        queryexecutereturn = 1,
        querytablesreturn = 2,
    }
    public class ServiceWebClientApp<T>
    {
        private static string zvarURL;
        private Response<T> result = null;

        public string zURL
        {
            get
            { return zvarURL; }
            //set
            //{
            //    zvarURL = value;
            //}
        }

        public ServiceWebClientApp()
        { }

        private void zUriServiceWeb(zEnumMetodoConsumir zMetConsumir)
        {
            switch (zMetConsumir)
            {
                case zEnumMetodoConsumir.queryexecutereturn:
                    zvarURL = CDGlobales.zURLMetGen_Request + "queryexecutereturn";
                    break;
                case zEnumMetodoConsumir.querytablesreturn:
                    zvarURL = CDGlobales.zURLMetGen_Request + "querytablesreturn";
                    break;
            }
        }

        private async Task<bool> zObtenerToken()
        {
            if (App.zToken == String.Empty || App.zToken == null)
            {
                ServiceWebClient.zURL = Helpers.CDGlobales.zURLLogin;
                UserLoginRequest zusrlogReq = new UserLoginRequest();
                zusrlogReq.data.bdCc = 22;
                zusrlogReq.data.bdSp = "SPQRY_ValidaCredencialV2";
                zusrlogReq.data.bdCat = "SIDTUM_PROD";
                zusrlogReq.data.bdSch = "SidMovil";
                zusrlogReq.data.encV = 2;
                zusrlogReq.filter.usr = App.zUsuario;
                zusrlogReq.filter.pwd = App.zPassword;

                UserLoginResponse resp = await ServiceWebClient.LoginValidateResult(zusrlogReq);

                if (!resp.success)
                {
                    return false;
                }
                App.zToken = resp.token;
                return true;
            }
            else
                return true;

        }

        public async Task BackEndResultAsync(BodyRequest item, zEnumMetodoConsumir zopcConsumir)
        {
            try
            {
                bool zblnToken = await zObtenerToken();
                if (!zblnToken)
                {
                    result = new Response<T>
                    {
                        zSuccess = false,
                        zMessage = "Error de conexion al intentar obtener la llave hacia el servidor"
                    };
                    return;
                }
                zUriServiceWeb(zopcConsumir);
                Uri zUri = new Uri(string.Format(zvarURL, string.Empty));

                if (Connectivity.NetworkAccess == NetworkAccess.None)
                {
                    result = new Response<T>
                    {
                        zSuccess = false,
                        zMessage = "No tiene conectividad Wi-fi."
                    };
                    return;
                }

                using (HttpClient client = new HttpClient())
                {
                    var x = JsonConvert.SerializeObject(item);
                    StringContent content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.zToken);

                    var zanswer = await client.PostAsync(zUri, content);

                    if (!zanswer.IsSuccessStatusCode)
                    {
                        result = new Response<T>
                        {
                            zSuccess = false,
                            zMessage = "Error al intentar establecer conexion con el servidor."
                        };
                        return;
                    }
                    else
                    {
                        try
                        {
                            result = new Response<T>
                            {
                                zSuccess = true,
                                zResponse = JsonConvert.DeserializeObject<T>(await zanswer.Content.ReadAsStringAsync())
                            };
                        }
                        catch (NullReferenceException err)
                        {
                            result = new Response<T>
                            {
                                zSuccess = true,
                                zMessage = err.Message,

                            };
                        }
                    }
                }
            }
            catch (HttpRequestException ex)
            {
                result = new Response<T>
                {
                    zSuccess = true,
                    zMessage = ex.Message,

                };
            }
            catch (Exception ex)
            {
                result = new Response<T>
                {
                    zSuccess = true,
                    zMessage = ex.Message,
                };
            }
        }

        public Response<T> zResponseSWApp { get { return result; } }
    }
}
