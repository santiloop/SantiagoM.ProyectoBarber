using System;
using System.Collections.Generic;
using System.Text;

namespace ProyectoBarber.Shared.DTO
{
    // Paso 3 - Un horario libre para un servicio en una fecha (CU02, y CU12 en la etapa 2).
    // Respuesta de GET api/turno/disponibles.
    public class HorarioDisponibleDTO
    {
        // Hora de inicio posible, en saltos de 5 minutos.
        public TimeOnly Hora { get; set; }

        // Fin estimado si se da el turno a esa hora.
        public TimeOnly HoraFinEstimada { get; set; }

        // Barberos libres a esa hora.
        public List<BarberoLibreDTO> Barberos { get; set; } = new();

        // Barbero que propone el sistema: el que tiene menos turnos ese dia.
        // El recepcionista puede elegir otro de la lista.
        public int BarberoSugeridoId { get; set; }
        public string BarberoSugeridoNombre { get; set; } = string.Empty;
    }

    public class BarberoLibreDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
