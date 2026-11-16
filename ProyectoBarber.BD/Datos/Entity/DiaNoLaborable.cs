using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.BD.Datos.Entity
{
    // DiaNoLaborable: una fecha puntual en la que el local no atiende (feriado, cierre),
    // aunque el horario de la semana diga que ese dia se trabaja.
    // CU16 - Gestionar dias no laborables. Lo cargan el Dueno y el Recepcionista.
    public class DiaNoLaborable : EntityBase
    {
        // Unica: no puede haber dos registros para la misma fecha (indice en AppDbContext).
        [Required(ErrorMessage = "El dato es obligatorio")]
        public DateOnly Fecha { get; set; }

        [Required(ErrorMessage = "El dato es obligatorio")]
        [MaxLength(100, ErrorMessage = "Máxima longitud 100 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}
