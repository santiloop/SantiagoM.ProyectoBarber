using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // DTO de Barbero para alta, modificacion y consulta. CU08.
    // Paso 1: Nombre en PascalCase, Estado reemplazado por Activo y validaciones
    // (las usa el EditForm de Blazor, como PaisDTO de la catedra).
    public class BarberoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El DNI es obligatorio")]
        [MaxLength(10, ErrorMessage = "Máxima longitud 10 caracteres.")]
        public string DNI { get; set; } = string.Empty;

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(100, ErrorMessage = "Máxima longitud 100 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        public bool Activo { get; set; } = true;

        // Paso 10 - Telefono opcional.
        [MaxLength(20, ErrorMessage = "Máxima longitud 20 caracteres.")]
        public string? Telefono { get; set; }
    }
}
