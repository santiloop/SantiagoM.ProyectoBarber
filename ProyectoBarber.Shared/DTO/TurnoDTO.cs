using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // DTO para crear / editar / consultar un turno con los Id de sus relaciones.
    // CU02 (POST), CU04 (GET por id y PUT).
    public class TurnoDTO
    {
        public int Id { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El cliente es obligatorio.")]
        public int ClienteId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El barbero es obligatorio.")]
        public int BarberoId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El servicio es obligatorio.")]
        public int ServicioId { get; set; }

        // Solo lectura: al crear siempre queda Pendiente y el PUT no lo cambia.
        // El estado se cambia con PUT api/turno/{id}/estado (paso 4).
        public int EstadoTurnoId { get; set; }

        // Inicio del turno, en saltos de 5 minutos (ej. "2026-10-08T10:05:00").
        public DateTime FechaHora { get; set; }

        // Solo lectura: inicio + duracion estimada del servicio.
        public DateTime FechaHoraFinEstimada { get; set; }

        [MaxLength(200, ErrorMessage = "Máxima longitud 200 caracteres.")]
        public string? Observaciones { get; set; }
    }
}
