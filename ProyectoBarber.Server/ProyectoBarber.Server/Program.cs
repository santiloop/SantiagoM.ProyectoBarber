using Microsoft.EntityFrameworkCore;
using ProyectoBarber.BD;
using ProyectoBarber.Repositorio;
using ProyectoBarber.BD.Datos;

var builder = WebApplication.CreateBuilder(args);

#region Servicios
var connectionString = builder.Configuration
                    .GetConnectionString("connSql") ??
                    throw new InvalidOperationException("La conexión a la base de datos no funciona.");

builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddSwaggerGen();

// Nota: la clase se llama AppDBContext (DB en mayúsculas) en ProyectoBarber.BD.Datos, ajustar si es necesario.
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IBarberRepositorio, BarberRepositorio>();

#endregion

var app = builder.Build();

#region Middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();

app.UseRouting();

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
#endregion

app.Run();
//using Microsoft.EntityFrameworkCore;
//using ProyectoBarber.BD.Datos;
//using ProyectoBarber.Server.Client.Pages;
//using ProyectoBarber.Server.Components;

//var builder = WebApplication.CreateBuilder(args);

//#region servicios
//var connectionString = builder.Configuration
//    .GetConnectionString("connSql") ?? 
//    throw new InvalidOperationException("La conexion a la base de datos no funciona.");
//builder.Services.AddDbContext<AppDBContext>(options =>
//    options.UseSqlServer(connectionString));

//builder.Services.AddControllersWithViews();
//builder.Services.AddRazorPages();
//builder.Services.AddRazorComponents()
//    .AddInteractiveWebAssemblyComponents();
//#endregion

//#region Servicios
//builder.Services.AddControllers();
//#endregion

//// cargar servicios
//#region cargar servicios
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents()
//    .AddInteractiveWebAssemblyComponents();
//#endregion

//var app = builder.Build();

//#region Middleware
//app.MapControllers();
//#endregion

//#region middleware
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error"); 
//    app.UseHsts();
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//#endregion 

//#region configuracion de servicios
//// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseWebAssemblyDebugging();
//}
//else
//{
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
//    app.UseHsts();
//}
//app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
//app.UseHttpsRedirection();

//app.UseAntiforgery();

//app.MapStaticAssets();
//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode()
//    .AddInteractiveWebAssemblyRenderMode()
//    .AddAdditionalAssemblies(typeof(ProyectoBarber.Server.Client._Imports).Assembly);
//#endregion

//app.Run();
