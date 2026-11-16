using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    // Repositorio de Turno. Hereda el CRUD generico de Repositorio<Turno>
    // y agrega consultas propias de los casos de uso de turnos.
    public class TurnoRepositorio : Repositorio<Turno>, ITurnoRepositorio
    {
        private readonly AppDbContext context;

        public TurnoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        // CU03 - Consultar agenda: une Turno con Cliente, Barbero, Servicio y
        // EstadoTurno para devolver nombres en lugar de Id.
        // Paso 4: filtros opcionales (fecha, barbero, codigo de estado) y orden por hora.
        public async Task<List<TurnoListadoDTO>> ListaTurnos(DateOnly? fecha, int? barberoId, string? estado)
        {
            var consulta = context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Barbero)
                .Include(t => t.Servicio)
                .Include(t => t.EstadoTurno)
                .AsQueryable();

            if (fecha != null)
            {
                var desde = fecha.Value.ToDateTime(TimeOnly.MinValue);
                var hasta = desde.AddDays(1);
                consulta = consulta.Where(t => t.FechaHora >= desde && t.FechaHora < hasta);
            }
            if (barberoId != null)
                consulta = consulta.Where(t => t.BarberoId == barberoId);
            if (!string.IsNullOrWhiteSpace(estado))
                consulta = consulta.Where(t => t.EstadoTurno.Codigo == estado);

            var lista = await consulta.OrderBy(t => t.FechaHora).ToListAsync();
            return lista.Select(t => ReglasHorario.AListado(t)).ToList();
        }

        // Paso 4 - Aviso 2 horas antes (turnos a confirmar).
        public async Task<List<TurnoListadoDTO>> AConfirmar(DateTime ahora, int horas)
        {
            var limite = ahora.AddHours(horas);
            var lista = await context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Barbero)
                .Include(t => t.Servicio)
                .Include(t => t.EstadoTurno)
                .Where(t => t.EstadoTurno.Codigo == ReglasEstado.Pendiente
                         && !t.Confirmado
                         && t.FechaHora >= ahora && t.FechaHora <= limite)
                .OrderBy(t => t.FechaHora)
                .ToListAsync();
            return lista.Select(t => ReglasHorario.AListado(t)).ToList();
        }

        // Paso 2 - Turnos pendientes de ahora en adelante (para listar los afectados
        // por un cambio de horario o un dia no laborable).
        public async Task<List<Turno>> PendientesFuturos()
        {
            return await context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Barbero)
                .Include(t => t.Servicio)
                .Include(t => t.EstadoTurno)
                .Where(t => t.EstadoTurno.Codigo == "PEN" && t.FechaHora >= DateTime.Now)
                .OrderBy(t => t.FechaHora)
                .ToListAsync();
        }

        // Paso 3 - Turnos Pendientes o En atencion de una fecha.
        public async Task<List<Turno>> OcupadosDelDia(DateOnly fecha)
        {
            var desde = fecha.ToDateTime(TimeOnly.MinValue);
            var hasta = desde.AddDays(1);
            return await context.Turnos
                .Include(t => t.Barbero)
                .Include(t => t.Servicio)
                .Include(t => t.EstadoTurno)
                .Where(t => ReglasHorario.CodigosQueOcupan.Contains(t.EstadoTurno.Codigo)
                         && t.FechaHora >= desde && t.FechaHora < hasta)
                .OrderBy(t => t.FechaHora)
                .ToListAsync();
        }

        // Paso 3 - Dos turnos se pisan si uno empieza antes de que termine el otro.
        public async Task<bool> HaySuperposicion(int barberoId, DateTime inicio, DateTime fin, int idExcluir)
        {
            return await context.Turnos.AnyAsync(t =>
                t.BarberoId == barberoId
                && t.Id != idExcluir
                && ReglasHorario.CodigosQueOcupan.Contains(t.EstadoTurno.Codigo)
                && t.FechaHora < fin
                && inicio < t.FechaHoraFinEstimada);
        }

        // Paso 9 - CU10 Reporte: turnos de un periodo, desde y hasta inclusive.
        public async Task<List<Turno>> TurnosDelPeriodo(DateOnly desde, DateOnly hasta)
        {
            var inicio = desde.ToDateTime(TimeOnly.MinValue);
            var fin = hasta.AddDays(1).ToDateTime(TimeOnly.MinValue);
            return await context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Barbero)
                .Include(t => t.Servicio)
                .Include(t => t.EstadoTurno)
                .Where(t => t.FechaHora >= inicio && t.FechaHora < fin)
                .OrderBy(t => t.FechaHora)
                .ToListAsync();
        }

        // Paso 11 - Portal del cliente: todos los turnos de un cliente (mas nuevos primero).
        public async Task<List<TurnoListadoDTO>> TurnosDelCliente(int clienteId)
        {
            var lista = await context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Barbero)
                .Include(t => t.Servicio)
                .Include(t => t.EstadoTurno)
                .Where(t => t.ClienteId == clienteId)
                .OrderByDescending(t => t.FechaHora)
                .ToListAsync();
            return lista.Select(t => ReglasHorario.AListado(t)).ToList();
        }

        // Paso 11 - Limite de 2 turnos pedidos por el cliente por mes (por la fecha del turno).
        public async Task<int> PedidosDelClienteEnMes(int clienteId, int anio, int mes)
        {
            var desde = new DateTime(anio, mes, 1);
            var hasta = desde.AddMonths(1);
            return await context.Turnos.CountAsync(t => t.ClienteId == clienteId
                                                     && t.PedidoPorCliente
                                                     && t.FechaHora >= desde && t.FechaHora < hasta);
        }

        // Paso 13 - Turno "completo": Include de las 4 relaciones y un DTO por cada una.
        public async Task<TurnoCompletoDTO?> SelectByIdCompleto(int id)
        {
            var entidad = await context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Barbero)
                .Include(t => t.Servicio)
                .Include(t => t.EstadoTurno)
                .FirstOrDefaultAsync(t => t.Id == id);
            if (entidad is null) return null;

            var DTO = new TurnoCompletoDTO();
            DTO.Id = entidad.Id;
            DTO.FechaHora = entidad.FechaHora;
            DTO.FechaHoraFinEstimada = entidad.FechaHoraFinEstimada;
            DTO.FechaAlta = entidad.FechaAlta;
            DTO.Observaciones = entidad.Observaciones;
            DTO.Precio = entidad.Precio;
            DTO.Confirmado = entidad.Confirmado;
            DTO.PedidoPorCliente = entidad.PedidoPorCliente;

            DTO.ClienteId = entidad.ClienteId;
            DTO.Cliente = new ClienteDTO
            {
                Id = entidad.Cliente.Id,
                Nombre = entidad.Cliente.Nombre,
                Telefono = entidad.Cliente.Telefono
            };

            DTO.BarberoId = entidad.BarberoId;
            DTO.Barbero = new BarberoDTO
            {
                Id = entidad.Barbero.Id,
                DNI = entidad.Barbero.DNI,
                Nombre = entidad.Barbero.Nombre,
                Telefono = entidad.Barbero.Telefono,
                Activo = entidad.Barbero.Activo
            };

            DTO.ServicioId = entidad.ServicioId;
            DTO.Servicio = new ServicioDTO
            {
                Id = entidad.Servicio.Id,
                Nombre = entidad.Servicio.Nombre,
                Duracion = entidad.Servicio.Duracion,
                Precio = entidad.Servicio.Precio
            };

            DTO.EstadoTurnoId = entidad.EstadoTurnoId;
            DTO.Estado = new EstadoTurnoDTO
            {
                Id = entidad.EstadoTurno.Id,
                Codigo = entidad.EstadoTurno.Codigo,
                Nombre = entidad.EstadoTurno.Nombre
            };

            return DTO;
        }
    }
}
