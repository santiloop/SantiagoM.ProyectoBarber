using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 4 - Un cambio de estado de un turno (GET api/turno/{id}/historial).
    public class HistorialEstadoDTO
    {
        // Paso 8 - Id del movimiento (para editarlo) y codigos de los estados.
        public int Id { get; set; }
        public string EstadoAnteriorCodigo { get; set; } = string.Empty;
        public string EstadoNuevoCodigo { get; set; } = string.Empty;
        // Paso 8 - true si es el ultimo movimiento: solo a ese se le puede cambiar el estado.
        public bool EsUltimo { get; set; }
        // Paso 8 - true si el movimiento es una reprogramacion (Pendiente -> Pendiente).
        public bool EsReprogramacion { get; set; }

        public DateTime FechaHora { get; set; }
        public string EstadoAnterior { get; set; } = string.Empty;
        public string EstadoNuevo { get; set; } = string.Empty;
        public string? Motivo { get; set; }

        // Paso 10 - Nombre del usuario que hizo el cambio ("" si fue antes del login).
        public string Usuario { get; set; } = string.Empty;
    }
}
