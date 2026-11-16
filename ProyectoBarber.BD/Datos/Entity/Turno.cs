using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoBarber.BD.Datos.Entity
{
    // Turno: entidad central. Une Cliente, Barbero, Servicio y EstadoTurno
    // con una fecha y hora. Casos de uso: CU02 a CU06 y CU10.
    public class Turno : EntityBase
    {
        [ForeignKey(nameof(Cliente))]
        public int ClienteId { get; set; }
        public Cliente Cliente { get; set; }

        [ForeignKey(nameof(Barbero))]
        public int BarberoId { get; set; }
        public Barbero Barbero { get; set; }

        [ForeignKey(nameof(Servicio))]
        public int ServicioId { get; set; }
        public Servicio Servicio { get; set; }

        [ForeignKey(nameof(EstadoTurno))]
        public int EstadoTurnoId { get; set; }
        public EstadoTurno EstadoTurno { get; set; }

        // Fecha y hora de inicio del turno (en saltos de 5 minutos: 10:00, 10:05, 10:10...).
        public DateTime FechaHora { get; set; }

        // Paso 3 - Fin ESTIMADO = FechaHora + duracion del servicio al momento de dar el turno.
        // Se usa para no superponer turnos del mismo barbero.
        public DateTime FechaHoraFinEstimada { get; set; }

        // Paso 3 - Cuando se cargo el turno.
        public DateTime FechaAlta { get; set; }

        // Paso 3 - Notas opcionales del recepcionista.
        [MaxLength(200, ErrorMessage = "Máxima longitud 200 caracteres.")]
        public string? Observaciones { get; set; }

        // Paso 4 - Hora real en que el recepcionista toco "Iniciar atencion" (puede ser antes de FechaHora).
        public DateTime? FechaHoraInicioReal { get; set; }

        // Paso 4 - Hora real en que el recepcionista toco "Finalizar atencion".
        public DateTime? FechaHoraFinReal { get; set; }

        // Paso 4 - Aviso 2 horas antes: la recepcionista llama al cliente y marca Confirmado.
        public bool Confirmado { get; set; }
        public DateTime? FechaConfirmacion { get; set; }

        // Paso 9 - Precio del servicio AL MOMENTO de dar el turno (o de cambiarle el servicio).
        // Lo usa el reporte (CU10) para que la recaudacion de meses anteriores no cambie
        // cuando se actualiza el precio del servicio.
        [Column(TypeName = "decimal(18,2)")]
        public decimal Precio { get; set; }

        // Paso 11 - true si el turno lo pidio el propio cliente desde "Pedir turno"
        // (portal del cliente). Se usa para el limite de 2 por mes y para marcarlo en la agenda.
        public bool PedidoPorCliente { get; set; }
    }
}
