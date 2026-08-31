using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;



namespace ProyectoBarber.BD.Datos.Entity
{
    public class Servicio : EntityBase
    {
        public string nombre { get; set; }
        public int duracion { get; set; }
        public List<Turno> Turnos { get; set; } = new();
    }
}




