using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    // Repositorio de EstadoTurno. Hereda el CRUD generico, pero el controller
    // solo usa las lecturas (los 5 estados se cargan con la migracion).
    public class EstadoTurnoRepositorio : Repositorio<EstadoTurno>, IEstadoTurnoRepositorio
    {
        private readonly AppDbContext context;

        public EstadoTurnoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        // Devuelve el estado con ese codigo, o null si no existe.
        public async Task<EstadoTurno?> SelectByCodigo(string codigo)
        {
            return await context.EstadosTurnos.FirstOrDefaultAsync(e => e.Codigo == codigo);
        }

        // Paso 13 - DTO Resumen armado con una proyeccion (Select), como la catedra.
        public async Task<EstadoTurnoResumenDTO?> SelectResumenByCodigo(string codigo)
        {
            return await context.EstadosTurnos
                .Where(e => e.Codigo == codigo)
                .Select(e => new EstadoTurnoResumenDTO
                {
                    Codigo = e.Codigo,
                    Nombre = e.Nombre,
                    CantidadTurnos = e.Turnos.Count()
                })
                .FirstOrDefaultAsync();
        }
    }
}
