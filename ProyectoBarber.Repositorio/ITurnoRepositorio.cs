using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    // Contrato del repositorio de Turno (CU02 a CU06, CU10).
    public interface ITurnoRepositorio : IRepositorio<Turno>
    {
        // CU03 - Agenda con nombres.
        // Paso 4: filtros opcionales por fecha, barbero y codigo de estado.
        Task<List<TurnoListadoDTO>> ListaTurnos(DateOnly? fecha, int? barberoId, string? estado);

        // Paso 4 - Aviso 2 horas antes: turnos Pendientes, sin confirmar, que empiezan
        // entre ahora y ahora + horas.
        Task<List<TurnoListadoDTO>> AConfirmar(DateTime ahora, int horas);

        // Paso 2 - Turnos en estado Pendiente desde ahora en adelante, con Cliente,
        // Barbero, Servicio y EstadoTurno cargados. Se usa para avisar que turnos
        // quedan afuera cuando cambia un horario o se marca un feriado.
        Task<List<Turno>> PendientesFuturos();

        // Paso 3 - Turnos que OCUPAN horario (Pendiente o En atencion) en una fecha,
        // con Barbero y Servicio cargados. Se usa para calcular horarios libres.
        Task<List<Turno>> OcupadosDelDia(DateOnly fecha);

        // Paso 3 - true si el barbero ya tiene un turno que ocupa horario y se pisa
        // con [inicio, fin). idExcluir = el turno que se esta reprogramando (0 en un alta).
        Task<bool> HaySuperposicion(int barberoId, DateTime inicio, DateTime fin, int idExcluir);

        // Paso 9 - CU10 Reporte: turnos de un periodo (desde y hasta inclusive) con
        // Cliente, Barbero, Servicio y EstadoTurno cargados.
        Task<List<Turno>> TurnosDelPeriodo(DateOnly desde, DateOnly hasta);

        // Paso 11 - Portal del cliente: todos los turnos de un cliente (mas nuevos primero).
        Task<List<TurnoListadoDTO>> TurnosDelCliente(int clienteId);

        // Paso 11 - Cuantos turnos pidio el cliente desde el portal para un mes (por la fecha del turno).
        Task<int> PedidosDelClienteEnMes(int clienteId, int anio, int mes);

        // Paso 13 - Como SelectByIdCompleto de la catedra: el turno con sus relaciones
        // (Include) como DTO anidados. null si no existe.
        Task<TurnoCompletoDTO?> SelectByIdCompleto(int id);
    }
}
