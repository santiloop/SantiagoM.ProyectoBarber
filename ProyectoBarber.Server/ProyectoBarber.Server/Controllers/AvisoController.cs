using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Server.Controllers
{
    // ==================================================================
    // AVISO CONTROLLER  -  ruta base: api/aviso   (paso 12)
    // CU13 - Avisos (campanita) para el Dueno y la Recepcionista.
    //   GET api/aviso?dias=7       -> avisos de los ultimos 7, 30 o 60 dias (maximo 60)
    //   GET api/aviso/cantidad     -> cuantos no leyo el usuario (para la campanita)
    //   PUT api/aviso/leidos       -> marca todos como leidos para el usuario
    // Leido es POR PERSONA. Los avisos de mas de 2 meses se borran solos.
    // ==================================================================
    [Authorize]
    [ApiController]
    [Route("api/aviso")]
    public class AvisoController : ControllerBase
    {
        public const int DiasMaximo = 60;

        private readonly IAvisoRepositorio repositorio;
        private readonly IUsuarioRepositorio usuarioRepositorio;

        public AvisoController(IAvisoRepositorio repositorio, IUsuarioRepositorio usuarioRepositorio)
        {
            this.repositorio = repositorio;
            this.usuarioRepositorio = usuarioRepositorio;
        }

        [HttpGet] //api/aviso?dias=7
        public async Task<ActionResult<List<AvisoDTO>>> Get(int dias = 7)
        {
            var usuario = await UsuarioActual();
            if (usuario is null) return Unauthorized("Sesión inválida.");

            dias = Math.Clamp(dias, 1, DiasMaximo);
            await repositorio.BorrarAnteriores(DateTime.Now.AddDays(-DiasMaximo));

            var lista = await repositorio.Desde(DateTime.Now.AddDays(-dias));
            return Ok(lista.Select(a => new AvisoDTO
            {
                Id = a.Id,
                FechaHora = a.FechaHora,
                Tipo = a.Tipo,
                Mensaje = a.Mensaje,
                FechaReferencia = a.FechaReferencia,
                Autor = a.Usuario?.Nombre ?? "Cliente (web)",
                Nuevo = a.Id > usuario.UltimoAvisoLeidoId && a.UsuarioId != usuario.Id
            }).ToList());
        }

        [HttpGet("cantidad")] //api/aviso/cantidad
        public async Task<ActionResult<int>> Cantidad()
        {
            var usuario = await UsuarioActual();
            if (usuario is null) return Unauthorized("Sesión inválida.");
            return Ok(await repositorio.NoLeidos(usuario.Id, usuario.UltimoAvisoLeidoId, DateTime.Now.AddDays(-DiasMaximo)));
        }

        [HttpPut("leidos")] //api/aviso/leidos
        public async Task<ActionResult<bool>> MarcarLeidos()
        {
            var usuario = await UsuarioActual();
            if (usuario is null) return Unauthorized("Sesión inválida.");

            usuario.UltimoAvisoLeidoId = await repositorio.UltimoId();
            await usuarioRepositorio.Update(usuario);
            return Ok(true);
        }

        private async Task<ProyectoBarber.BD.Datos.Entity.Usuario?> UsuarioActual()
        {
            if (!int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id)) return null;
            return await usuarioRepositorio.SelectById(id);
        }
    }
}
