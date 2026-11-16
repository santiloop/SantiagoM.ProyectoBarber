using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using ProyectoBarber.Server.Client;
using ProyectoBarber.Servicio.ServicioHTTP;
using ProyectoBarber.Server.Client.Auth;

// Paso 5 - Arranque del cliente Blazor WebAssembly (igual que Program.cs de la catedra).
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// HttpClient apuntando al mismo servidor que sirve la app (https://localhost:7128/).
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// Servicio HTTP de la catedra: las pages lo usan con @inject IHttpService http.
builder.Services.AddScoped<IHttpService, HttpService>();

// Paso 10 - Sesion del usuario (token del login). Usa el mismo HttpClient que el HttpService.
builder.Services.AddScoped<SesionService>();

await builder.Build().RunAsync();
