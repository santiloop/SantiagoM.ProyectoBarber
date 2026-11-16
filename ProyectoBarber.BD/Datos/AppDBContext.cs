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
        public DbSet<Barbero> Barberos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<EstadoTurno> EstadosTurnos { get; set; }
        public DbSet<HorarioLocal> HorariosLocal { get; set; }          // Paso 2 - CU15
        public DbSet<HorarioBarbero> HorariosBarberos { get; set; }     // Paso 2 - CU08
        public DbSet<DiaNoLaborable> DiasNoLaborables { get; set; }     // Paso 2 - CU16
        public DbSet<HistorialEstadoTurno> HistorialesEstadosTurnos { get; set; }   // Paso 4 - CU05, CU06
        public DbSet<Aviso> Avisos { get; set; }                         // Paso 12 - CU13

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDbContext()
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Relaciones de Turno: Restrict impide borrar un Cliente, Barbero,
            // Servicio o Estado que tenga turnos (se protege el historial).
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

            // Paso 1 - Indices unicos: la base no acepta datos repetidos.
            modelBuilder.Entity<Cliente>().HasIndex(c => c.Telefono).IsUnique();
            modelBuilder.Entity<Barbero>().HasIndex(b => b.DNI).IsUnique();
            modelBuilder.Entity<Servicio>().HasIndex(s => s.Nombre).IsUnique();
            modelBuilder.Entity<EstadoTurno>().HasIndex(e => e.Codigo).IsUnique();
            modelBuilder.Entity<Usuario>().HasIndex(u => u.NombreUsuario).IsUnique();

            // Paso 1 - Datos iniciales: los 4 estados del turno se cargan con la migracion.
            // CU05 y CU06 los buscan por Codigo.
            modelBuilder.Entity<EstadoTurno>().HasData(
                new { Id = 1, Codigo = "PEN", Nombre = "Pendiente" },
                new { Id = 2, Codigo = "ATE", Nombre = "Atendido" },
                new { Id = 3, Codigo = "AUS", Nombre = "Ausente" },
                new { Id = 4, Codigo = "CAN", Nombre = "Cancelado" },
                new { Id = 5, Codigo = "ENA", Nombre = "En atención" }   // Paso 4 - Iniciar atencion
            );

            // Paso 4 - Historial de estados: dos relaciones a EstadoTurno (anterior y nuevo).
            // Restrict: no se puede borrar un estado usado en el historial.
            // Si se borra el turno (correccion de carga), se borra su historial.
            modelBuilder.Entity<HistorialEstadoTurno>()
                .HasOne(h => h.Turno)
                .WithMany()
                .HasForeignKey(h => h.TurnoId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<HistorialEstadoTurno>()
                .HasOne(h => h.EstadoAnterior)
                .WithMany()
                .HasForeignKey(h => h.EstadoAnteriorId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<HistorialEstadoTurno>()
                .HasOne(h => h.EstadoNuevo)
                .WithMany()
                .HasForeignKey(h => h.EstadoNuevoId)
                .OnDelete(DeleteBehavior.Restrict);

            // Paso 10 - Usuario que hizo cada cambio. Restrict: un usuario con movimientos
            // no se borra (se lo desactiva).
            modelBuilder.Entity<HistorialEstadoTurno>()
                .HasOne(h => h.Usuario)
                .WithMany()
                .HasForeignKey(h => h.UsuarioId)
                .OnDelete(DeleteBehavior.Restrict);

            // Paso 12 - Aviso: quien lo genero. SetNull: si se borra el usuario, el aviso queda.
            modelBuilder.Entity<Aviso>()
                .HasOne(a => a.Usuario)
                .WithMany()
                .HasForeignKey(a => a.UsuarioId)
                .OnDelete(DeleteBehavior.SetNull);

            // Paso 2 - No puede haber dos dias no laborables con la misma fecha.
            modelBuilder.Entity<DiaNoLaborable>().HasIndex(d => d.Fecha).IsUnique();

            // Paso 2 - Horario de un barbero: si se borra el barbero, se borran sus horarios.
            modelBuilder.Entity<HorarioBarbero>()
                .HasOne(h => h.Barbero)
                .WithMany(b => b.Horarios)
                .HasForeignKey(h => h.BarberoId)
                .OnDelete(DeleteBehavior.Cascade);

            // Paso 2 - Horario inicial del local: lunes a viernes de 10 a 13 y de 15 a 21.
            // El Dueno lo puede cambiar despues (CU15): agregar sabado, sacar el martes, etc.
            modelBuilder.Entity<HorarioLocal>().HasData(
                new { Id = 1,  DiaSemana = DayOfWeek.Monday,    HoraDesde = new TimeOnly(10, 0), HoraHasta = new TimeOnly(13, 0) },
                new { Id = 2,  DiaSemana = DayOfWeek.Monday,    HoraDesde = new TimeOnly(15, 0), HoraHasta = new TimeOnly(21, 0) },
                new { Id = 3,  DiaSemana = DayOfWeek.Tuesday,   HoraDesde = new TimeOnly(10, 0), HoraHasta = new TimeOnly(13, 0) },
                new { Id = 4,  DiaSemana = DayOfWeek.Tuesday,   HoraDesde = new TimeOnly(15, 0), HoraHasta = new TimeOnly(21, 0) },
                new { Id = 5,  DiaSemana = DayOfWeek.Wednesday, HoraDesde = new TimeOnly(10, 0), HoraHasta = new TimeOnly(13, 0) },
                new { Id = 6,  DiaSemana = DayOfWeek.Wednesday, HoraDesde = new TimeOnly(15, 0), HoraHasta = new TimeOnly(21, 0) },
                new { Id = 7,  DiaSemana = DayOfWeek.Thursday,  HoraDesde = new TimeOnly(10, 0), HoraHasta = new TimeOnly(13, 0) },
                new { Id = 8,  DiaSemana = DayOfWeek.Thursday,  HoraDesde = new TimeOnly(15, 0), HoraHasta = new TimeOnly(21, 0) },
                new { Id = 9,  DiaSemana = DayOfWeek.Friday,    HoraDesde = new TimeOnly(10, 0), HoraHasta = new TimeOnly(13, 0) },
                new { Id = 10, DiaSemana = DayOfWeek.Friday,    HoraDesde = new TimeOnly(15, 0), HoraHasta = new TimeOnly(21, 0) }
            );
        }
    }
}
