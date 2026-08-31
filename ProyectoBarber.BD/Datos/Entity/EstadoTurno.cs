using System;
using System.Collections.Generic;
using System.Text;


//Preueba6

namespace ProyectoBarber.BD.Datos.Entity
{
    public class EstadoTurno : EntityBase
    {
        public string nombre { get; set; } = string.Empty;
        public List<Turno> Turnos { get; set; } = new();
    }
}