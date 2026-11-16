using Microsoft.EntityFrameworkCore;
using ProyectoBarber.BD;
using ProyectoBarber.Repositorio;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;   // Paso 10 - login con token JWT (modelo UDX)
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using ProyectoBarber.Shared.DTO;

var builder = WebApplication.CreateBuilder(args);

#region Servicios

var connectionString = builder.Configuration
                    .GetConnectionString("AppConn") ??
                    throw new InvalidOperationException("La conexión a la base de datos no funciona.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// Paso 10 - Swagger con boton "Authorize" para pegar el token del login.
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Pegá el token de POST api/auth/login (sin 'Bearer ', solo el token)"
    });
    options.AddSecurityRequirement(doc => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer", doc, null),
            new List<string>()
        }
    });
});

// Paso 10 - Autenticacion con token JWT. Los datos (clave, emisor) estan en appsettings.json.
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });
builder.Services.AddAuthorization();
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

#region Inyeccion de dependencias
builder.Services.AddScoped<IBarberoRepositorio, BarberoRepositorio>();
builder.Services.AddScoped<ITurnoRepositorio, TurnoRepositorio>();
builder.Services.AddScoped<IClienteRepositorio, ClienteRepositorio>();
builder.Services.AddScoped<IServicioRepositorio, ServicioRepositorio>();      // Paso 1 - CU09
builder.Services.AddScoped<IEstadoTurnoRepositorio, EstadoTurnoRepositorio>(); // Paso 1 - estados del turno
builder.Services.AddScoped<IHorarioLocalRepositorio, HorarioLocalRepositorio>();     // Paso 2 - CU15
builder.Services.AddScoped<IHorarioBarberoRepositorio, HorarioBarberoRepositorio>(); // Paso 2 - CU08
builder.Services.AddScoped<IDiaNoLaborableRepositorio, DiaNoLaborableRepositorio>(); // Paso 2 - CU16
builder.Services.AddScoped<IValidadorAgenda, ValidadorAgenda>();                     // Paso 3 - reglas de agenda
builder.Services.AddScoped<IHistorialEstadoTurnoRepositorio, HistorialEstadoTurnoRepositorio>(); // Paso 4 - historial de estados
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();                // Paso 10 - CU01 y CU11
builder.Services.AddScoped<IAvisoRepositorio, AvisoRepositorio>();                    // Paso 12 - CU13 avisos
#endregion

#endregion

var app = builder.Build();

// Paso 10 - Usuario inicial: si no hay ningun dueno, se crea "admin" con contrasena
// "admin123" (encriptada). Al entrar por primera vez el sistema obliga a cambiarla.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    if (!db.Usuarios.Any(u => u.Rol == Roles.Dueno))
    {
        var admin = new Usuario
        {
            NombreUsuario = "admin",
            Nombre = "Administrador",
            Rol = Roles.Dueno,
            Activo = true,
            DebeCambiarContrasena = true
        };
        admin.Contrasena = new PasswordHasher<Usuario>().HashPassword(admin, "admin123");
        db.Usuarios.Add(admin);
        db.SaveChanges();
    }
}

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
app.UseBlazorFrameworkFiles();   // Paso 5 - sirve el cliente Blazor WebAssembly (modelo de la catedra)
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();   // Paso 10 - lee el token de cada pedido
app.UseAuthorization();    // Paso 10 - aplica [Authorize] y los roles

app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");   // Paso 5 - index.html esta en ProyectoBarber.Server.Client/wwwroot

#endregion

app.Run();
