using Microsoft.EntityFrameworkCore;
using ProyectoBarber.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.BD.Datos
{
    public class AppDBContext : DbContext
    {

        public DbSet<Persona> Personas { get; set; }
        public AppDBContext(DbContextOptions options) : base(options)
        {
        }

        protected AppDBContext()
        {
        }
    }
}
