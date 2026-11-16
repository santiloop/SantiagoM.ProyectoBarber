using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Repositorio
{
    // Repositorio de dias no laborables (CU16). Usa el CRUD generico de Repositorio<DiaNoLaborable>.
    public class DiaNoLaborableRepositorio : Repositorio<DiaNoLaborable>, IDiaNoLaborableRepositorio
    {
        private readonly AppDbContext context;

        public DiaNoLaborableRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<bool> ExisteFecha(DateOnly fecha, int idExcluir)
        {
            return await context.DiasNoLaborables.AnyAsync(d => d.Fecha == fecha && d.Id != idExcluir);
        }

        public async Task<List<DiaNoLaborable>> SelectDesde(DateOnly desde)
        {
            return await context.DiasNoLaborables
                .Where(d => d.Fecha >= desde)
                .OrderBy(d => d.Fecha)
                .ToListAsync();
        }
    }
}
