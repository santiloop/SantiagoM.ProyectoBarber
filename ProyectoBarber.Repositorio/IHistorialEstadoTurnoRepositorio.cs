using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    // Paso 4 - Contrato del repositorio del historial de estados (CU05, CU06, C7).
    public interface IHistorialEstadoTurnoRepositorio : IRepositorio<HistorialEstadoTurno>
    {
        // Cambios de estado de un turno, del mas viejo al mas nuevo.
        Task<List<HistorialEstadoDTO>> SelectPorTurno(int turnoId);

        // Paso 8 - Los movimientos de un turno como entidades (para corregir uno),
        // del mas viejo al mas nuevo.
        Task<List<HistorialEstadoTurno>> EntidadesPorTurno(int turnoId);
    }
}
