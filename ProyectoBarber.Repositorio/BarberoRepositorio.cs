using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Text;


namespace ProyectoBarber.Repositorio
{
    public class BarberoRepositorio : Repositorio<barbero>, IBarberoRepositorio
    {
        public BarberoRepositorio(AppDbContext context) : base(context)
        {
        }
    }
}
