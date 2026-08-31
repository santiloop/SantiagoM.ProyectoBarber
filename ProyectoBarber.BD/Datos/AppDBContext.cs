using Microsoft.EntityFrameworkCore;
using ProyectoBarber.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.BD.Datos
{
    public class AppDbContext : DbContext
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Servicio> Servicios { get; set; }
        public DbSet<Turno> Turnos { get; set; }
        public DbSet<barbero> Barberos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<EstadoTurno> EstadosTurnos { get; set; }

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Cliente)
                .WithMany(c => c.Turnos)
                .HasForeignKey(t => t.ClienteId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Barbero)
                .WithMany(b => b.Turnos)
                .HasForeignKey(t => t.BarberoId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.Servicio)
                .WithMany(s => s.Turnos)
                .HasForeignKey(t => t.ServicioId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Turno>()
                .HasOne(t => t.EstadoTurno)
                .WithMany(e => e.Turnos)
                .HasForeignKey(t => t.EstadoTurnoId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
    //public class AppDBContext : DbContext
    //{

    //    public DbSet<Persona> Personas { get; set; }
    //    public AppDBContext(DbContextOptions options) : base(options)
    //    {
    //    }

    //    protected AppDBContext()
    //    {
    //    }
    //}
}
