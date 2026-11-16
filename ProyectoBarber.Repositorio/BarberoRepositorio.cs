using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace ProyectoBarber.Repositorio
{
    // Repositorio de Barbero (CU08). Usa el CRUD generico de Repositorio<Barbero>.
    public class BarberoRepositorio : Repositorio<Barbero>, IBarberoRepositorio
    {
        private readonly AppDbContext context;

        public BarberoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        // CU08 - Extension 6b: true si otro barbero ya tiene ese DNI.
        public async Task<bool> ExisteDNI(string dni, int idExcluir)
        {
            return await context.Barberos.AnyAsync(b => b.DNI == dni && b.Id != idExcluir);
        }
    }
}
