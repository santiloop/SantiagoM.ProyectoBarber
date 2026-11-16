using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // DTO de Servicio para alta, modificacion, consulta y listado. CU09.
    public class ServicioDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "Máxima longitud 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        // Duracion ESTIMADA en minutos. Tiene que ser mayor a 0 (regla de CU09).
        [Range(1, int.MaxValue, ErrorMessage = "La duración tiene que ser mayor a 0 minutos.")]
        public int Duracion { get; set; }

        // Paso 3 - Precio solo informativo. No puede ser negativo.
        [Range(0, 9999999, ErrorMessage = "El precio no puede ser negativo.")]
        public decimal Precio { get; set; }
    }
}
