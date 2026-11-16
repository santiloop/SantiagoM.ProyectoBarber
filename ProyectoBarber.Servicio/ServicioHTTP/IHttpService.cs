// Paso 5 - Copiado de la catedra (Proyecto2026WA.Servicio/ServicioHTTP), solo cambia el namespace.
// Lo usan las pages de Blazor para llamar a la API (api/servicio, api/turno, etc.).
namespace ProyectoBarber.Servicio.ServicioHTTP
{
    public interface IHttpService
    {
        Task<HttpResp<TResp?>> DeleteAsync<TResp>(string url);
        Task<HttpResp<T?>> GetAsync<T>(string url);
        Task<HttpResp<TResp?>> PostAsync<T, TResp>(string url, T DTO);
        Task<HttpResp<TResp?>> PutAsync<T, TResp>(string url, T DTO);
    }
}