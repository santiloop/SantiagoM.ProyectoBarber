using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Repositorio
{
    // Repositorio de Servicio (CU09). Usa el CRUD generico de Repositorio<Servicio>.
    public class ServicioRepositorio : Repositorio<Servicio>, IServicioRepositorio
    {
        private readonly AppDbContext context;

        public ServicioRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        // CU09 - Extension 6b: true si otro servicio ya tiene ese nombre.
        public async Task<bool> ExisteNombre(string nombre, int idExcluir)
        {
            return await context.Servicios.AnyAsync(s => s.Nombre == nombre && s.Id != idExcluir);
        }
    }
}
