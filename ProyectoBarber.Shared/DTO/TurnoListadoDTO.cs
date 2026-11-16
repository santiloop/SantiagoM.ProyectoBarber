using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // DTO de listado: el turno con NOMBRES en lugar de Id. CU03 (agenda).
    public class TurnoListadoDTO
    {
        public int Id { get; set; }
        public string Cliente { get; set; } = string.Empty;

        // Paso 4 - Para que la recepcionista pueda llamar al cliente (aviso 2 horas antes).
        public string ClienteTelefono { get; set; } = string.Empty;

        public string Barbero { get; set; } = string.Empty;
        public string Servicio { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;

        // Paso 4 - PEN, ENA, ATE, AUS o CAN (para mostrar los botones que corresponden).
        public string EstadoCodigo { get; set; } = string.Empty;

        public DateTime FechaHora { get; set; }

        // Paso 3 - Fin estimado (inicio + duracion del servicio).
        public DateTime FechaHoraFinEstimada { get; set; }

        // Paso 4 - Horas reales de Iniciar / Finalizar atencion (vacias hasta que se marquen).
        public DateTime? FechaHoraInicioReal { get; set; }
        public DateTime? FechaHoraFinReal { get; set; }

        // Paso 4 - El cliente confirmo que viene.
        public bool Confirmado { get; set; }

        // Paso 8 - Observaciones del turno (se muestran en la agenda y en el historial).
        public string? Observaciones { get; set; }

        // Paso 11 - Para el portal del cliente (reprogramar con el mismo servicio)
        // y para marcar en la agenda los turnos que pidio el cliente.
        public int ServicioId { get; set; }
        public int BarberoId { get; set; }
        public bool PedidoPorCliente { get; set; }
    }
}
