using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Repositorio
{
    // Repositorio del horario del local (CU15). Usa el CRUD generico de Repositorio<HorarioLocal>.
    public class HorarioLocalRepositorio : Repositorio<HorarioLocal>, IHorarioLocalRepositorio
    {
        private readonly AppDbContext context;

        public HorarioLocalRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<List<HorarioLocal>> SelectPorDia(DayOfWeek dia)
        {
            return await context.HorariosLocal
                .Where(h => h.DiaSemana == dia)
                .OrderBy(h => h.HoraDesde)
                .ToListAsync();
        }
    }
}
