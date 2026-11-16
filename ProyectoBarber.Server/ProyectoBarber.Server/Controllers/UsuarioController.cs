using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Server.Controllers
{
    // ==================================================================
    // USUARIO CONTROLLER  -  ruta base: api/usuario   (paso 10)
    // CU11 - Gestionar usuarios. Actor: SOLO el Dueno.
    //   GET    api/usuario                 -> listado (sin contrasenas)
    //   GET    api/usuario/{id}
    //   POST   api/usuario                 -> alta con contrasena inicial (la cambia al entrar)
    //   PUT    api/usuario/{id}            -> nombre, rol y activo
    //   PUT    api/usuario/{id}/contrasena -> blanqueo (la cambia al entrar)
    // Reglas: el nombre de usuario no se repite; siempre tiene que quedar al menos
    // un Dueno activo; el dueno no se puede desactivar ni quitar el rol a si mismo.
    // No hay borrado: un usuario se desactiva (sus cambios quedan en el historial).
    // ==================================================================
    [Authorize(Roles = Roles.Dueno)]
    [ApiController]
    [Route("api/usuario")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioRepositorio repositorio;
        private readonly PasswordHasher<Usuario> hasher = new();

        public UsuarioController(IUsuarioRepositorio repositorio)
        {
            this.repositorio = repositorio;
        }

        [HttpGet] //api/usuario
        public async Task<ActionResult<List<UsuarioDTO>>> Get()
        {
            var lista = await repositorio.Select();
            return Ok(lista.OrderByDescending(u => u.Activo).ThenBy(u => u.Nombre).Select(ADTO).ToList());
        }

        [HttpGet("{id:int}")] //api/usuario/1
        public async Task<ActionResult<UsuarioDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el usuario con id: {id}.");
            return Ok(ADTO(entidad));
        }

        [HttpPost] //api/usuario
        public async Task<ActionResult<int>> Post(UsuarioDTO DTO)
        {
            var error = Validar(DTO);
            if (error != null) return BadRequest(error);

            if (string.IsNullOrWhiteSpace(DTO.Contrasena) || DTO.Contrasena.Length < 6)
                return BadRequest("La contraseña inicial tiene que tener al menos 6 caracteres.");

            if (await repositorio.ExisteNombreUsuario(DTO.NombreUsuario, 0))
                return BadRequest($"Ya existe el usuario {DTO.NombreUsuario.Trim()}.");

            var entidad = new Usuario
            {
                NombreUsuario = DTO.NombreUsuario.Trim(),
                Nombre = DTO.Nombre.Trim(),
                Rol = DTO.Rol,
                Activo = true,
                DebeCambiarContrasena = true
            };
            entidad.Contrasena = hasher.HashPassword(entidad, DTO.Contrasena);

            await repositorio.Insert(entidad);
            return Ok(entidad.Id);
        }

        [HttpPut("{id:int}")] //api/usuario/1
        public async Task<ActionResult<bool>> Put(int id, UsuarioDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            var error = Validar(DTO);
            if (error != null) return BadRequest(error);

            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el usuario con id: {id}.");

            if (await repositorio.ExisteNombreUsuario(DTO.NombreUsuario, id))
                return BadRequest($"Ya existe otro usuario {DTO.NombreUsuario.Trim()}.");

            var dejaDeSerDuenoActivo = entidad.Rol == Roles.Dueno && entidad.Activo &&
                                       (DTO.Rol != Roles.Dueno || !DTO.Activo);
            if (dejaDeSerDuenoActivo)
            {
                if (id == UsuarioActualId())
                    return BadRequest("No podés desactivarte ni quitarte el rol de dueño a vos mismo.");
                if (await repositorio.DuenosActivos(id) == 0)
                    return BadRequest("Tiene que quedar al menos un dueño activo.");
            }

            entidad.NombreUsuario = DTO.NombreUsuario.Trim();
            entidad.Nombre = DTO.Nombre.Trim();
            entidad.Rol = DTO.Rol;
            entidad.Activo = DTO.Activo;

            return Ok(await repositorio.Update(entidad));
        }

        [HttpPut("{id:int}/contrasena")] //api/usuario/1/contrasena
        public async Task<ActionResult<bool>> Blanquear(int id, NuevaContrasenaDTO DTO)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el usuario con id: {id}.");

            entidad.Contrasena = hasher.HashPassword(entidad, DTO.Nueva);
            entidad.DebeCambiarContrasena = true;
            return Ok(await repositorio.Update(entidad));
        }

        private static string? Validar(UsuarioDTO DTO)
        {
            if (!Roles.Todos.Contains(DTO.Rol))
                return "Rol inválido. Valores permitidos: Dueno, Recepcionista.";
            if (string.IsNullOrWhiteSpace(DTO.NombreUsuario) || DTO.NombreUsuario.Trim().Contains(' '))
                return "El nombre de usuario es obligatorio y no puede tener espacios.";
            return null;
        }

        private int? UsuarioActualId()
        {
            var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(valor, out var id) ? id : null;
        }

        private static UsuarioDTO ADTO(Usuario u) => new()
        {
            Id = u.Id,
            NombreUsuario = u.NombreUsuario,
            Nombre = u.Nombre,
            Rol = u.Rol,
            Activo = u.Activo
        };
    }
}
