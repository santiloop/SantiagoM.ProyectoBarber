using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    // Paso 3 - Reglas de la agenda (CU02, CU04, CU12).
    // Un turno se puede dar si:
    //  1. Empieza en un salto de 5 minutos (10:00, 10:05, 10:10...).
    //  2. Es a futuro.
    //  3. La fecha no es un dia no laborable (feriado, cierre).
    //  4. El barbero esta activo.
    //  5. Inicio y fin estimado entran en una franja del horario del local.
    //  6. Inicio y fin estimado entran en una franja del horario del barbero.
    //  7. El barbero no tiene otro turno Pendiente / En atencion que se pise.
    public class ValidadorAgenda : IValidadorAgenda
    {
        private readonly IHorarioLocalRepositorio horarioLocalRepositorio;
        private readonly IHorarioBarberoRepositorio horarioBarberoRepositorio;
        private readonly IDiaNoLaborableRepositorio diaNoLaborableRepositorio;
        private readonly ITurnoRepositorio turnoRepositorio;

        public ValidadorAgenda(IHorarioLocalRepositorio horarioLocalRepositorio,
                               IHorarioBarberoRepositorio horarioBarberoRepositorio,
                               IDiaNoLaborableRepositorio diaNoLaborableRepositorio,
                               ITurnoRepositorio turnoRepositorio)
        {
            this.horarioLocalRepositorio = horarioLocalRepositorio;
            this.horarioBarberoRepositorio = horarioBarberoRepositorio;
            this.diaNoLaborableRepositorio = diaNoLaborableRepositorio;
            this.turnoRepositorio = turnoRepositorio;
        }

        public async Task<string?> ValidarTurno(DateTime inicio, Servicio servicio, Barbero barbero, int idTurnoExcluir)
        {
            var fin = inicio.AddMinutes(servicio.Duracion);
            var fecha = DateOnly.FromDateTime(inicio);
            var dia = inicio.DayOfWeek;
            var nombreDia = ReglasHorario.NombreDia(dia);
            var horaInicio = TimeOnly.FromDateTime(inicio);
            var horaFin = TimeOnly.FromDateTime(fin);

            // 1. Saltos de 5 minutos
            if (inicio.Minute % ReglasHorario.SaltoMinutos != 0 || inicio.Second != 0 || inicio.Millisecond != 0)
                return $"El turno tiene que empezar en saltos de {ReglasHorario.SaltoMinutos} minutos (ej. 10:00, 10:05, 10:10).";

            // 2. A futuro
            if (inicio <= DateTime.Now)
                return "No se puede dar un turno en una fecha u hora que ya pasó.";

            if (DateOnly.FromDateTime(fin) != fecha)
                return "El turno no puede terminar al día siguiente.";

            // 3. Dia no laborable
            if (await diaNoLaborableRepositorio.ExisteFecha(fecha, 0))
                return $"El {fecha:dd/MM/yyyy} es un día no laborable.";

            // 4. Barbero activo
            if (!barbero.Activo)
                return $"El barbero {barbero.Nombre} está inactivo.";

            // 5. Horario del local
            var franjasLocal = (await horarioLocalRepositorio.SelectPorDia(dia))
                                .Select(f => (f.HoraDesde, f.HoraHasta)).ToList();
            if (franjasLocal.Count == 0)
                return $"El local no atiende el día {nombreDia}.";
            if (!ReglasHorario.EstaDentro(horaInicio, horaFin, franjasLocal))
                return $"De {horaInicio:HH:mm} a {horaFin:HH:mm} queda fuera del horario del local del {nombreDia} (" +
                       string.Join(" y ", franjasLocal.Select(f => $"{f.HoraDesde:HH:mm} a {f.HoraHasta:HH:mm}")) + ").";

            // 6. Horario del barbero
            var franjasBarbero = (await horarioBarberoRepositorio.SelectPorBarbero(barbero.Id))
                                .Where(f => f.DiaSemana == dia)
                                .Select(f => (f.HoraDesde, f.HoraHasta)).ToList();
            if (franjasBarbero.Count == 0)
                return $"{barbero.Nombre} no trabaja el día {nombreDia}.";
            if (!ReglasHorario.EstaDentro(horaInicio, horaFin, franjasBarbero))
                return $"De {horaInicio:HH:mm} a {horaFin:HH:mm} queda fuera del horario de {barbero.Nombre} del {nombreDia} (" +
                       string.Join(" y ", franjasBarbero.Select(f => $"{f.HoraDesde:HH:mm} a {f.HoraHasta:HH:mm}")) + ").";

            // 7. Superposicion con otro turno del barbero
            if (await turnoRepositorio.HaySuperposicion(barbero.Id, inicio, fin, idTurnoExcluir))
                return $"{barbero.Nombre} ya tiene un turno que se pisa con {horaInicio:HH:mm} a {horaFin:HH:mm}.";

            return null;
        }

        public async Task<List<HorarioDisponibleDTO>> Disponibles(DateOnly fecha, Servicio servicio, int? barberoId, int idTurnoExcluir = 0)
        {
            var resultado = new List<HorarioDisponibleDTO>();
            var dia = fecha.DayOfWeek;

            if (await diaNoLaborableRepositorio.ExisteFecha(fecha, 0))
                return resultado;

            var franjasLocal = await horarioLocalRepositorio.SelectPorDia(dia);

            // Franjas de los barberos activos ese dia (opcionalmente uno solo).
            var franjasBarberos = (await horarioBarberoRepositorio.SelectPorDia(dia))
                                    .Where(h => h.Barbero.Activo)
                                    .Where(h => barberoId == null || h.BarberoId == barberoId)
                                    .ToList();
            var barberos = franjasBarberos.Select(h => h.Barbero).DistinctBy(b => b.Id).ToList();

            // Paso 8 - al reprogramar, el propio turno no ocupa lugar.
            var ocupados = (await turnoRepositorio.OcupadosDelDia(fecha))
                            .Where(t => t.Id != idTurnoExcluir)
                            .ToList();

            // Cantidad de turnos de cada barbero ese dia, para elegir el sugerido.
            var cantidadTurnos = barberos.ToDictionary(b => b.Id, b => ocupados.Count(t => t.BarberoId == b.Id));

            var ahora = DateTime.Now;

            foreach (var franja in franjasLocal)
            {
                for (var hora = franja.HoraDesde;
                     hora.AddMinutes(servicio.Duracion) <= franja.HoraHasta && hora >= franja.HoraDesde;
                     hora = hora.AddMinutes(ReglasHorario.SaltoMinutos))
                {
                    var inicio = fecha.ToDateTime(hora);
                    var fin = inicio.AddMinutes(servicio.Duracion);
                    var horaFin = TimeOnly.FromDateTime(fin);

                    if (inicio <= ahora)
                        continue;

                    var libres = barberos
                        .Where(b => ReglasHorario.EstaDentro(hora, horaFin,
                                        franjasBarberos.Where(f => f.BarberoId == b.Id).Select(f => (f.HoraDesde, f.HoraHasta))))
                        .Where(b => !ocupados.Any(t => t.BarberoId == b.Id && t.FechaHora < fin && inicio < t.FechaHoraFinEstimada))
                        .ToList();

                    if (libres.Count == 0)
                        continue;

                    var sugerido = libres
                        .OrderBy(b => cantidadTurnos[b.Id])
                        .ThenBy(b => b.Nombre)
                        .First();

                    resultado.Add(new HorarioDisponibleDTO
                    {
                        Hora = hora,
                        HoraFinEstimada = horaFin,
                        Barberos = libres.OrderBy(b => b.Nombre)
                                         .Select(b => new BarberoLibreDTO { Id = b.Id, Nombre = b.Nombre })
                                         .ToList(),
                        BarberoSugeridoId = sugerido.Id,
                        BarberoSugeridoNombre = sugerido.Nombre
                    });
                }
            }

            return resultado.OrderBy(r => r.Hora).ToList();
        }
    }
}
