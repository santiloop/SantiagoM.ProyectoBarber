using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;


namespace ProyectoBarber.Repositorio
{
    // Repositorio de Cliente (CU07). Usa el CRUD generico de Repositorio<Cliente>.
    public class ClienteRepositorio : Repositorio<Cliente>, IClienteRepositorio
    {
        private readonly AppDbContext context;

        public ClienteRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        // CU07 - Extension 6b: true si otro cliente ya tiene ese telefono.
        public async Task<bool> ExisteTelefono(string telefono, int idExcluir)
        {
            return await context.Clientes.AnyAsync(c => c.Telefono == telefono && c.Id != idExcluir);
        }

        // Paso 11 - Portal del cliente: busca al cliente por su telefono.
        public async Task<Cliente?> SelectByTelefono(string telefono)
        {
            return await context.Clientes.FirstOrDefaultAsync(c => c.Telefono == telefono);
        }
    }
}
