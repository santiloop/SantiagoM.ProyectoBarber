using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using ProyectoBarber.Shared.DTO;

namespace ProyectoBarber.Repositorio
{
    public class TurnoRepositorio : Repositorio<Turno>, ITurnoRepositorio
    {
        private readonly AppDbContext context;

        public TurnoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<TurnoListadoDTO>> ListaTurnos()
        {
            var lista = await context.Turnos
                .Include(t => t.Cliente)
                .Include(t => t.Barbero)
                .Include(t => t.Servicio)
                .Include(t => t.EstadoTurno)
                .Select(t => new TurnoListadoDTO
                {
                    Id = t.Id,
                    Cliente = t.Cliente.Nombre,
                    Barbero = t.Barbero.nombre,
                    Servicio = t.Servicio.nombre,
                    Estado = t.EstadoTurno.nombre,
                    FechaHora = t.FechaHora
                })
                .ToListAsync();
            return lista;
        }
    }
}