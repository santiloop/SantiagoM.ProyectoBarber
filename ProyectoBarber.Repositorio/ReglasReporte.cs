using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    // Paso 9 - CU10 Reporte. Arma el ReporteDTO con las cuentas de un periodo
    // a partir de los turnos (con Cliente, Barbero, Servicio y EstadoTurno cargados).
    //  - Resumen: cantidad por estado, % de ausentismo y recaudacion estimada.
    //  - Semanas: de lunes a domingo, recortadas al periodo.
    //  - Por barbero, por servicio, clientes que mas faltan y detalle de ventas.
    // La recaudacion suma Turno.Precio (precio guardado al dar el turno) de los Atendidos.
    public static class ReglasReporte
    {
        public static ReporteDTO Armar(List<Turno> turnos, DateOnly desde, DateOnly hasta)
        {
            var atendidos = turnos.Where(t => t.EstadoTurno.Codigo == ReglasEstado.Atendido).ToList();
            var ausentes = turnos.Where(t => t.EstadoTurno.Codigo == ReglasEstado.Ausente).ToList();

            var reporte = new ReporteDTO
            {
                Desde = desde,
                Hasta = hasta,
                Resumen = Resumen(turnos)
            };

            // Semanas de lunes a domingo, recortadas al periodo.
            var inicio = desde;
            while (inicio <= hasta)
            {
                var diasHastaDomingo = (7 - (int)inicio.DayOfWeek) % 7;
                var fin = inicio.AddDays(diasHastaDomingo);
                if (fin > hasta) fin = hasta;
                var desdeSemana = inicio;
                var turnosSemana = turnos.Where(t => DateOnly.FromDateTime(t.FechaHora) >= desdeSemana
                                                  && DateOnly.FromDateTime(t.FechaHora) <= fin).ToList();
                reporte.Semanas.Add(new ReporteSemanaDTO { Desde = inicio, Hasta = fin, Resumen = Resumen(turnosSemana) });
                inicio = fin.AddDays(1);
            }

            reporte.Barberos = turnos
                .GroupBy(t => t.Barbero.Nombre)
                .Select(g => new ReporteBarberoDTO
                {
                    Barbero = g.Key,
                    Atendidos = g.Count(t => t.EstadoTurno.Codigo == ReglasEstado.Atendido),
                    Ausentes = g.Count(t => t.EstadoTurno.Codigo == ReglasEstado.Ausente),
                    Cancelados = g.Count(t => t.EstadoTurno.Codigo == ReglasEstado.Cancelado),
                    MinutosTrabajados = (int)g.Where(t => t.EstadoTurno.Codigo == ReglasEstado.Atendido
                                                       && t.FechaHoraInicioReal != null && t.FechaHoraFinReal != null)
                                              .Sum(t => (t.FechaHoraFinReal!.Value - t.FechaHoraInicioReal!.Value).TotalMinutes),
                    Recaudacion = g.Where(t => t.EstadoTurno.Codigo == ReglasEstado.Atendido).Sum(t => t.Precio)
                })
                .OrderByDescending(b => b.Recaudacion)
                .ThenBy(b => b.Barbero)
                .ToList();

            reporte.Servicios = atendidos
                .GroupBy(t => t.Servicio.Nombre)
                .Select(g => new ReporteServicioDTO
                {
                    Servicio = g.Key,
                    Cantidad = g.Count(),
                    Recaudacion = g.Sum(t => t.Precio)
                })
                .OrderByDescending(s => s.Cantidad)
                .ToList();

            reporte.ClientesQueFaltan = ausentes
                .GroupBy(t => t.ClienteId)
                .Select(g => new ReporteClienteDTO
                {
                    Cliente = g.First().Cliente.Nombre,
                    Telefono = g.First().Cliente.Telefono,
                    Ausencias = g.Count()
                })
                .OrderByDescending(c => c.Ausencias)
                .ThenBy(c => c.Cliente)
                .Take(10)
                .ToList();

            reporte.Ventas = atendidos
                .OrderBy(t => t.FechaHora)
                .Select(t => new ReporteVentaDTO
                {
                    TurnoId = t.Id,
                    FechaHora = t.FechaHora,
                    Cliente = t.Cliente.Nombre,
                    Barbero = t.Barbero.Nombre,
                    Servicio = t.Servicio.Nombre,
                    Precio = t.Precio,
                    FechaHoraInicioReal = t.FechaHoraInicioReal,
                    FechaHoraFinReal = t.FechaHoraFinReal
                })
                .ToList();

            return reporte;
        }

        private static ReporteResumenDTO Resumen(List<Turno> turnos)
        {
            int Cuenta(string codigo) => turnos.Count(t => t.EstadoTurno.Codigo == codigo);

            var atendidos = Cuenta(ReglasEstado.Atendido);
            var ausentes = Cuenta(ReglasEstado.Ausente);
            return new ReporteResumenDTO
            {
                Total = turnos.Count,
                Pendientes = Cuenta(ReglasEstado.Pendiente),
                EnAtencion = Cuenta(ReglasEstado.EnAtencion),
                Atendidos = atendidos,
                Ausentes = ausentes,
                Cancelados = Cuenta(ReglasEstado.Cancelado),
                PorcentajeAusentismo = atendidos + ausentes == 0 ? 0 : Math.Round(100m * ausentes / (atendidos + ausentes), 1),
                Recaudacion = turnos.Where(t => t.EstadoTurno.Codigo == ReglasEstado.Atendido).Sum(t => t.Precio)
            };
        }
    }
}
