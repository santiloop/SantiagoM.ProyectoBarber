using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ProyectoBarber.Server.Controllers
{
    // ==================================================================
    // HORARIO BARBERO CONTROLLER  -  ruta base: api/horariobarbero
    // CU08 - Gestionar barberos: dias y horarios en que trabaja cada uno.
    // Actor: Dueno. Cada registro es una franja (ej. Juan, Lunes 10:00 a 13:00).
    // Reglas acordadas:
    //  - La franja tiene que quedar DENTRO de una franja del local ese dia.
    //  - Dos franjas del mismo barbero el mismo dia no se pueden pisar.
    //  - Si un cambio deja afuera turnos pendientes del barbero, se guarda igual
    //    y se devuelven esos turnos para reprogramarlos o cancelarlos.
    // ==================================================================
    // Paso 10 - Ver: dueno y recepcionista. Alta, modificacion y baja: solo el dueno.
    [Authorize]
    [ApiController]
    [Route("api/horariobarbero")]
    public class HorarioBarberoController : ControllerBase
    {
        private readonly IHorarioBarberoRepositorio repositorio;
        private readonly IHorarioLocalRepositorio horarioLocalRepositorio;
        private readonly IBarberoRepositorio barberoRepositorio;
        private readonly ITurnoRepositorio turnoRepositorio;
        private readonly IAvisoRepositorio avisoRepositorio;   // Paso 12 - avisos de cambios de horario

        public HorarioBarberoController(IHorarioBarberoRepositorio repositorio,
                                        IHorarioLocalRepositorio horarioLocalRepositorio,
                                        IBarberoRepositorio barberoRepositorio,
                                        ITurnoRepositorio turnoRepositorio,
                                        IAvisoRepositorio avisoRepositorio)
        {
            this.repositorio = repositorio;
            this.horarioLocalRepositorio = horarioLocalRepositorio;
            this.barberoRepositorio = barberoRepositorio;
            this.turnoRepositorio = turnoRepositorio;
            this.avisoRepositorio = avisoRepositorio;
        }

        // ------------------------------------------------------------------
        // CU08 - Ver los dias y horarios de un barbero
        // GET api/horariobarbero/barbero/{barberoId}
        // ------------------------------------------------------------------
        [HttpGet("barbero/{barberoId:int}")] //api/horariobarbero/barbero/1
        public async Task<ActionResult<List<HorarioBarberoDTO>>> GetPorBarbero(int barberoId)
        {
            var barbero = await barberoRepositorio.SelectById(barberoId);
            if (barbero is null)
                return NotFound($"No existe el barbero con id: {barberoId}.");

            var lista = await repositorio.SelectPorBarbero(barberoId);
            return Ok(lista.Select(h => ADTO(h)).ToList());
        }

        // ------------------------------------------------------------------
        // CU08 - Consultar una franja de un barbero (para editarla)
        // GET api/horariobarbero/{id}
        // ------------------------------------------------------------------
        [HttpGet("{id:int}")] //api/horariobarbero/1
        public async Task<ActionResult<HorarioBarberoDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            var barbero = await barberoRepositorio.SelectById(entidad.BarberoId);
            entidad.Barbero = barbero!;
            return Ok(ADTO(entidad));
        }

        // ------------------------------------------------------------------
        // CU08 - Agregar una franja de trabajo a un barbero
        // POST api/horariobarbero  -  devuelve el Id creado.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpPost] //api/horariobarbero
        public async Task<ActionResult<int>> Post(HorarioBarberoDTO DTO)
        {
            var error = await ValidarFranja(DTO, 0);
            if (error != null)
                return BadRequest(error);

            HorarioBarbero entidad = new HorarioBarbero();
            entidad.BarberoId = DTO.BarberoId;
            entidad.DiaSemana = (DayOfWeek)DTO.DiaSemana;
            entidad.HoraDesde = DTO.HoraDesde;
            entidad.HoraHasta = DTO.HoraHasta;

            await repositorio.Insert(entidad);

            await Avisar(entidad.BarberoId, $"se agregó el {Franja(entidad.DiaSemana, entidad.HoraDesde, entidad.HoraHasta)}.");   // Paso 12
            return Ok(entidad.Id);
        }

        // ------------------------------------------------------------------
        // CU08 - Modificar una franja de un barbero
        // PUT api/horariobarbero/{id}  -  el id de la ruta debe coincidir con el del DTO.
        // Si deja afuera turnos pendientes del barbero, guarda y los devuelve.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpPut("{id:int}")] //api/horariobarbero/1
        public async Task<ActionResult<CambioHorarioDTO>> Put(int id, HorarioBarberoDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            if (entidad.BarberoId != DTO.BarberoId)
                return BadRequest("No se puede pasar una franja de un barbero a otro: borrala y cargala en el otro barbero.");

            var error = await ValidarFranja(DTO, id);
            if (error != null)
                return BadRequest(error);

            var diaViejo = entidad.DiaSemana;
            var franjaVieja = Franja(entidad.DiaSemana, entidad.HoraDesde, entidad.HoraHasta);   // Paso 12
            entidad.DiaSemana = (DayOfWeek)DTO.DiaSemana;
            entidad.HoraDesde = DTO.HoraDesde;
            entidad.HoraHasta = DTO.HoraHasta;
            await repositorio.Update(entidad);

            var afectados = await TurnosAfuera(entidad.BarberoId, new List<DayOfWeek> { diaViejo, entidad.DiaSemana });
            await Avisar(entidad.BarberoId, $"el {franjaVieja} pasó a {Franja(entidad.DiaSemana, entidad.HoraDesde, entidad.HoraHasta)}." +
                                            TurnosParaAvisar(afectados.Count));   // Paso 12
            return Ok(new CambioHorarioDTO
            {
                Id = id,
                TurnosAfectados = afectados,
                Mensaje = afectados.Count == 0
                    ? "Horario del barbero actualizado."
                    : $"Horario del barbero actualizado. Hay {afectados.Count} turno(s) pendiente(s) fuera de su horario: reprogramalos o cancelalos."
            });
        }

        // ------------------------------------------------------------------
        // CU08 - Borrar una franja de un barbero (ej. deja de trabajar los lunes)
        // DELETE api/horariobarbero/{id}
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpDelete("{id:int}")] //api/horariobarbero/1
        public async Task<ActionResult<CambioHorarioDTO>> Delete(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            var barberoId = entidad.BarberoId;
            var dia = entidad.DiaSemana;
            var franjaQuitada = Franja(entidad.DiaSemana, entidad.HoraDesde, entidad.HoraHasta);   // Paso 12
            await repositorio.Delete(id);

            var afectados = await TurnosAfuera(barberoId, new List<DayOfWeek> { dia });
            await Avisar(barberoId, $"se quitó el {franjaQuitada}." + TurnosParaAvisar(afectados.Count));   // Paso 12
            return Ok(new CambioHorarioDTO
            {
                Id = id,
                TurnosAfectados = afectados,
                Mensaje = afectados.Count == 0
                    ? "Franja eliminada."
                    : $"Franja eliminada. Hay {afectados.Count} turno(s) pendiente(s) fuera del horario del barbero: reprogramalos o cancelalos."
            });
        }

        // ================== Metodos de apoyo ==================

        // Paso 12 - Aviso para el dueno y la recepcionista: "Horario de Juan: se agrego el viernes de 15:00 a 20:00."
        private async Task Avisar(int barberoId, string cambio)
        {
            var barbero = await barberoRepositorio.SelectById(barberoId);
            int? usuarioId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : null;
            await avisoRepositorio.Crear("HorarioBarbero", $"Horario de {barbero?.Nombre}: {cambio}", null, usuarioId);
        }

        private static string Franja(DayOfWeek dia, TimeOnly desde, TimeOnly hasta) =>
            $"{ReglasHorario.NombreDia(dia).ToLower()} de {desde:HH:mm} a {hasta:HH:mm}";

        private static string TurnosParaAvisar(int cantidad) =>
            cantidad == 0 ? "" : $" Hay {cantidad} turno(s) pendiente(s) fuera de su horario.";

        // Valida: barbero existe, fin despues del inicio, dentro del horario del local
        // y sin pisar otra franja del mismo barbero ese dia.
        private async Task<string?> ValidarFranja(HorarioBarberoDTO DTO, int idExcluir)
        {
            var barbero = await barberoRepositorio.SelectById(DTO.BarberoId);
            if (barbero is null)
                return $"No existe el barbero con id: {DTO.BarberoId}.";

            if (DTO.HoraDesde >= DTO.HoraHasta)
                return "La hora de fin tiene que ser posterior a la hora de inicio.";

            var dia = (DayOfWeek)DTO.DiaSemana;
            var nombreDia = ReglasHorario.NombreDia(dia);

            var franjasLocal = (await horarioLocalRepositorio.SelectPorDia(dia))
                                .Select(f => (f.HoraDesde, f.HoraHasta))
                                .ToList();
            if (franjasLocal.Count == 0)
                return $"El local no atiende el día {nombreDia}.";

            if (!ReglasHorario.EstaDentro(DTO.HoraDesde, DTO.HoraHasta, franjasLocal))
                return $"El horario queda fuera del horario del local del {nombreDia} (" +
                       string.Join(" y ", franjasLocal.Select(f => $"{f.HoraDesde:HH:mm} a {f.HoraHasta:HH:mm}")) + ").";

            var franjasBarbero = await repositorio.SelectPorBarbero(DTO.BarberoId);
            var pisada = franjasBarbero.FirstOrDefault(f => f.Id != idExcluir && f.DiaSemana == dia &&
                            ReglasHorario.SeSuperponen(f.HoraDesde, f.HoraHasta, DTO.HoraDesde, DTO.HoraHasta));
            if (pisada != null)
                return $"Se pisa con otra franja del barbero el {nombreDia} ({pisada.HoraDesde:HH:mm} a {pisada.HoraHasta:HH:mm}).";

            return null;
        }

        // Turnos pendientes futuros del barbero, en esos dias, que quedan fuera de su horario.
        private async Task<List<TurnoListadoDTO>> TurnosAfuera(int barberoId, List<DayOfWeek> dias)
        {
            var franjas = await repositorio.SelectPorBarbero(barberoId);
            var pendientes = await turnoRepositorio.PendientesFuturos();

            return pendientes
                    .Where(t => t.BarberoId == barberoId && dias.Contains(t.FechaHora.DayOfWeek))
                    .Where(t =>
                    {
                        var (inicio, fin) = ReglasHorario.RangoTurno(t);
                        var franjasDia = franjas
                                            .Where(f => f.DiaSemana == t.FechaHora.DayOfWeek)
                                            .Select(f => (f.HoraDesde, f.HoraHasta));
                        return !ReglasHorario.EstaDentro(inicio, fin, franjasDia);
                    })
                    .Select(t => ReglasHorario.AListado(t))
                    .ToList();
        }

        private static HorarioBarberoDTO ADTO(HorarioBarbero h)
        {
            return new HorarioBarberoDTO
            {
                Id = h.Id,
                BarberoId = h.BarberoId,
                BarberoNombre = h.Barbero?.Nombre ?? string.Empty,
                DiaSemana = (int)h.DiaSemana,
                DiaNombre = ReglasHorario.NombreDia(h.DiaSemana),
                HoraDesde = h.HoraDesde,
                HoraHasta = h.HoraHasta
            };
        }
    }
}
