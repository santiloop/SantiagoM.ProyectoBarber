using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Repositorio;
using ProyectoBarber.Shared.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ProyectoBarber.Server.Controllers
{
    // ==================================================================
    // HORARIO LOCAL CONTROLLER  -  ruta base: api/horariolocal
    // CU15 - Configurar horario del local. Actor: Dueno.
    // Cada registro es una franja de un dia (ej. Lunes 10:00 a 13:00).
    // Un dia sin franjas = el local no trabaja ese dia.
    // Reglas acordadas:
    //  - Dos franjas del mismo dia no se pueden pisar.
    //  - Si un cambio deja afuera el horario de algun barbero, se RECHAZA
    //    (primero hay que ajustar al barbero).
    //  - Si un cambio deja afuera turnos pendientes, se GUARDA igual y se
    //    devuelve la lista de esos turnos para reprogramarlos o cancelarlos.
    // ==================================================================
    // Paso 10 - Ver: dueno y recepcionista. Alta, modificacion y baja: solo el dueno.
    [Authorize]
    [ApiController]
    [Route("api/horariolocal")]
    public class HorarioLocalController : ControllerBase
    {
        private readonly IHorarioLocalRepositorio repositorio;
        private readonly IHorarioBarberoRepositorio horarioBarberoRepositorio;
        private readonly ITurnoRepositorio turnoRepositorio;

        public HorarioLocalController(IHorarioLocalRepositorio repositorio,
                                      IHorarioBarberoRepositorio horarioBarberoRepositorio,
                                      ITurnoRepositorio turnoRepositorio)
        {
            this.repositorio = repositorio;
            this.horarioBarberoRepositorio = horarioBarberoRepositorio;
            this.turnoRepositorio = turnoRepositorio;
        }

        // ------------------------------------------------------------------
        // CU15 - Ver el horario semanal del local
        // GET api/horariolocal  -  todas las franjas, ordenadas por dia y hora.
        // ------------------------------------------------------------------
        [HttpGet] //api/horariolocal
        public async Task<ActionResult<List<HorarioLocalDTO>>> Get()
        {
            var lista = await repositorio.Select();
            if (lista == null)
                return NotFound("No se encontro elementos de la lista, VERIFICAR.");

            var listaDTO = lista
                            .OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraDesde)
                            .Select(h => ADTO(h))
                            .ToList();

            return Ok(listaDTO);
        }

        // ------------------------------------------------------------------
        // CU15 - Consultar una franja (para editarla)
        // GET api/horariolocal/{id}
        // ------------------------------------------------------------------
        [HttpGet("{id:int}")] //api/horariolocal/1
        public async Task<ActionResult<HorarioLocalDTO>> GetById(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            return Ok(ADTO(entidad));
        }

        // ------------------------------------------------------------------
        // CU15 - Agregar una franja (ej. abrir los sabados de 10 a 14)
        // POST api/horariolocal  -  devuelve el Id creado.
        // Extensiones 3a y 3b: fin posterior al inicio y sin pisar otra franja del dia.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpPost] //api/horariolocal
        public async Task<ActionResult<int>> Post(HorarioLocalDTO DTO)
        {
            var error = await ValidarFranja(DTO, 0);
            if (error != null)
                return BadRequest(error);

            HorarioLocal entidad = new HorarioLocal();
            entidad.DiaSemana = (DayOfWeek)DTO.DiaSemana;
            entidad.HoraDesde = DTO.HoraDesde;
            entidad.HoraHasta = DTO.HoraHasta;

            await repositorio.Insert(entidad);

            return Ok(entidad.Id);
        }

        // ------------------------------------------------------------------
        // CU15 - Modificar una franja (ej. cambiar 15-21 por 16-20)
        // PUT api/horariolocal/{id}
        // Rechaza si deja afuera el horario de un barbero.
        // Si deja afuera turnos pendientes, guarda y los devuelve en TurnosAfectados.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpPut("{id:int}")] //api/horariolocal/1
        public async Task<ActionResult<CambioHorarioDTO>> Put(int id, HorarioLocalDTO DTO)
        {
            if (id != DTO.Id)
                return BadRequest("Datos incorrectos, no se actualizó.");

            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            var error = await ValidarFranja(DTO, id);
            if (error != null)
                return BadRequest(error);

            // Dias que cambian: el dia viejo de la franja y el dia nuevo (pueden ser el mismo).
            var diaViejo = entidad.DiaSemana;
            var diaNuevo = (DayOfWeek)DTO.DiaSemana;
            var dias = new List<DayOfWeek> { diaViejo };
            if (diaNuevo != diaViejo) dias.Add(diaNuevo);

            var franjasResultantes = new Dictionary<DayOfWeek, List<(TimeOnly Desde, TimeOnly Hasta)>>();
            foreach (var dia in dias)
                franjasResultantes[dia] = await FranjasDelDia(dia, id, dia == diaNuevo ? DTO : null);

            // Regla: no puede quedar ningun barbero trabajando fuera del horario del local.
            var conflictos = await BarberosAfuera(franjasResultantes);
            if (conflictos.Count > 0)
                return BadRequest("No se puede: estos barberos tienen horario fuera del nuevo horario del local: "
                                  + string.Join(", ", conflictos) + ". Ajustá primero su horario.");

            entidad.DiaSemana = diaNuevo;
            entidad.HoraDesde = DTO.HoraDesde;
            entidad.HoraHasta = DTO.HoraHasta;
            await repositorio.Update(entidad);

            var afectados = await TurnosAfuera(franjasResultantes);
            return Ok(new CambioHorarioDTO
            {
                Id = id,
                TurnosAfectados = afectados,
                Mensaje = afectados.Count == 0
                    ? "Horario actualizado."
                    : $"Horario actualizado. Hay {afectados.Count} turno(s) pendiente(s) fuera del nuevo horario: reprogramalos o cancelalos."
            });
        }

        // ------------------------------------------------------------------
        // CU15 - Borrar una franja (ej. dejar de atender los martes)
        // DELETE api/horariolocal/{id}
        // Mismas reglas que el PUT.
        // ------------------------------------------------------------------
        [Authorize(Roles = Roles.Dueno)]
        [HttpDelete("{id:int}")] //api/horariolocal/1
        public async Task<ActionResult<CambioHorarioDTO>> Delete(int id)
        {
            var entidad = await repositorio.SelectById(id);
            if (entidad is null)
                return NotFound($"No existe el registro con id: {id}.");

            var dia = entidad.DiaSemana;
            var franjasResultantes = new Dictionary<DayOfWeek, List<(TimeOnly Desde, TimeOnly Hasta)>>
            {
                [dia] = await FranjasDelDia(dia, id, null)
            };

            var conflictos = await BarberosAfuera(franjasResultantes);
            if (conflictos.Count > 0)
                return BadRequest("No se puede: estos barberos tienen horario en esa franja: "
                                  + string.Join(", ", conflictos) + ". Ajustá primero su horario.");

            await repositorio.Delete(id);

            var afectados = await TurnosAfuera(franjasResultantes);
            return Ok(new CambioHorarioDTO
            {
                Id = id,
                TurnosAfectados = afectados,
                Mensaje = afectados.Count == 0
                    ? "Franja eliminada."
                    : $"Franja eliminada. Hay {afectados.Count} turno(s) pendiente(s) fuera del horario: reprogramalos o cancelalos."
            });
        }

        // ================== Metodos de apoyo ==================

        // Valida la franja: fin despues del inicio y sin pisar otra franja del mismo dia.
        private async Task<string?> ValidarFranja(HorarioLocalDTO DTO, int idExcluir)
        {
            if (DTO.HoraDesde >= DTO.HoraHasta)
                return "La hora de fin tiene que ser posterior a la hora de inicio.";

            var dia = (DayOfWeek)DTO.DiaSemana;
            var franjasDia = await repositorio.SelectPorDia(dia);
            var pisada = franjasDia.FirstOrDefault(f => f.Id != idExcluir &&
                            ReglasHorario.SeSuperponen(f.HoraDesde, f.HoraHasta, DTO.HoraDesde, DTO.HoraHasta));
            if (pisada != null)
                return $"La franja se pisa con otra del {ReglasHorario.NombreDia(dia)} " +
                       $"({pisada.HoraDesde:HH:mm} a {pisada.HoraHasta:HH:mm}).";

            return null;
        }

        // Franjas que quedarian en un dia despues del cambio: las actuales menos la editada,
        // mas la franja nueva si corresponde a ese dia.
        private async Task<List<(TimeOnly Desde, TimeOnly Hasta)>> FranjasDelDia(DayOfWeek dia, int idExcluir, HorarioLocalDTO? nueva)
        {
            var franjas = (await repositorio.SelectPorDia(dia))
                            .Where(f => f.Id != idExcluir)
                            .Select(f => (f.HoraDesde, f.HoraHasta))
                            .ToList();
            if (nueva != null)
                franjas.Add((nueva.HoraDesde, nueva.HoraHasta));
            return franjas;
        }

        // Barberos con alguna franja que ya no entra en el horario del local.
        private async Task<List<string>> BarberosAfuera(Dictionary<DayOfWeek, List<(TimeOnly Desde, TimeOnly Hasta)>> franjasPorDia)
        {
            var conflictos = new List<string>();
            foreach (var (dia, franjas) in franjasPorDia)
            {
                var horariosBarberos = await horarioBarberoRepositorio.SelectPorDia(dia);
                foreach (var h in horariosBarberos)
                {
                    if (!ReglasHorario.EstaDentro(h.HoraDesde, h.HoraHasta, franjas))
                        conflictos.Add($"{h.Barbero.Nombre} ({ReglasHorario.NombreDia(dia)} {h.HoraDesde:HH:mm} a {h.HoraHasta:HH:mm})");
                }
            }
            return conflictos;
        }

        // Turnos pendientes futuros de esos dias que quedan fuera de las franjas.
        private async Task<List<TurnoListadoDTO>> TurnosAfuera(Dictionary<DayOfWeek, List<(TimeOnly Desde, TimeOnly Hasta)>> franjasPorDia)
        {
            var pendientes = await turnoRepositorio.PendientesFuturos();
            return pendientes
                    .Where(t => franjasPorDia.ContainsKey(t.FechaHora.DayOfWeek))
                    .Where(t =>
                    {
                        var (inicio, fin) = ReglasHorario.RangoTurno(t);
                        return !ReglasHorario.EstaDentro(inicio, fin, franjasPorDia[t.FechaHora.DayOfWeek]);
                    })
                    .Select(t => ReglasHorario.AListado(t))
                    .ToList();
        }

        private static HorarioLocalDTO ADTO(HorarioLocal h)
        {
            return new HorarioLocalDTO
            {
                Id = h.Id,
                DiaSemana = (int)h.DiaSemana,
                DiaNombre = ReglasHorario.NombreDia(h.DiaSemana),
                HoraDesde = h.HoraDesde,
                HoraHasta = h.HoraHasta
            };
        }
    }
}
