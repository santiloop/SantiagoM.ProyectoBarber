using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using ProyectoBarber.Shared.DTO;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Repositorio
{
    // Paso 10 - Repositorio de usuarios. El CRUD generico + busquedas por nombre de usuario.
    public class UsuarioRepositorio : Repositorio<Usuario>, IUsuarioRepositorio
    {
        private readonly AppDbContext context;

        public UsuarioRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task<Usuario?> SelectByNombreUsuario(string nombreUsuario)
        {
            return await context.Usuarios.FirstOrDefaultAsync(u => u.NombreUsuario == nombreUsuario.Trim());
        }

        public async Task<bool> ExisteNombreUsuario(string nombreUsuario, int idExcluir)
        {
            return await context.Usuarios.AnyAsync(u => u.NombreUsuario == nombreUsuario.Trim() && u.Id != idExcluir);
        }

        public async Task<int> DuenosActivos(int idExcluir)
        {
            return await context.Usuarios.CountAsync(u => u.Rol == Roles.Dueno && u.Activo && u.Id != idExcluir);
        }
    }
}
