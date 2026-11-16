using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Repositorio
{
    // Paso 4 - Repositorio del historial de estados. Usa el CRUD generico para insertar.
    public class HistorialEstadoTurnoRepositorio : Repositorio<HistorialEstadoTurno>, IHistorialEstadoTurnoRepositorio
    {
        private readonly AppDbContext context;

        public HistorialEstadoTurnoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<HistorialEstadoDTO>> SelectPorTurno(int turnoId)
        {
            var lista = await context.HistorialesEstadosTurnos
                .Where(h => h.TurnoId == turnoId)
                .OrderBy(h => h.FechaHora).ThenBy(h => h.Id)
                .Select(h => new HistorialEstadoDTO
                {
                    Id = h.Id,
                    FechaHora = h.FechaHora,
                    EstadoAnterior = h.EstadoAnterior.Nombre,
                    EstadoNuevo = h.EstadoNuevo.Nombre,
                    EstadoAnteriorCodigo = h.EstadoAnterior.Codigo,
                    EstadoNuevoCodigo = h.EstadoNuevo.Codigo,
                    EsReprogramacion = h.EstadoAnteriorId == h.EstadoNuevoId,
                    Motivo = h.Motivo,
                    Usuario = h.Usuario != null ? h.Usuario.Nombre : ""   // Paso 10
                })
                .ToListAsync();

            // Paso 8 - marca el ultimo movimiento (el unico al que se le puede cambiar el estado).
            if (lista.Count > 0)
                lista[^1].EsUltimo = true;
            return lista;
        }

        // Paso 8 - Movimientos como entidades, sin Include (para editarlos sin
        // arrastrar los EstadoTurno al Update).
        public async Task<List<HistorialEstadoTurno>> EntidadesPorTurno(int turnoId)
        {
            return await context.HistorialesEstadosTurnos
                .Where(h => h.TurnoId == turnoId)
                .OrderBy(h => h.FechaHora).ThenBy(h => h.Id)
                .ToListAsync();
        }
    }
}
