using Movil.Helpers;
using Movil.Models;
using Movil.Models.Request;
using Movil.Models.Response;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;

namespace Movil
{
    public static class ServiceWebClient
    {
        private static string zvarURL;

        public static string zURL
        {
            get
            { return zvarURL; }
            set
            {
                zvarURL = value;
            }
        }

        public static async Task<UserLoginResponse> LoginValidateResult(UserLoginRequest item)
        {
            UserLoginResponse zResult = new UserLoginResponse();

            try
            {
                Uri zUri = new Uri(string.Format(zURL, string.Empty));

                if (Connectivity.NetworkAccess == NetworkAccess.None)
                {
                    return zResult = new UserLoginResponse() { success = false, message = "No tiene conectividad Wi-fi." };
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    StringContent content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                    var zanswer = await client.PostAsync(zUri, content);

                    if (!zanswer.IsSuccessStatusCode)
                    {
                        return zResult = new UserLoginResponse() { success = false, message = "Error al intentar establecer conexion con el servidor." };
                    }
                    else
                        return JsonConvert.DeserializeObject<UserLoginResponse>(await zanswer.Content.ReadAsStringAsync());
                    client.Dispose();

                }
            }
            catch (HttpRequestException ex)
            {
                return zResult = new UserLoginResponse() { success = false, message = ex.Message };
            }
            catch (OutOfMemoryException ex)
            {
                return zResult = new UserLoginResponse() { success = false, message = ex.Message };
            }
            catch (Exception ex)
            {
                return zResult = new UserLoginResponse() { success = false, message = ex.Message };
            }
        }

        public static async Task<DescargaViaje> GetTravels(ViajesRequest item)
        {
            DescargaViaje zResult = new DescargaViaje();

            try
            {
                Uri zUri = new Uri(string.Format(zURL, string.Empty));

                if (Connectivity.NetworkAccess == NetworkAccess.None)
                {
                    return zResult = new DescargaViaje() { success = false, message = "No tiene conectividad Wi-fi." };
                }

                using (HttpClient client = new HttpClient())
                {
                    StringContent content = new StringContent(JsonConvert.SerializeObject(item), Encoding.UTF8, "application/json");
                    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", App.zToken);
                    var zanswer = await client.PostAsync(zUri, content);

                    if (!zanswer.IsSuccessStatusCode)
                    {
                        return zResult = new DescargaViaje() { success = false, message = "Error al intentar establecer conexion con el servidor." };
                    }
                    else
                    {
                        var otroObj = JsonConvert.DeserializeObject<DescargaViaje>(await zanswer.Content.ReadAsStringAsync());

                        return otroObj;
                    }client.Dispose();

                }
            }
            catch (HttpRequestException ex)
            {
                return zResult = new DescargaViaje() { success = false, message = ex.Message };
            }
            catch (OutOfMemoryException ex)
            {
                return zResult = new DescargaViaje() { success = false, message = ex.Message };
            }
            catch (Exception ex)
            {
                return zResult = new DescargaViaje() { success = false, message = ex.Message };
            }
        }

        public static async Task<OrdenEmbarqueDetalleResponse> OrdenEmbarqueDetalle(List<ArticuloObject> articulos)
        {
            OrdenEmbarqueDetalleResponse detalleResponse;

            try
            {
                Uri zUri = new Uri(string.Format(zURL, string.Empty));

                if (Connectivity.NetworkAccess == NetworkAccess.None)
                {
                    return detalleResponse = new OrdenEmbarqueDetalleResponse() { success = false, message = "No tiene conectividad Wi-fi." };
                }

                using (HttpClient client = new HttpClient())
                {
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    var httpContent = new StringContent(JsonConvert.SerializeObject(articulos), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(CDGlobales.zURLOrdenEmbarqueDetalle, httpContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        return detalleResponse = new OrdenEmbarqueDetalleResponse() { success = false, message = "Error al intentar establecer conexion con el servidor." };
                    }
                    else
                    {
                        return detalleResponse = new OrdenEmbarqueDetalleResponse() { success = true, message = await response.Content.ReadAsStringAsync() };
                    }client.Dispose();

                }
            }
            catch (HttpRequestException ex)
            {
                return detalleResponse = new OrdenEmbarqueDetalleResponse() { success = false, message = ex.Message };
            }
            catch (OutOfMemoryException ex)
            {
                return detalleResponse = new OrdenEmbarqueDetalleResponse() { success = false, message = ex.Message };
            }
            catch (Exception ex)
            {
                return new OrdenEmbarqueDetalleResponse() { success = false, message = ex.Message };
            }
        }

        public static async Task<IncidenciaResponse> Incidencia(IncidenciaObject incidencia)
        {
            try
            {
                Uri zUri = new Uri(string.Format(zURL, string.Empty));

                if (Connectivity.NetworkAccess == NetworkAccess.None)
                {
                    return new IncidenciaResponse() { success = false, message = "No tiene conectividad Wi-fi." };
                }

                using (HttpClient client = new HttpClient())
                {
                    var httpContent = new StringContent(JsonConvert.SerializeObject(incidencia), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(CDGlobales.zURLIncidencia, httpContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        return new IncidenciaResponse() { success = false, message = "Error al intentar establecer conexion con el servidor." };
                    }
                    else
                    {
                        return new IncidenciaResponse() { success = true, message = await response.Content.ReadAsStringAsync() };
                    }client.Dispose();

                }
            }
            catch (HttpRequestException ex)
            {
                return new IncidenciaResponse() { success = false, message = ex.Message };
            }
            catch (OutOfMemoryException ex)
            {
                return new IncidenciaResponse() { success = false, message = ex.Message };
            }
            catch (Exception ex)
            {
                return new IncidenciaResponse() { success = false, message = ex.Message };
            }
        }

        public static async Task<MasterResponse> Master(MasterObject master)
        {
            try
            {
                Uri zUri = new Uri(string.Format(zURL, string.Empty));

                if (Connectivity.NetworkAccess == NetworkAccess.None)
                {
                    return new MasterResponse() { success = false, message = "No tiene conectividad Wi-fi." };
                }

                using (HttpClient client = new HttpClient())
                {
                    string masterJson = JsonConvert.SerializeObject(master);

                    var httpContent = new StringContent(JsonConvert.SerializeObject(master), Encoding.UTF8, "application/json");
                    HttpResponseMessage response = await client.PostAsync(CDGlobales.zURLMaster, httpContent);

                    if (!response.IsSuccessStatusCode)
                    {
                        return new MasterResponse() { success = false, message = "Error al intentar establecer conexion con el servidor." };
                    }
                    else
                    {
                        return new MasterResponse() { success = true, message = await response.Content.ReadAsStringAsync() };
                    }client.Dispose();

                }
            }
            catch (HttpRequestException ex)
            {
                return new MasterResponse() { success = false, message = ex.Message };
            }
            catch (OutOfMemoryException ex)
            {
                return new MasterResponse() { success = false, message = ex.Message };
            }
            catch (Exception ex)
            {
                return new MasterResponse() { success = false, message = ex.Message };
            }
        }
    }
}