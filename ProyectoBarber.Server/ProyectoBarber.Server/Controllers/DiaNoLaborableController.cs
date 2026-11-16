using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProyectoBarber.Server.Controllers
{
    // ==================================================================
    // DIA NO LABORABLE CONTROLLER  -  ruta base: api/dianolaborable
    // CU16 - Gestionar dias no laborables: dias puntuales en que no se atiende
    // (feriado, cierre), aunque el horario semanal diga que se trabaja.
    // Actores: Dueno y Recepcionista.
    // Regla acordada: si ese dia hay turnos pendientes, el dia se guarda igual y
    // se devuelven esos turnos para reprogramarlos o cancelarlos.
    // ==================================================================
    // Paso 10 - Dueno y recepcionista (feriados de ultimo momento).
    // Paso 12 - Cada alta, cambio o baja genera un aviso para el dueno y la recepcionista.
    [Authorize]
    [ApiController]
    [Route("api/dianolaborable")]
    public class DiaNoLaborableController : ControllerBase
    {
        private readonly IDiaNoLaborableRepositorio repositorio;
        private readonly ITurnoRepositorio turnoRepositorio;
        private readonly IAvisoRepositorio avisoRepositorio;   // Paso 12

        public DiaNoLaborableController(IDiaNoLaborableRepositorio repositorio,
                                        ITurnoRepositorio turnoRepositorio,
                                        IAvisoRepositorio avisoRepositorio)
        {
            this.repositorio = repositorio;
            this.turnoRepositorio = turnoRepositorio;
            this.avisoRepositorio = avisoRepositorio;
        }

        // ------------------------------------------------------------------
        // CU16 - Listar los dias no laborables de hoy en adelante
        // GET api/dianolaborable
        // ------------------------------------------------------------------
        [HttpGet] //api/dianolaborable
        public async Task<ActionResult<List<DiaNoLaborableDTO>>> Get()
        {
            var lista = await repositorio.SelectDesde(DateOnly.FromDateTime(DateTime.Today));
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");

            return Ok(lista.Select(d => ADTO(d)).ToList());
        }

        // ------------------------------------------------------------------
        // CU16 - Consultar un dia no laborable (para editarlo)
        // GET api/dianolaborable/{id}
        // ------------------------------------------------------------------
        [HttpGet("{id:int}")] //api/dianolaborable/1
        public async Task<ActionResult<DiaNoLaborableDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(ADTO(entidad));
        }

        // ------------------------------------------------------------------
        // CU16 - Marcar un dia como no laborable (ej. feriado del lunes)
        // POST api/dianolaborable
        // No acepta fechas pasadas ni una fecha ya cargada.
        // Devuelve el Id y los turnos pendientes de ese dia (si hay).
        // ------------------------------------------------------------------
        [HttpPost] //api/dianolaborable
        public async Task<ActionResult<CambioHorarioDTO>> Post(DiaNoLaborableDTO DTO)
        {
            var error = await Validar(DTO, 0);
            if (error != null)
                return BadRequest(error);

            DiaNoLaborable entidad = new DiaNoLaborable();
            entidad.Fecha = DTO.Fecha;
            entidad.Motivo = DTO.Motivo;

            await repositorio.Insert(entidad);

            var resultado = await Resultado(entidad.Id, DTO.Fecha, "Día no laborable guardado.");
            await avisoRepositorio.Crear("DiaNoLaborable",
                $"El día {DTO.Fecha:dd/MM} no habrá jornada laboral. Motivo: {DTO.Motivo}." + TurnosParaAvisar(resultado),
                DTO.Fecha, UsuarioActualId());   // Paso 12
            return Ok(resultado);
        }

        // ------------------------------------------------------------------
        // CU16 - Modificar un dia no laborable (fecha o motivo)
        // PUT api/dianolaborable/{id}  -  el id de la ruta debe coincidir con el del DTO.
        // ------------------------------------------------------------------
        [HttpPut("{id:int}")] //api/dianolaborable/1
        public async Task<ActionResult<CambioHorarioDTO>> Put(int id, DiaNoLaborableDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            var error = await Validar(DTO, id);
            if (error != null)
                return BadRequest(error);

            var fechaAnterior = entidad.Fecha;
            entidad.Fecha = DTO.Fecha;
            entidad.Motivo = DTO.Motivo;
            await repositorio.Update(entidad);

            var resultado = await Resultado(id, DTO.Fecha, "Día no laborable actualizado.");
            var antes = fechaAnterior != DTO.Fecha ? $" (antes era el {fechaAnterior:dd/MM}, ese día vuelve a ser laboral)" : "";
            await avisoRepositorio.Crear("DiaNoLaborable",
                $"Cambio de día no laborable: el día {DTO.Fecha:dd/MM} no habrá jornada laboral{antes}. Motivo: {DTO.Motivo}." + TurnosParaAvisar(resultado),
                DTO.Fecha, UsuarioActualId());   // Paso 12
            return Ok(resultado);
        }

        // ------------------------------------------------------------------
        // CU16 - Quitar un dia no laborable (el local vuelve a atender ese dia)
        // DELETE api/dianolaborable/{id}
        // ------------------------------------------------------------------
        [HttpDelete("{id:int}")] //api/dianolaborable/1
        public async Task<ActionResult<bool>> Delete(int id)
        {
            var entidad = await repositorio.SelectById(id);   // Paso 12 - para el aviso
            var resultado = await repositorio.Delete(id);
            if (!resultado)
                return NotFound($"No existe el registro con id: {id}.");

            if (entidad != null)
                await avisoRepositorio.Crear("DiaNoLaborable",
                    $"El día {entidad.Fecha:dd/MM} vuelve a ser laboral (se quitó: {entidad.Motivo}).",
                    entidad.Fecha, UsuarioActualId());   // Paso 12
            return Ok(true);
        }

        // ================== Metodos de apoyo ==================

        // Paso 12 - Usuario logueado (sale del token).
        private int? UsuarioActualId()
        {
            var valor = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(valor, out var id) ? id : null;
        }

        // Paso 12 - Agrega al aviso cuantos turnos pendientes hay ese dia.
        private static string TurnosParaAvisar(CambioHorarioDTO resultado) =>
            resultado.TurnosAfectados.Count == 0 ? ""
            : $" Hay {resultado.TurnosAfectados.Count} turno(s) pendiente(s) ese día para reprogramar o cancelar.";

        private async Task<string?> Validar(DiaNoLaborableDTO DTO, int idExcluir)
        {
            if (DTO.Fecha < DateOnly.FromDateTime(DateTime.Today))
                return "La fecha ya pasó.";

            if (await repositorio.ExisteFecha(DTO.Fecha, idExcluir))
                return $"El {DTO.Fecha:dd/MM/yyyy} ya está cargado como día no laborable.";

            return null;
        }

        // Arma la respuesta con los turnos pendientes de esa fecha.
        private async Task<CambioHorarioDTO> Resultado(int id, DateOnly fecha, string mensajeBase)
        {
            var pendientes = await turnoRepositorio.PendientesFuturos();
            var afectados = pendientes
                                .Where(t => DateOnly.FromDateTime(t.FechaHora) == fecha)
                                .Select(t => ReglasHorario.AListado(t))
                                .ToList();

            return new CambioHorarioDTO
            {
                Id = id,
                TurnosAfectados = afectados,
                Mensaje = afectados.Count == 0
                    ? mensajeBase
                    : $"{mensajeBase} Hay {afectados.Count} turno(s) pendiente(s) ese día: reprogramalos o cancelalos."
            };
        }

        private static DiaNoLaborableDTO ADTO(DiaNoLaborable d)
        {
            return new DiaNoLaborableDTO
            {
                Id = d.Id,
                Fecha = d.Fecha,
                Motivo = d.Motivo
            };
        }
    }
}
