using ProyectoBarber.BD.Datos;
using ProyectoBarber.BD.Datos.Entity;
using System;
using System.Collections.Generic;
using System.Text;


namespace ProyectoBarber.Repositorio
{
    public class ClienteRepositorio : Repositorio<Cliente>, IClienteRepositorio
    {
        public ClienteRepositorio(AppDbContext context) : base(context)
        {
        }
    }
}
