using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    // Paso 3 - Reglas de la agenda que usan el alta y la reprogramacion de turnos
    // (CU02, CU04) y la busqueda de horarios libres (CU02, CU12).
    public interface IValidadorAgenda
    {
        // Devuelve null si el turno se puede dar, o el mensaje de por que no.
        // idTurnoExcluir = el turno que se esta reprogramando (0 en un alta).
        Task<string?> ValidarTurno(DateTime inicio, Servicio servicio, Barbero barbero, int idTurnoExcluir);

        // Horarios libres de una fecha para un servicio, en saltos de 5 minutos.
        // barberoId opcional: solo los horarios de ese barbero.
        // Paso 8 - idTurnoExcluir: al reprogramar, el propio turno no cuenta como ocupado.
        Task<List<HorarioDisponibleDTO>> Disponibles(DateOnly fecha, Servicio servicio, int? barberoId, int idTurnoExcluir = 0);
    }
}
