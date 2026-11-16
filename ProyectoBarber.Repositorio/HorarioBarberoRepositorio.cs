using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Repositorio
{
    // Repositorio de horarios de los barberos (CU08). Usa el CRUD generico de Repositorio<HorarioBarbero>.
    public class HorarioBarberoRepositorio : Repositorio<HorarioBarbero>, IHorarioBarberoRepositorio
    {
        private readonly AppDbContext context;

        public HorarioBarberoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<HorarioBarbero>> SelectPorBarbero(int barberoId)
        {
            return await context.HorariosBarberos
                .Include(h => h.Barbero)
                .Where(h => h.BarberoId == barberoId)
                .OrderBy(h => h.DiaSemana).ThenBy(h => h.HoraDesde)
                .ToListAsync();
        }

        public async Task<List<HorarioBarbero>> SelectPorDia(DayOfWeek dia)
        {
            return await context.HorariosBarberos
                .Include(h => h.Barbero)
                .Where(h => h.DiaSemana == dia)
                .OrderBy(h => h.HoraDesde)
                .ToListAsync();
        }
    }
}
