using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Server.Controllers
{
    // ==================================================================
    // AUTH CONTROLLER  -  ruta base: api/auth   (paso 10, modelo de UDX)
    // CU01 - Iniciar sesion: POST api/auth/login devuelve un token JWT que
    //        dura una jornada (12 horas). El cliente lo manda en cada pedido.
    //        La contrasena se compara contra el HASH guardado (nunca en texto).
    // Cambiar la propia contrasena: PUT api/auth/contrasena
    // ==================================================================
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        public const int HorasJornada = 12;

        private readonly IUsuarioRepositorio repositorio;
        private readonly IConfiguration configuration;
        private readonly PasswordHasher<Usuario> hasher = new();

        public AuthController(IUsuarioRepositorio repositorio, IConfiguration configuration)
        {
            this.repositorio = repositorio;
            this.configuration = configuration;
        }

        // ------------------------------------------------------------------
        // CU01 - Iniciar sesion
        // POST api/auth/login   body: { "nombreUsuario": "admin", "contrasena": "..." }
        // ------------------------------------------------------------------
        [AllowAnonymous]
        [HttpPost("login")]
        public async Task<ActionResult<LoginRespuestaDTO>> Login(LoginDTO DTO)
        {
            var usuario = await repositorio.SelectByNombreUsuario(DTO.NombreUsuario);
            if (usuario is null ||
                hasher.VerifyHashedPassword(usuario, usuario.Contrasena, DTO.Contrasena) == PasswordVerificationResult.Failed)
                return Unauthorized("Usuario o contraseña incorrectos.");

            if (!usuario.Activo)
                return Unauthorized("El usuario está desactivado. Hablá con el dueño.");

            var expira = DateTime.Now.AddHours(HorasJornada);
            return Ok(new LoginRespuestaDTO
            {
                Token = GenerarToken(usuario, expira),
                NombreUsuario = usuario.NombreUsuario,
                Nombre = usuario.Nombre,
                Rol = usuario.Rol,
                Expira = expira,
                DebeCambiarContrasena = usuario.DebeCambiarContrasena
            });
        }

        // ------------------------------------------------------------------
        // Cambiar la propia contraseña (tambien la primera vez que se entra).
        // PUT api/auth/contraseña   body: { "actual": "...", "nueva": "..." }
        // ------------------------------------------------------------------
        [Authorize]
        [HttpPut("contrasena")]
        public async Task<ActionResult<bool>> CambiarContrasena(CambiarContrasenaDTO DTO)
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                return Unauthorized("Sesión inválida.");

            var usuario = await repositorio.SelectById(id);
            if (usuario is null)
                return NotFound("No existe el usuario.");

            if (hasher.VerifyHashedPassword(usuario, usuario.Contrasena, DTO.Actual) == PasswordVerificationResult.Failed)
                return BadRequest("La contraseña actual no es correcta.");

            if (DTO.Nueva == DTO.Actual)
                return BadRequest("La contraseña nueva tiene que ser distinta de la actual.");

            usuario.Contrasena = hasher.HashPassword(usuario, DTO.Nueva);
            usuario.DebeCambiarContrasena = false;
            await repositorio.Update(usuario);
            return Ok(true);
        }

        // Token con el id, el usuario, el nombre y el rol (para [Authorize(Roles = ...)]).
        private string GenerarToken(Usuario usuario, DateTime expira)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Name, usuario.NombreUsuario),
                new Claim("nombre", usuario.Nombre),
                new Claim(ClaimTypes.Role, usuario.Rol)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!));
            var credenciales = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: expira,
                signingCredentials: credenciales);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
