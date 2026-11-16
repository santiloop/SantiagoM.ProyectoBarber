using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace ProyectoBarber.BD.Datos.Entity
{
    // Paso 4 - HistorialEstadoTurno: cada cambio de estado de un turno queda registrado
    // con el estado anterior, el nuevo, cuando fue y el motivo (justificativo).
    // CU05 (cancelar), CU06 (iniciar / finalizar atencion, ausente) y las correcciones (C7).
    // Paso 10 - Se guarda el usuario que hizo el cambio (null en los movimientos anteriores al login).
    public class HistorialEstadoTurno : EntityBase
    {
        [ForeignKey(nameof(Turno))]
        public int TurnoId { get; set; }
        public Turno Turno { get; set; }

        public int EstadoAnteriorId { get; set; }
        public EstadoTurno EstadoAnterior { get; set; }

        public int EstadoNuevoId { get; set; }
        public EstadoTurno EstadoNuevo { get; set; }

        public DateTime FechaHora { get; set; }

        // Obligatorio al cancelar o marcar ausente un turno que estaba En atencion.
        [MaxLength(300, ErrorMessage = "Máxima longitud 300 caracteres.")]
        public string? Motivo { get; set; }

        // Paso 10 - Quien hizo el cambio.
        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }
    }
}
