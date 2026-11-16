using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    // Paso 2 - Reglas de horario que comparten los controllers de horarios (CU15 y CU08)
    // y que van a usar las reglas del turno (paso 3).
    public static class ReglasHorario
    {
        // Paso 3 - Estados de turno que ocupan horario en la agenda:
        // Pendiente (PEN) y En atencion (ENA, se agrega en el paso 4).
        // Atendido, Ausente y Cancelado liberan el horario.
        public static readonly string[] CodigosQueOcupan = { "PEN", "ENA" };

        // Paso 3 - Los turnos empiezan en saltos de 5 minutos.
        public const int SaltoMinutos = 5;

        // Paso 4 - Cuantas horas antes se avisa a la recepcionista para confirmar el turno.
        public const int HorasAviso = 2;

        // true si el rango [desde, hasta) entra completo en alguna de las franjas.
        public static bool EstaDentro(TimeOnly desde, TimeOnly hasta, IEnumerable<(TimeOnly Desde, TimeOnly Hasta)> franjas)
        {
            return franjas.Any(f => f.Desde <= desde && hasta <= f.Hasta);
        }

        // true si dos franjas del mismo dia se pisan (10-13 y 12-14 se pisan; 10-13 y 13-15 no).
        public static bool SeSuperponen(TimeOnly desde1, TimeOnly hasta1, TimeOnly desde2, TimeOnly hasta2)
        {
            return desde1 < hasta2 && desde2 < hasta1;
        }

        // Nombre del dia en espanol para mostrar en pantalla.
        public static string NombreDia(DayOfWeek dia)
        {
            return dia switch
            {
                DayOfWeek.Sunday => "Domingo",
                DayOfWeek.Monday => "Lunes",
                DayOfWeek.Tuesday => "Martes",
                DayOfWeek.Wednesday => "Miércoles",
                DayOfWeek.Thursday => "Jueves",
                DayOfWeek.Friday => "Viernes",
                _ => "Sábado"
            };
        }

        // Hora de inicio y fin estimada de un turno.
        // Paso 3: usa el fin estimado guardado en el turno.
        public static (TimeOnly Inicio, TimeOnly Fin) RangoTurno(Turno t)
        {
            return (TimeOnly.FromDateTime(t.FechaHora),
                    TimeOnly.FromDateTime(t.FechaHoraFinEstimada));
        }

        // Turno -> TurnoListadoDTO (con nombres). Necesita Cliente, Barbero, Servicio y EstadoTurno cargados.
        public static TurnoListadoDTO AListado(Turno t)
        {
            return new TurnoListadoDTO
            {
                Id = t.Id,
                Cliente = t.Cliente.Nombre,
                ClienteTelefono = t.Cliente.Telefono,
                Barbero = t.Barbero.Nombre,
                Servicio = t.Servicio.Nombre,
                Estado = t.EstadoTurno.Nombre,
                EstadoCodigo = t.EstadoTurno.Codigo,
                FechaHora = t.FechaHora,
                FechaHoraFinEstimada = t.FechaHoraFinEstimada,
                FechaHoraInicioReal = t.FechaHoraInicioReal,
                FechaHoraFinReal = t.FechaHoraFinReal,
                Confirmado = t.Confirmado,
                Observaciones = t.Observaciones,
                ServicioId = t.ServicioId,               // Paso 11
                BarberoId = t.BarberoId,                 // Paso 11
                PedidoPorCliente = t.PedidoPorCliente    // Paso 11
            };
        }
    }
}
