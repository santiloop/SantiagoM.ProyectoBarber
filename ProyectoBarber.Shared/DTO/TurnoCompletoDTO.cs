using System;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 13 - DTO "Completo" (como EstadoCompletoDTO de la catedra).
    // Trae el turno con sus relaciones como DTO ANIDADOS (no solo los Id):
    // el repositorio usa Include(...) y arma cada DTO hijo.
    // GET api/turno/SelectByIdCompleto/{id}
    public class TurnoCompletoDTO
    {
        public int Id { get; set; }
        public DateTime FechaHora { get; set; }
        public DateTime FechaHoraFinEstimada { get; set; }
        public DateTime FechaAlta { get; set; }
        public string? Observaciones { get; set; }
        public decimal Precio { get; set; }
        public bool Confirmado { get; set; }
        public bool PedidoPorCliente { get; set; }

        public int ClienteId { get; set; }
        public ClienteDTO Cliente { get; set; } = new();

        public int BarberoId { get; set; }
        public BarberoDTO Barbero { get; set; } = new();

        public int ServicioId { get; set; }
        public ServicioDTO Servicio { get; set; } = new();

        public int EstadoTurnoId { get; set; }
        public EstadoTurnoDTO Estado { get; set; } = new();
    }
}
