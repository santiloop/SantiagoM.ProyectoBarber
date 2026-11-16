using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // DTO de un dia no laborable (feriado, cierre). CU15.
    public class DiaNoLaborableDTO
    {
        public int Id { get; set; }

        // Formato "2026-10-12"
        public DateOnly Fecha { get; set; }

        [Required(ErrorMessage = "El motivo es obligatorio")]
        [MaxLength(100, ErrorMessage = "Máxima longitud 100 caracteres.")]
        public string Motivo { get; set; } = string.Empty;
    }
}
