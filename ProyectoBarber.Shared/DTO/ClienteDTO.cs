using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // DTO de Cliente para alta, modificacion y consulta. CU07.
    // Paso 1: Telefono pasa a texto y se agregan validaciones.
    public class ClienteDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio")]
        [MaxLength(200, ErrorMessage = "Máxima longitud 200 caracteres.")]
        public string Nombre { get; set; } = string.Empty;

        [Required(ErrorMessage = "El teléfono es obligatorio")]
        [MaxLength(20, ErrorMessage = "Máxima longitud 20 caracteres.")]
        public string Telefono { get; set; } = string.Empty;
    }
}
