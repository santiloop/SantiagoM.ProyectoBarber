using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 8 - Correccion de un movimiento del historial de un turno.
    // PUT api/turno/{id}/historial/{historialId}
    // Se puede corregir la fecha y el motivo de cualquier movimiento, y el estado
    // solo del ULTIMO (el turno pasa a ese estado). Opcion B acordada con el usuario.
    public class EditarHistorialDTO
    {
        public DateTime FechaHora { get; set; }

        [MaxLength(300, ErrorMessage = "Máxima longitud 300 caracteres.")]
        public string? Motivo { get; set; }

        // Codigo del nuevo estado (PEN, ENA, ATE, AUS, CAN). Vacio = no se cambia.
        [MaxLength(3, ErrorMessage = "Máxima longitud 3 caracteres.")]
        public string? CodigoEstadoNuevo { get; set; }
    }
}
