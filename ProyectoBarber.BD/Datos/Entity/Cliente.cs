using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace ProyectoBarber.BD.Datos.Entity
{
    public class Cliente : EntityBase
    {
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(200, ErrorMessage = "Máxima longitud 200 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El dato es obligatorio")]
        public int Telefono { get; set; }

        public List<Turno> Turnos { get; set; } = new();
    }
}
