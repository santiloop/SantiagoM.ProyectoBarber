using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace ProyectoBarber.BD.Datos.Entity
{
    public class barbero : EntityBase
    {
        public string DNI { get; set; }

        [Required(ErrorMessage = "El datos es obligatorio")]
        public string nombre { get; set; }

        public string Estado { get; set; } = string.Empty;

        public List<Turno> Turnos { get; set; } = new();
    }
}
