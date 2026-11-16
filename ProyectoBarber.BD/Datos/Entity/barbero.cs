using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;


namespace ProyectoBarber.BD.Datos.Entity
{
    // Barbero: empleado que atiende los turnos. No usa el sistema. CU08 (ABM).
    // Paso 1: clase renombrada a Barbero (antes "barbero"), propiedades en PascalCase
    // y "Estado" (texto libre) reemplazado por "Activo" (si / no).
    public class Barbero : EntityBase
    {
        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(10, ErrorMessage = "Máxima longitud 10 caracteres.")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(100, ErrorMessage = "Máxima longitud 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        // Paso 10 - Telefono opcional del barbero.
        [MaxLength(20, ErrorMessage = "Máxima longitud 20 caracteres.")]
        public string? Telefono { get; set; }

        // Un barbero inactivo no recibe turnos nuevos (CU08, CU02).
        public bool Activo { get; set; } = true;

        public List<Turno> Turnos { get; set; } = new();

        // Paso 2 - Dias y horarios en que trabaja el barbero (CU08).
        public List<HorarioBarbero> Horarios { get; set; } = new();
    }
}
