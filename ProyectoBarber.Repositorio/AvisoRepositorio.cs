using System;
using System.Collections.Generic;
using System.Text;
using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using Microsoft.EntityFrameworkCore;

namespace ProyectoBarber.Repositorio
{
    // Paso 12 - Repositorio de avisos (campanita del Dueno y la Recepcionista).
    public class AvisoRepositorio : Repositorio<Aviso>, IAvisoRepositorio
    {
        private readonly AppDbContext context;

        public AvisoRepositorio(AppDbContext context) : base(context)
        {
            this.context = context;
        }

        public async Task Crear(string tipo, string mensaje, DateOnly? fechaReferencia, int? usuarioId)
        {
            context.Avisos.Add(new Aviso
            {
                FechaHora = DateTime.Now,
                Tipo = tipo,
                Mensaje = mensaje.Length > 400 ? mensaje[..400] : mensaje,
                FechaReferencia = fechaReferencia,
                UsuarioId = usuarioId
            });
            await context.SaveChangesAsync();
        }

        public async Task<List<Aviso>> Desde(DateTime desde)
        {
            return await context.Avisos
                .Include(a => a.Usuario)
                .Where(a => a.FechaHora >= desde)
                .OrderByDescending(a => a.Id)
                .ToListAsync();
        }

        public async Task<int> NoLeidos(int usuarioId, int ultimoLeidoId, DateTime desde)
        {
            return await context.Avisos.CountAsync(a => a.Id > ultimoLeidoId
                                                     && a.FechaHora >= desde
                                                     && (a.UsuarioId == null || a.UsuarioId != usuarioId));
        }

        public async Task<int> UltimoId()
        {
            return await context.Avisos.MaxAsync(a => (int?)a.Id) ?? 0;
        }

        public async Task BorrarAnteriores(DateTime limite)
        {
            await context.Avisos.Where(a => a.FechaHora < limite).ExecuteDeleteAsync();
        }
    }
}
