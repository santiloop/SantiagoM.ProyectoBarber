using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.JSInterop;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Server.Client.Auth
{
    // ==================================================================
    // Paso 10 - SESION DEL USUARIO en el navegador (CU01).
    // Guarda los datos del login (token, nombre, rol, vencimiento) en el
    // localStorage del navegador, para que al recargar la pagina no haya
    // que volver a entrar mientras dure la jornada (12 horas).
    // Pone el token en el HttpClient: asi TODOS los pedidos del HttpService
    // de la catedra van con "Authorization: Bearer <token>".
    // ==================================================================
    public class SesionService
    {
        private const string Clave = "proyectobarber_sesion";
        private readonly HttpClient http;
        private readonly IJSRuntime js;

        public SesionService(HttpClient http, IJSRuntime js)
        {
            this.http = http;
            this.js = js;
        }

        public LoginRespuestaDTO? Datos { get; private set; }

        // Avisa al menu y al diseno que se entro o se salio.
        public event Action? OnCambio;

        // Paso 12 - La pagina de Avisos marco todo como leido: la campanita se pone en 0.
        public event Action? OnAvisosLeidos;
        public void AvisosLeidos() => OnAvisosLeidos?.Invoke();

        public bool Logueado => Datos != null && Datos.Expira > DateTime.Now;
        public bool EsDueno => Logueado && Datos!.Rol == Roles.Dueno;
        public string Nombre => Datos?.Nombre ?? "";
        public string RolNombre => Roles.Nombre(Datos?.Rol ?? "");

        // Al abrir la app: recupera la sesion guardada si todavia no vencio.
        public async Task Cargar()
        {
            try
            {
                var json = await js.InvokeAsync<string?>("localStorage.getItem", Clave);
                if (string.IsNullOrEmpty(json)) return;

                var datos = JsonSerializer.Deserialize<LoginRespuestaDTO>(json);
                if (datos != null && datos.Expira > DateTime.Now)
                    Aplicar(datos);
                else
                    await Cerrar();
            }
            catch
            {
                Datos = null;
            }
        }

        public async Task Iniciar(LoginRespuestaDTO datos)
        {
            Aplicar(datos);
            await Guardar();
            OnCambio?.Invoke();
        }

        // Despues de cambiar la contrasena obligatoria.
        public async Task ContrasenaCambiada()
        {
            if (Datos == null) return;
            Datos.DebeCambiarContrasena = false;
            await Guardar();
            OnCambio?.Invoke();
        }

        public async Task Cerrar()
        {
            Datos = null;
            http.DefaultRequestHeaders.Authorization = null;
            try { await js.InvokeVoidAsync("localStorage.removeItem", Clave); } catch { }
            OnCambio?.Invoke();
        }

        private void Aplicar(LoginRespuestaDTO datos)
        {
            Datos = datos;
            http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", datos.Token);
        }

        private async Task Guardar()
        {
            try { await js.InvokeVoidAsync("localStorage.setItem", Clave, JsonSerializer.Serialize(Datos)); } catch { }
        }
    }
}
