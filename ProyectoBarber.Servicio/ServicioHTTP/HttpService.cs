using System;
using System.Collections.Generic;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;

// Paso 5 - Copiado de la catedra (Proyecto2026WA.Servicio/ServicioHTTP), solo cambia el namespace.
// Lo usan las pages de Blazor para llamar a la API (api/servicio, api/turno, etc.).
namespace ProyectoBarber.Servicio.ServicioHTTP
{
    public class HttpService : IHttpService
    {
        private readonly HttpClient http;

        public HttpService(HttpClient http)
        {
            this.http = http;
        }

        public async Task<HttpResp<T?>> GetAsync<T>(string url)
        {
            try
            {
                var response = await http.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<T?>(default, true, response, await MensajeDelServidor(response));
                }

                var dato = await response.Content.ReadFromJsonAsync<T>();

                return new HttpResp<T?>(dato, false, response, null);

            }
            catch (Exception e)
            {
                return new HttpResp<T?>(default, true, null, e.Message);
            }

        }

        public async Task<HttpResp<TResp?>> PostAsync<T, TResp>(string url, T DTO)
        {
            try
            {
                var JsonEnviar = JsonSerializer.Serialize(DTO);
                var cuerpo = new StringContent(JsonEnviar, Encoding.UTF8, "application/json");

                var response = await http.PostAsync(url, cuerpo);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResp?>(default, true, response, await MensajeDelServidor(response));
                }
                else
                {
                    var dato = await response.Content.ReadFromJsonAsync<TResp>();
                    return new HttpResp<TResp?>(dato, false, response, null);
                }
            }
            catch (Exception e)
            {
                return new HttpResp<TResp?>(default, true, null, e.Message);
            }
        }

        public async Task<HttpResp<TResp?>> PutAsync<T, TResp>(string url, T DTO)
        {
            try
            {
                var JsonEnviar = JsonSerializer.Serialize(DTO);
                var cuerpo = new StringContent(JsonEnviar,
                                               Encoding.UTF8,
                                               "application/json");

                var response = await http.PutAsync(url, cuerpo);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResp?>(default, true, response, await MensajeDelServidor(response));
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var dato = DesSerializar<TResp>(content);

                    //var dato = await response.Content.ReadFromJsonAsync<TResp>();
                    return new HttpResp<TResp?>(dato, false, response, null);
                }
            }
            catch (Exception e)
            {
                return new HttpResp<TResp?>(default, true, null, e.Message);
                //+ " " + e.InnerException?.Message 
                //+ " " + e.StackTrace);
            }
        }

        public async Task<HttpResp<TResp?>> DeleteAsync<TResp>(string url)
        {
            try
            {
                var response = await http.DeleteAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new HttpResp<TResp?>(default, true, response, await MensajeDelServidor(response));
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var dato = DesSerializar<TResp>(content);
                    //var dato = await response.Content.ReadFromJsonAsync<TResp>();
                    return new HttpResp<TResp?>(dato, false, response, null);
                }
            }
            catch (Exception e)
            {
                return new HttpResp<TResp?>(default, true, null, e.Message);
            }
        }
        // Paso 7 - Cuando la API responde con error (por ejemplo BadRequest("Ya existe un
        // servicio llamado Corte.")) se lee ese texto para mostrarlo en la pantalla.
        // Si no hay texto, HttpResp usa el mensaje generico segun el StatusCode (como antes).
        private static async Task<string?> MensajeDelServidor(HttpResponseMessage response)
        {
            try
            {
                if (response.Content.Headers.ContentType?.MediaType != "text/plain")
                    return null;
                var texto = await response.Content.ReadAsStringAsync();
                return string.IsNullOrWhiteSpace(texto) ? null : texto;
            }
            catch
            {
                return null;
            }
        }

        private T? DesSerializar<T>(string content)
        {
            return JsonSerializer.Deserialize<T>(content,
                                                new JsonSerializerOptions
                                                {
                                                    PropertyNameCaseInsensitive = true
                                                });
        }
    }
}
