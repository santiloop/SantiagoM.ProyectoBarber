using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 4 - Cuerpo de PUT api/turno/{id}/estado (CU05 y CU06).
    // Codigo del estado nuevo:
    //   ENA = Iniciar atencion     ATE = Finalizar atencion (Atendido)
    //   AUS = Ausente              CAN = Cancelar
    //   PEN = Volver a Pendiente (correccion de un error)
    public class CambioEstadoTurnoDTO
    {
        [Required(ErrorMessage = "El estado es obligatorio")]
        [MaxLength(3, ErrorMessage = "Máxima longitud 3 caracteres.")]
        public string Codigo { get; set; } = string.Empty;

        // Justificativo. Obligatorio al cancelar o marcar ausente un turno En atencion.
        [MaxLength(300, ErrorMessage = "Máxima longitud 300 caracteres.")]
        public string? Motivo { get; set; }
    }
}
